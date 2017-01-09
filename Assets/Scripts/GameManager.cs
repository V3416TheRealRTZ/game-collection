using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class GameManager : Photon.PunBehaviour {

    private bool _isLeaving = false;
    private int botsCount = 0;
    private GameObject girl;
    public float addBotTime = 1;
    private float _currentTime = 0;
    public bool started = false;
    public bool finished = false;

    private List<GameObject> bots = new List<GameObject>();
    public List<GameObject> places = new List<GameObject>();

    #region Public Proporites

    public Transform startPoint;
    #endregion

    public void Start()
    {
        Debug.Log("Start game scene");
        girl = null;
        if (PhotonNetwork.room == null)
            girl = (GameObject)Instantiate(Resources.Load<GameObject>("AdvGirl"), startPoint.position, startPoint.rotation);
        else
            girl = PhotonNetwork.Instantiate("AdvGirl", startPoint.position, startPoint.rotation, 0);
        GameObject.Find("Camera").GetComponent<CameraScript>().setTarget(girl.transform);


        if (PhotonNetwork.room == null)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject bot = (GameObject)Instantiate(Resources.Load<GameObject>("AdvGirl"), startPoint.position, startPoint.rotation);
                bot.GetComponent<PlayerController>().isBot = true;
                bot.GetComponent<PlayerController>().go();
            }
            girl.GetComponent<PlayerController>().go();
        }
    }



    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !_isLeaving)
        {
            _isLeaving = true;
            LeaveRoom();
        }

        //var playing = girl.GetComponent<PlayerController>().runnerPlaying;
        if (!started && PhotonNetwork.isMasterClient && PhotonNetwork.room != null)
        {
            if (PhotonNetwork.room.playerCount + botsCount >= PhotonNetwork.room.maxPlayers)
            {
                girl.GetComponent<PlayerController>().go();
                foreach (var bot in bots)
                {
                    bot.GetComponent<PlayerController>().go();
                }
                girl.GetComponent<PlayerController>().photonView.RPC("changeToStarted", PhotonTargets.Others, null);
                started = true;
            }
            else if (_currentTime >= addBotTime)
            {
                _currentTime -= addBotTime;
                GameObject bot = PhotonNetwork.InstantiateSceneObject("AdvGirl", startPoint.position, startPoint.rotation, 0, null);
                bot.GetComponent<PlayerController>().isBot = true;
                bots.Add(bot);
                botsCount += 1;
            }
            else
                _currentTime += Time.deltaTime;
        }

        if (places.Count >= 3 && !finished && PhotonNetwork.isMasterClient)
        {
            girl.GetComponent<PlayerController>().photonView.RPC("gameFinished", PhotonTargets.All, null);
            

            /*foreach (var bot in bots)
            {
                bot.GetComponent<PlayerController>().stop();
            }
            girl.GetComponent<PlayerController>().stop();
            */

            foreach (var obj in FindObjectsOfType<PlayerController>())
                if (!places.Contains(obj.gameObject))
                {
                    places.Add(obj.gameObject);
                    break;
                }

            int[] pl = new int[4];
            for (int i =0; i<4; i++)
            {
                PlayerController pc = places[i].GetComponent<PlayerController>();
                pl[i] = pc.PlayfabScore;
                if (pc.isBot || pl[i] == 0)
                    pl[i] = 1200;
            }
            foreach (var obj in places)
                Debug.Log(obj.GetComponent<PlayerController>().PlayerUiPrefab.GetComponent<PlayerUI>().PlayerNameText.GetComponent<Text>().text);
            foreach (var obj in pl)
               Debug.Log(obj);

            pl = ELO.new_scores(pl);

            Debug.Log("New Scores:");
            foreach (var obj in pl)
                Debug.Log(obj);

            for (int i = 0; i < 4; i++)
            {
                PlayerController pc = places[i].GetComponent<PlayerController>();
                if (!pc.isBot && pc.photonView.isMine) {
                    var d = new Dictionary<string, string>();
                    d.Add("Score", pl[i].ToString());
                    UpdateUserDataRequest req = new UpdateUserDataRequest()
                    {
                        Data = d,
                        Permission = UserDataPermission.Public
                    };
                    PlayFabClientAPI.UpdateUserPublisherData(req, (UpdateUserDataResult) => { Debug.Log("Good"); }, (PlayFabError err) => { Debug.Log(err.ErrorMessage); });
                }
            }

        }


    }

    #region Photon Messages

    public void OnLeftRoom()
    {
        Debug.Log("Leave room");
    }

    public void OnConnectedToMaster()
    {
        Loading.Load(LoadingScene.Lobby);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    
    #endregion
}
