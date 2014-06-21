using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	
	void OnTriggerStay2D(Collider2D other) {
		if(networkView.isMine) {
			switch (other.gameObject.tag) {
			case "Minion":
				other.GetComponent<Health> ().hp -= 9000;
				break;
//			case "Player":
//				other.GetComponent<Health> ().hp -= 9000;
//				break;
			}
		}
		Network.Destroy (gameObject);
	}
}
