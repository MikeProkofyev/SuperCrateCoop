using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public int scoreValue = 10;
	public float hp = 100;

	private NetworkManager gameController;

	void Start () {
		GameObject gameControllerObject = GameObject.FindWithTag("NetworkManager");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent <NetworkManager> ();
		} else {
			Debug.Log ("Cannot find 'NetworkManagerObject' ");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (hp <= 0 && networkView.isMine) {
			Network.Destroy(gameObject);
			gameController.AddScore(scoreValue);
		}
	}


}
