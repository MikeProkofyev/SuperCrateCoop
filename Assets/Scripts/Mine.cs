using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {

	public Transform groundCheck;
	public LayerMask whatIsGround;


	Animator anim;
	bool grounded = false;
	float groundRadius = 0.2f;
	float spawnTime;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		spawnTime = Time.time;
	}

	void FixedUpdate () {
		if(gameObject.GetComponent<Rigidbody2D>() != null) {
			anim.SetFloat ("vSpeed", rigidbody2D.velocity.x * 2);
			grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
			anim.SetBool ("Ground", grounded);
		}
	}


	void OnTriggerEnter2D(Collider2D other) {
		if(networkView.isMine) {
			switch (other.gameObject.tag) {
			case "Minion":
				networkView.RPC ("Explode", RPCMode.All);
				break;
			case "Player":
				networkView.RPC ("Explode", RPCMode.All);
				break;
			case "Mine":
				networkView.RPC ("Explode", RPCMode.All);
				break;
			case "Bullet":
				networkView.RPC ("Explode", RPCMode.All);
				break;
			}
		}
	}

	[RPC]
	void Explode() {
		anim.SetBool("exploding", true);
		Destroy (rigidbody2D);
		Destroy(gameObject, 0.7f);
	}
}
