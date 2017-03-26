using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;

using System;

public class ServerGM : NetworkManager {


    #region UI
    public Toggle OnOffToggle;
    public Canvas UICanvas;
    public Text IPAddressText;
    public InputField PortInput;

    void UpdatePortText() {
        PortInput.text = networkPort.ToString();
        }

    void UpdateIPText() {
        IPAddressText.text = Network.player.ipAddress.ToString();
        }

    #endregion

    #region defaults
    int mDefaultPort = 32000;
    #endregion



    #region ServerControl

    //Check if run on system without GUI, like a headless Linux server
    bool isHeadless {
        get {
            return SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
            }
        }

    public void SetupServer() {
        if (isHeadless) {
            StartServer();
            Debug.Log("StartServer() Headless mode");
            }
        }

    public void SetToggle() {       //Used by UI to start and stop server
        if (OnOffToggle.isOn) {
            if(!NetworkServer.active) {
                StartServer();
            }
        } else {
            if (NetworkServer.active) {
                StopServer();
            }
        }
    }
    #endregion


    public override void OnClientConnect(NetworkConnection vConn) {
        Debug.LogFormat("Client Connected {0}", vConn.address, vConn);
        base.OnClientConnect(vConn);

    }

    public override void OnClientDisconnect(NetworkConnection vConn) {
        Debug.LogFormat("Client Disconnected {0}", vConn.address, vConn);
        base.OnClientDisconnect(vConn);
    }
    public override void OnClientError(NetworkConnection vConn, int vErrorCode) {
        Debug.LogFormat("Client Error {0} Code {1}", vConn.address, vErrorCode);
        base.OnClientError(vConn, vErrorCode);
    }

    void RunSpawner(bool vRun) {
        if (mSpawner != null) {
            StopCoroutine(mSpawner);
        }
        if (vRun) {
            mSpawner = StartCoroutine(MakeEnemy());
        }
    }




    private void Start() {
        mDiscovery = GetComponent<NetworkDiscovery>();
        mDiscovery.Initialize();
        StartUp();
    }

    void StartUp()
    {
        networkPort = mDefaultPort;
        if (isHeadless)
        {       //Disable UI canvas
            UICanvas.enabled = false;
        }
        else
        {
            UpdatePortText();
            UpdateIPText();
        }
        StartCoroutine(ShowDebug());
        SetupServer();
    }

    NetworkDiscovery mDiscovery;        //Reference to Discovery Script

    public override void OnStartServer ()
	{
		base.OnStartServer ();
		RunSpawner (true);
		Debug.Log (System.Reflection.MethodBase.GetCurrentMethod ().Name);
        mDiscovery.broadcastData = Environment.UserName + "@" + Environment.MachineName;
        if(!mDiscovery.running) {
            mDiscovery.StartAsServer();
        }
    }

	public override void OnStopServer ()
	{
		base.OnStopServer ();
		RunSpawner (false);
		Debug.Log (System.Reflection.MethodBase.GetCurrentMethod ().Name);
        if (mDiscovery.running) {
            mDiscovery.StopBroadcast();
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		GameObject player = (GameObject)Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.identity);
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
	}



	#region Enemies
	public	GameObject	Enemy;

	Coroutine	mSpawner;

	IEnumerator	MakeEnemy() {
		while (true) {
			if (NetworkServer.active) {
				GameObject	tGO = Instantiate (Enemy);
				Destroy (tGO, 5f);
				tGO.transform.position = new Vector3 (UnityEngine.Random.Range (-5f, 5f), 1f, UnityEngine.Random.Range (-5f, 5f));
				NetworkServer.Spawn (tGO);
			}
			yield	return	new	WaitForSeconds (1f);
		}
	}
    #endregion


    #region Debug
    void OnEnable() {
		//Application.logMessageReceived += HandleLog;
	}
	void OnDisable() {
		//Application.logMessageReceived -= HandleLog;
	}
	void HandleLog(string vLogString, string vStackTrace, LogType vType) {
		DebugLogText.text += vLogString + "\n";
	}

	public	Text	DebugText;
	public	Text	DebugLogText;

	IEnumerator	ShowDebug() {
		while (true) {
			StringBuilder tSB = new StringBuilder ();
			if (NetworkServer.active) {
				tSB.AppendLine("Active");
				for (int tI=0; tI<NetworkServer.connections.Count; tI++) {
					if (NetworkServer.connections [tI] != null) {
						tSB.AppendFormat("{0}\n",NetworkServer.connections [tI].address);				
					}
				}
			} else {
				tSB.AppendLine("Not active");				
			}
			DebugText.text = tSB.ToString ();
			yield	return	new WaitForSeconds (0.25f);
		}
	}
    #endregion
}
