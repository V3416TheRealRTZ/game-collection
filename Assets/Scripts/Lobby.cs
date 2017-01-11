using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;

public class Lobby : Photon.PunBehaviour {

    private List<string> rooms = new List<string>();
    public Text helloText;
    public Text moneyText;
    public Text scoreText;
    public Text ratingText;

    void Start()
    {
        scoreText.text = PlayerInfo.Score.ToString();
        ratingText.text = PlayerInfo.Rating.ToString();
        moneyText.text = PlayerInfo.Money.ToString();
        PlayerInfo.UpdateScore();
        PlayerInfo.UpdateMoney();
        PlayerInfo.UpdateInventory();
        helloText.text += PlayerInfo.DisplayName;
    }

    void Update()
    {
        if (PlayerInfo.IsScoreChanged)
        {
            scoreText.text = PlayerInfo.Score.ToString();
            ratingText.text = PlayerInfo.Rating.ToString();
        }
        if (PlayerInfo.IsMoneyChanged)
        {
            if (moneyText != null) moneyText.text = PlayerInfo.Money.ToString();
        }
    }

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

    public override void OnJoinedRoom()
    {
        Loading.Load(LoadingScene.Game);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Shop()
    {
        Loading.Load(LoadingScene.Shop);
    }

    public void Rating()
    {
        Loading.Load(LoadingScene.Rating);
    }
}
