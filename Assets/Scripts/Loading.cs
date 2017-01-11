using UnityEngine;
using System.Collections;

public enum LoadingScene
{
    Lobby,
    Game,
    Register,
    Shop,
    Rating,
    Login
}

public class Loading : MonoBehaviour {

    private static LoadingScene _nextScene { get; set; }

    void Update()
    {
        if (PlayerInfo.IsMoneyChanged)
            PhotonNetwork.networkingPeer.OpJoinLobby(TypedLobby.Default);
    }

    void Start()
    {
        if (_nextScene == LoadingScene.Lobby)
        {
            PlayerInfo.UpdateScore();
            PlayerInfo.UpdateMoney();
            PlayerInfo.UpdateInventory();
            PlayerInfo.UpdateTitleUserData();
        }
        else if (_nextScene == LoadingScene.Game)
            PhotonNetwork.LoadLevel("4 Game");
        else if (_nextScene == LoadingScene.Register)
            PhotonNetwork.LoadLevel("5 Register");
        else if (_nextScene == LoadingScene.Shop)
            PhotonNetwork.LoadLevel("6 Shop");
        else if (_nextScene == LoadingScene.Rating)
            PhotonNetwork.LoadLevel("7 Rating");
        else if (_nextScene == LoadingScene.Login)
            PhotonNetwork.LoadLevel("1 Login");
    }

	void OnJoinedLobby()
    {
        PhotonNetwork.LoadLevel("3 Lobby");
    }

    public static void Load(LoadingScene nextScene)
    {
        _nextScene = nextScene;
        PhotonNetwork.LoadLevel("2 Loading");
    }
}
