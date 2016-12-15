using UnityEngine;
using System.Collections;

public enum LoadingScene
{
    Lobby,
    Game,
    Register
}

public class Loading : MonoBehaviour {

    private static LoadingScene _nextScene { get; set; }

    void Start()
    {
        if (_nextScene == LoadingScene.Lobby)
        {
            PhotonNetwork.networkingPeer.OpJoinLobby(TypedLobby.Default);
        }
        else if (_nextScene == LoadingScene.Game)
            PhotonNetwork.LoadLevel("4 Game");
        else if (_nextScene == LoadingScene.Register)
            PhotonNetwork.LoadLevel("5 Register");
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
