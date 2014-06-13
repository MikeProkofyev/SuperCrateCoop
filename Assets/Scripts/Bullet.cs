using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	public float speed = 20f;
	public float lifeTime = 0.5f;
	public float distance = 10000f;
	public bool shootingtLeft = true;
	public GameObject obstacleHitSFXPrefab;

	float spawnTime = 0f;

	void Start () {
//		if(!networkView.isMine)
//			enabled = false;
		spawnTime = Time.time;
	}
	
	void Update () {
		if(networkView.isMine) {
			networkView.RPC("UpdatePosition", RPCMode.All);
			if (Time.time > spawnTime + lifeTime || distance < 0) {
				Network.RemoveRPCsInGroup(1);
				Network.Destroy (gameObject);
			}
		}
	}


	[RPC]
	void UpdatePosition() {
		if (shootingtLeft)
			transform.localPosition += -transform.right * speed * Time.deltaTime;
		else
			transform.localPosition += transform.right * speed * Time.deltaTime;
		distance -= speed * Time.deltaTime;
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if(networkView.isMine) {
			switch (other.gameObject.tag) {
				case "Minion":
					other.GetComponent<Health> ().hp -= 10;
					break;
				case "Obstacle":
					Network.Instantiate(obstacleHitSFXPrefab, transform.position, transform.rotation, 3);
					break;
			}
			Network.RemoveRPCsInGroup(1);
			Network.Destroy (gameObject);
		}
	}
}
