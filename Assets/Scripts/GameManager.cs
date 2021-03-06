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
    public float addBotTime = 10;
    private float _currentTime = 0;
    public bool started = false;
    public bool readyToStart = false;
    public bool finished = false;
    private float countdown = 4f;

    private List<GameObject> bots = new List<GameObject>();
    public List<GameObject> places = new List<GameObject>();

    public Text countdownText;
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

    private void runAll()
    {
        Debug.Log("before go");
        foreach (var obj in FindObjectsOfType<PlayerController>())
            if (obj.isBot)
                obj.go();
            else
                obj.photonView.RPC("go", PhotonTargets.All);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !_isLeaving)
        {
            _isLeaving = true;
            LeaveRoom();
        }

        if (readyToStart)
        {
            
            if (countdown <= 0)
            {
                countdownText.text = "RUN";
                if (!started)
                    runAll();
                started = true;
            }
            else
            {
                if (countdown <= 1)
                    countdownText.text = "1";
                else if (countdown <= 2)
                    countdownText.text = "2";
                else if (countdown <= 3)
                {
                    countdownText.text = "3";
                    countdownText.fontSize = 150;
                }
                
                
                    countdown -= Time.deltaTime;
                
            }
        }
        else
        {
            countdownText.text = "Waiting for players...\n"+(PhotonNetwork.room.playerCount + botsCount).ToString() + " of 4";
        }

        if (started)
            countdownText.gameObject.SetActive(false);

        if (!started && PhotonNetwork.isMasterClient && PhotonNetwork.room != null)
        {
            if (PhotonNetwork.room.playerCount + botsCount >= PhotonNetwork.room.maxPlayers)
            {
                readyToStart = true;
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
            finishPopup.photonView.RPC("switchOn", PhotonTargets.All);

            foreach (var obj in FindObjectsOfType<PlayerController>())
            {
                obj.photonView.RPC("stop", PhotonTargets.All);
                if (!places.Contains(obj.gameObject))
                {
                    places.Add(obj.gameObject);
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
                Debug.Log("before set new score");
                finishPopup.photonView.RPC("setNewScore", PhotonTargets.All, pl[i], pc.PlayerName);
                if (!pc.isBot && pc.photonView.isMine)
                {
                    finishPopup.setNewScore(pl[i], pc.PlayerName);
                    /*
                    UpdatePlayerStatisticsRequest req = new UpdatePlayerStatisticsRequest()
                    {
                        Statistics = new List<StatisticUpdate> { new StatisticUpdate() { StatisticName = "Score", Value = pl[i] } }
                    };
                    PlayFabClientAPI.UpdatePlayerStatistics(req, (UpdatePlayerStatisticsResult r) => { PlayerInfo.UpdateScore(); }, (PlayFabError err) => { Debug.Log(err.ErrorMessage); });

                    AddUserVirtualCurrencyRequest reqCur = new AddUserVirtualCurrencyRequest()
                    {
                        VirtualCurrency = "GO",
                        Amount = pc.gameObject.GetComponent<PlayerStatistics>().Scores,
                    };
                    PlayFabClientAPI.AddUserVirtualCurrency(reqCur, (ModifyUserVirtualCurrencyResult res) =>
                    {
                        PlayerInfo.Money = res.Balance;
                        Debug.Log("Money updated for " + res.BalanceChange.ToString() + ". Now money = " + res.Balance.ToString());
                    },
                    (PlayFabError err) => Debug.Log(err.ErrorMessage));*/     
                }
            }
            finishPopup.photonView.RPC("UpdatePlayerStatistics", PhotonTargets.All);
            Debug.Log("before activate actions");

            finishPopup.photonView.RPC("activateActions", PhotonTargets.All);


        }


    }

    #region Photon Messages

    

    public override void OnConnectedToMaster()
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
            stream.Serialize(ref readyToStart);
        }
        else
        {
            stream.Serialize(ref finished);
            stream.Serialize(ref started);
            stream.Serialize(ref readyToStart);

        }
    }

    #endregion
}
