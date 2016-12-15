using UnityEngine;
using System.Collections.Generic;

public class Lobby : Photon.PunBehaviour {

    private List<string> rooms = new List<string>();

    public void JoinRndRoom()
    {
        PhotonNetwork.JoinRandomRoom();

    }

    public void CreateRndRoom()
    {
        string rndRoomName = "room" + (int)Random.Range(0.0f, 1000.0f);
        while (rooms.IndexOf(rndRoomName) != -1)
        {
            rndRoomName = "room" + (int)Random.Range(0.0f, 1000.0f);
        }
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(rndRoomName, roomOptions, TypedLobby.Default);
        Debug.Log(rndRoomName);
    }

    public void OnPhotonRandomJoinFailed()
    {
        CreateRndRoom();
    }

    public void OnJoinedRoom()
    {
        Loading.Load(LoadingScene.Game);
    }

}
