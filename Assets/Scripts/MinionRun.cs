using UnityEngine;
using System.Collections;

public class MinionRun : MonoBehaviour {

	public float speed  = 10f;
	public bool runningLeft = true;
	public float collisionFrequency = 100f;
	public bool facingLeft = true;
	float lastCollisionTime = -1f;


	// Use this for initialization
	void Start () {
		
	}

	void Update () {
		if(networkView.isMine) 
			networkView.RPC("UpdateRunning", RPCMode.All);
		
		
	}

	[RPC]
	void UpdateRunning() {
		int direction = runningLeft ? -1 : 1;
		rigidbody2D.velocity = new Vector2 (speed * direction, rigidbody2D.velocity.y);
		
		if (rigidbody2D.velocity.x < 0 && !facingLeft)
			Flip ();
		else if (rigidbody2D.velocity.x > 0 && facingLeft)
			Flip ();
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (Time.time > lastCollisionTime && other.gameObject.tag == "Obstacle") {
			runningLeft = !runningLeft;
			lastCollisionTime = Time.time + Time.deltaTime * 2;
		}
	}
	

	[RPC]
	void ChangeDirection () {
		runningLeft = !runningLeft;
	}

	void CallRPCChangeDirection(){
		networkView.RPC("ChangeDirection", RPCMode.All);
	}


	void Flip () {
		print ("flipping");
		facingLeft = !facingLeft;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
