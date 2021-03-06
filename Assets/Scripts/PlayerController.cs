﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float maxSpeed  = 10f;
	public float jumpForce = 700;
	public float throwForce = 0.1f;
	public bool facingLeft = true;
	public Transform groundCheck;
	public LayerMask whatIsGround;
	public GameObject minePrefab;

	public GameObject bulletPrefab;
	public Transform bulletSpawnPoint;
	public float fireRate = 10f;
	public float mineThrowingRate = 600f;

	float lastFireTime = -1f;
	float lastMineThrowTime = -1f;
	bool grounded = false;
	float groundRadius = 0.2f;
	float move = 0;
	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}

	void FixedUpdate () {
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		anim.SetBool ("Ground", grounded);
		
		anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);

		if (networkView.isMine)
			networkView.RPC("setMoveValue", RPCMode.All, Input.GetAxis ("Horizontal"));
		
		anim.SetFloat ("Speed", Mathf.Abs(move));
		rigidbody2D.velocity = new Vector2 (move * maxSpeed, rigidbody2D.velocity.y);
		if (move < 0 && !facingLeft)
			Flip ();
		else if (move > 0 && facingLeft)
			Flip ();
	}

	void Update () {
		if(grounded && Input.GetButtonDown("Jump") && networkView.isMine)
			networkView.RPC("jump", RPCMode.All);


		if (Input.GetButton("Fire1") && anim.GetBool("Ground") && move == 0 && networkView.isMine) {
			if (Time.time > lastFireTime + 1 / fireRate){ 
				networkView.RPC("Shoot", RPCMode.All);
				lastFireTime = Time.time;
			}
		} else if (anim.GetBool("Shooting") && networkView.isMine) {
			networkView.RPC("StopShooting", RPCMode.All);
		} else if (Input.GetButtonDown("Fire2") && anim.GetBool("Ground") && move == 0 && networkView.isMine) {
			if (Time.time > lastMineThrowTime + 1 / mineThrowingRate){ 
				networkView.RPC("ThrowMine", RPCMode.All);
				lastMineThrowTime = Time.time;
			}
		}else if (anim.GetBool("ThrowingMine") && networkView.isMine && (Time.time > lastMineThrowTime + mineThrowingRate)) {
			networkView.RPC("StopThrowingMines", RPCMode.All);
		}
	}

	[RPC]
	void Shoot () {
		if(!anim.GetBool("Shooting")) {
			anim.SetBool("Shooting", true);
		}
		Vector3 newBulletSpawnPosition = new Vector3 (bulletSpawnPoint.position.x, bulletSpawnPoint.position.y + Random.Range (-0.25F, 0.25F));
		GameObject bullet = Network.Instantiate (bulletPrefab, newBulletSpawnPosition, bulletSpawnPoint.rotation, 1) as GameObject;
		Vector3 bulletScale = bullet.transform.localScale;
		if (!facingLeft) {
			bullet.GetComponent<Bullet> ().shootingtLeft = false;
			bulletScale.x *= -1;
			bullet.transform.localScale = bulletScale;
		}
	}

	[RPC]
	void ThrowMine() {
		if(!anim.GetBool("ThrowingMine"))
			anim.SetBool("ThrowingMine", true);

		Invoke("CreateMine", 0.5f);
	}

	

	[RPC]
	void CreateMine() {
		GameObject mine = Instantiate (minePrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation) as GameObject;
		int multiplier = facingLeft ? -1 : 1;
		mine.rigidbody2D.AddForce (new Vector2 (multiplier * throwForce, 350)); 
	}

	[RPC]
	void StopThrowingMines() {
		anim.SetBool("ThrowingMine", false);
	}


	[RPC]
	void StopShooting() {
		anim.SetBool("Shooting", false);
	}

	[RPC]
	void setMoveValue(float move) {
		this.move = move;
	}

	[RPC]
	void jump() {
		anim.SetBool("Ground", false);
		rigidbody2D.AddForce(new Vector2(0, jumpForce));
	}

	void Flip () {
		facingLeft = !facingLeft;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
