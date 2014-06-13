using UnityEngine;
using System.Collections;

public class DestroyAfterSeconds : MonoBehaviour {

	public float lifeTime = 0.1f;

	float timer;

	void Awake() {
		timer = Time.time;
	}
	
	void Update() {
		if (Time.time - timer > lifeTime && networkView.isMine){
			Network.RemoveRPCsInGroup(3);
			Network.Destroy(gameObject);
		}
	}
}
