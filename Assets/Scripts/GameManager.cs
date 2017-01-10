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


    public FinishPopup finishPopup;
    #region Public Proporites

    public Transform startPoint;
    #endregion

    public void Start()
    {
        finishPopup.switchOff();
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
                /*girl.GetComponent<PlayerController>().go();
                foreach (var bot in bots)
                {
                    bot.GetComponent<PlayerController>().go();
                }*/
                //girl.GetComponent<PlayerController>().photonView.RPC("changeToStarted", PhotonTargets.Others, null);
                started = true;
                Debug.Log("before go");
                foreach (var obj in FindObjectsOfType<PlayerController>())
                    if (obj.isBot)
                        obj.go();
                    else
                        obj.photonView.RPC("go", PhotonTargets.All);
                PhotonNetwork.room.open = false;
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
            finished = true;

            Debug.Log("before stop");

            foreach (var obj in FindObjectsOfType<PlayerController>())
            {
                obj.photonView.RPC("stop", PhotonTargets.All);
                if (!places.Contains(obj.gameObject))
                {
                    places.Add(obj.gameObject);
                    Debug.Log("before set results");

                    finishPopup.photonView.RPC("setResult", PhotonTargets.All, 4, obj.gameObject.GetComponent<PlayerStatistics>().Scores, obj.PlayerName);
                }
            }

                int[] pl = new int[4];
                for (int i = 0; i < 4; i++)
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
                    if (!pc.isBot && pc.photonView.isMine)
                    {
                        UpdatePlayerStatisticsRequest req = new UpdatePlayerStatisticsRequest()
                        {
                            Statistics = new List<StatisticUpdate> { new StatisticUpdate() { StatisticName = "Score", Value = pl[i] } }
                        };
                        PlayFabClientAPI.UpdatePlayerStatistics(req, (UpdatePlayerStatisticsResult) => { Debug.Log("Good"); }, (PlayFabError err) => { Debug.Log(err.ErrorMessage); });
                    }
                Debug.Log("before set new score");

                finishPopup.photonView.RPC("setNewScore", PhotonTargets.All, pl[i], pc.PlayerName);
                    
                    //finishPopup.setNewScore(pl[i], pc.PlayerName);
                }
            Debug.Log("before activate actions");

            finishPopup.photonView.RPC("activateActions", PhotonTargets.All);


        }


    }

    #region Photon Messages

    

    public void OnConnectedToMaster()
    {
        Loading.Load(LoadingScene.Lobby);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Restart()
    {
        Loading.Load(LoadingScene.Game);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.Serialize(ref finished);
            stream.Serialize(ref started);
        }
        else
        {
            stream.Serialize(ref finished);
            stream.Serialize(ref started);
        }
    }

    #endregion
}
