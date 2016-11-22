using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Launcher : Photon.PunBehaviour
{
    public string version = "0.0.1";
    // Use this for initialization

    #region Public Properties

    [Tooltip("The UI Panel to let the user enter name, connect and play")]
    public GameObject controlPanel;
    [Tooltip("The UI Label to inform the user about connection")]
    public GameObject progressLabel;

    #endregion
    private List<string> rooms = new List<string>();

    #region MonoBehaviour CallBacks

    void Awake()
    {
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    #endregion

    #region Public Methods

    public void Connect()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings(version);
        }
    }

    #endregion

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    #region Photon.PunBehaviour CallBacks

    public override void OnConnectedToMaster()
    {
        progressLabel.GetComponent<Text>().text = "Connected as " + PhotonNetwork.playerName.ToString();

    }



    public override void OnJoinedLobby()
    {
        Debug.Log("Joined to Lobby");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("OnPhotonRandomJoinFailed");
        string rndRoomName = "room" + (int)Random.Range(0.0f, 1000.0f);
        while (rooms.IndexOf(rndRoomName) != -1)
        {
            rndRoomName = "room" + (int)Random.Range(0.0f, 1000.0f);
        }
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(rndRoomName, roomOptions, TypedLobby.Default);
        Debug.Log(rndRoomName);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("OnJoinedRoom start scene");
        //SceneManager.LoadScene("RunnerGame");    
        PhotonNetwork.LoadLevel("2 RunnerGame");
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("StartGame");
    }

    public override void OnDisconnectedFromPhoton()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }



    #endregion
}