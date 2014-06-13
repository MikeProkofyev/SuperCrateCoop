using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {

	public Transform groundCheck;
	public LayerMask whatIsGround;


	Animator anim;
	bool grounded = false;
	float groundRadius = 0.2f;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}

	void FixedUpdate () {
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		anim.SetBool ("Ground", grounded);
		
		anim.SetFloat ("vSpeed", rigidbody2D.velocity.x);
	}
}
