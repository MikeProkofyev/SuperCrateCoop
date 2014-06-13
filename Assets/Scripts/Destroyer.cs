using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {
	

	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other){
		if(networkView.isMine)
			Network.Destroy (other.gameObject);
	}

}
