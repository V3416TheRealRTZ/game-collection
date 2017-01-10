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
        getInfo();
        string dispayName = "";
        if (PlayerPrefs.HasKey("DispayName"))
            dispayName = PlayerPrefs.GetString("DisplayName");
        else if (PlayerPrefs.HasKey("Username"))
            dispayName = PlayerPrefs.GetString("Username");
        helloText.text += dispayName;
    }

    public void getInfo()
    {
        GetLeaderboardAroundPlayerRequest req = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = "Score",
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(req, (GetLeaderboardAroundPlayerResult res) =>
        {
            if (res.Leaderboard != null)
            {
                scoreText.text = res.Leaderboard[0].StatValue.ToString();
                ratingText.text = (res.Leaderboard[0].Position+1).ToString();
            }
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));

        AddUserVirtualCurrencyRequest reqCur = new AddUserVirtualCurrencyRequest()
        {
            VirtualCurrency = "GO",
            Amount = 0,
        };
        PlayFabClientAPI.AddUserVirtualCurrency(reqCur, (ModifyUserVirtualCurrencyResult res) =>
        {
            if (res != null) moneyText.text = res.Balance.ToString();
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));
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

    public void OnJoinedRoom()
    {
        Loading.Load(LoadingScene.Game);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
