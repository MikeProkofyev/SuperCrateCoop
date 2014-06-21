using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public string gameName = "SuperCoopCrates";
	public GameObject playerPrefab;
	public Transform spawnObject;

	public GameObject minion;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public int minionsCount;
	public Transform spawnPoint;
	public bool spawnWaves = true;
	public GUIText scoreText;
	
	private int score = 0;
	private bool gameOver = false;

	float buttonX;
	float buttonY;
	float buttonW;
	float buttonH;

	bool refreshing = false;
	HostData[] hostData;


	// Use this for initialization
	void Start () {
		buttonX = Screen.width * 0.05f;
		buttonY = Screen.width * 0.05f;
		buttonW = Screen.width * 0.1f;
		buttonH = Screen.width * 0.1f;
		
	}

	void startServer() {
		bool useNat = !Network.HavePublicAddress ();
		Network.InitializeServer (2, 25001, useNat);
		MasterServer.RegisterHost (gameName, gameName, "Infinite coop arcade");
	}

	void refreshHostList() {
		MasterServer.RequestHostList (gameName);
		refreshing = true;
	}

	void Update () {
		if(refreshing && MasterServer.PollHostList().Length > 0) {
			refreshing = false;
			hostData = MasterServer.PollHostList();
		}

	}

	void spawnPlayer() {
		Network.Instantiate (playerPrefab, spawnObject.position, Quaternion.identity, 0);
	}

	//Messages
	void OnServerInitialized() {
		print ("Server Initialized");
		spawnPlayer();
		if (Network.isServer && spawnWaves) {
			networkView.RPC("UpdateScore", RPCMode.All, 0);
			StartCoroutine (SpawnWaves ());
		}
	}

	void OnConnectedToServer() {
		spawnPlayer();
		networkView.RPC("UpdateScore", RPCMode.All, 0);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent) {
		if (msEvent == MasterServerEvent.RegistrationSucceeded)
			print ("Registered Server");
	}

	[RPC]
	void StartSpawnWaves(){
			StartCoroutine (SpawnWaves ());

	}
	
	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (startWait);
		while (true) {
			for (int i = 0; i < minionsCount; i++) {
				GameObject newMinion = Network.Instantiate (minion, spawnPoint.position, spawnPoint.rotation, 1) as GameObject;
				if (Random.Range(0, 2) == 1)
					newMinion.GetComponent<MinionRun>().SendMessage("CallRPCChangeDirection");
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds(waveWait);
			
			if (gameOver) 
			{
				//				restartText.text = "Press 'R' for restart";
				//				restart = true;
				break;
			}
		}
	}

	//GUI
	void OnGUI() {
		if (!Network.isClient && !Network.isServer) {
			if (GUI.Button (new Rect (buttonX, buttonY, buttonW, buttonH), "Start Server")) {
				print("Starting Server");
				startServer();
			}

			if (GUI.Button (new Rect (buttonX, buttonY * 1.2f + buttonH, buttonW, buttonH), "Refresh Hosts")) {
				print("Refreshing hosts");
				refreshHostList();
			}

			if (hostData != null && hostData.Length != 0)
							for (int hostIndex = 0; hostIndex < hostData.Length; hostIndex++)
									if (GUI.Button (new Rect (buttonX * 1.5f + buttonW, buttonY * 1.2f + (buttonH * hostIndex), buttonW * 3, buttonH * 0.5f), hostData [hostIndex].gameName))
											Network.Connect (hostData [hostIndex]);
		}
	}

	[RPC]
	void UpdateScore (int newScoreValue) {
		score += newScoreValue;
		scoreText.text = "Score: " + score;
	}
	

	public void AddScore (int newScoreValue) {
		networkView.RPC("UpdateScore", RPCMode.All, newScoreValue);
//		UpdateScore (newScoreValue);
	}
}
