using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkGM : NetworkManager {

	int		defaultPort=32000;
	string	defaultIP="localhost";

	public	Toggle		OnOffToggle;
	public	Dropdown	WhatDropdown;
	public	Canvas 		UICanvas;


	bool	isHeadless {
		get {
			return	SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
		}
	}

	public	GameObject	UFO;

	NetworkClient mClient;

	Coroutine	mSpawner;

	void	Awake() {
		if (isHeadless) {		//Disable UI canvas
			UICanvas.enabled = false;
		}
	}

	void	Start() {
		SetupSystem ();
	}

	public void SetupSystem()
	{
		networkPort = defaultPort;
		networkAddress = defaultIP;

		if (isHeadless) {
			StartServer ();
			Debug.Log ("StartServer() Headless mode");
		}
	}

	public	void	SetToggle() {
		if (OnOffToggle.isOn) {
			switch (WhatDropdown.value) {
			case 0:
				if (StartServer ()) {
					Debug.Log ("StartServer()");
				}
				break;
			case 1:
				mClient = StartClient ();
				Debug.LogFormat ("StartClient() {0}:{1}", mClient.serverIp, mClient.serverPort);
				break;

			case 2:
				mClient = StartHost ();
				Debug.LogFormat ("StartHost() {0}:{1}", mClient.serverIp, mClient.serverPort);
				break;
			}
		} else {
			Stop ();
		}
	}

	void	Stop() {
		if (NetworkServer.active && NetworkClient.active) {
			StopHost ();
			Debug.Log ("Stopping Host");
		} else 	if (NetworkClient.active) {
			StopClient ();
			Debug.Log ("Stopping Client");
		} else if (NetworkServer.active) {
			StopServer ();
			Debug.Log ("Stopping Server");
		}
	}

	public override void	OnClientConnect(NetworkConnection vConn)  {
		Debug.LogFormat ("Client Connected {0}",vConn.address,vConn);
		base.OnClientConnect (vConn);

	}

	public override void	OnClientDisconnect(NetworkConnection vConn) {
		Debug.LogFormat ("Client Disconnected {0}",vConn.address,vConn);
		base.OnClientDisconnect (vConn);
	}
	public override	void OnClientError(NetworkConnection vConn, int vErrorCode){
		Debug.LogFormat ("Client Error {0} Code {1}",vConn.address,vErrorCode);
		base.OnClientError (vConn, vErrorCode);
	}

	public override	void OnServerConnect(NetworkConnection vConn) {
		Debug.LogFormat ("Server-Client Connected {0}",vConn.address,vConn);
		base.OnServerConnect (vConn);
	}

	public override void OnServerDisconnect(NetworkConnection vConn) {
		Debug.LogFormat ("Server-Client Disconnected {0}",vConn.address,vConn);
		base.OnServerDisconnect (vConn);
	}

	void	RunSpawner(bool vRun) {
		if (mSpawner != null) {
			StopCoroutine (mSpawner);
		}
		if (vRun) {
			mSpawner = StartCoroutine (MakeUFO ());
		}
	}

	public override void OnStartServer ()
	{
		base.OnStartServer ();
		RunSpawner (true);
		Debug.Log (System.Reflection.MethodBase.GetCurrentMethod ().Name);
	}

	public override void OnStopServer ()
	{
		base.OnStopServer ();
		RunSpawner (false);
		Debug.Log (System.Reflection.MethodBase.GetCurrentMethod ().Name);
	}

	public override void OnStartHost ()
	{
		base.OnStartHost ();
		Debug.Log (System.Reflection.MethodBase.GetCurrentMethod ().Name);
	}

	public override void OnStopHost ()
	{
		base.OnStopHost ();
		Debug.Log (System.Reflection.MethodBase.GetCurrentMethod ().Name);
	}

	public override void OnStartClient (NetworkClient client)
	{
		base.OnStartClient (client);
		Debug.Log (System.Reflection.MethodBase.GetCurrentMethod ().Name);
	}

	public override void OnStopClient ()
	{
		base.OnStopClient ();
		Debug.Log (System.Reflection.MethodBase.GetCurrentMethod ().Name);
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
	}

	IEnumerator	MakeUFO() {
		while (true) {
			if (NetworkServer.active) {
				GameObject	tGO = Instantiate (UFO);
				Destroy (tGO, 5f);
				tGO.transform.position = new Vector2 (Random.Range (-5f, 5f), Random.Range (-5f, 5f));
				NetworkServer.Spawn (tGO);
			}
			yield	return	new	WaitForSeconds (1f);
		}
	}
		
}
