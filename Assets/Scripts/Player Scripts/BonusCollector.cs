using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class BonusCollector : Photon.PunBehaviour
{
    private bool finished = false;
    private class Boost
    {
        public string SendCommand { get; private set; }
        public float BoostingCoeff { get; private set; }
        public float RemainingTime { get; set; }
       

        public Boost(string command, float coefficient, float time)
        {
            SendCommand = command;
            BoostingCoeff = coefficient;
            RemainingTime = time;
        }
    }
    private float _rockTime;
    private float _magnetTime;
    private float _moneyBonusTime;
    private float _moneyCoeff;
    private float _shieldTime;
    private List<Boost> _boosts;
    private PlayerStatistics _stats;
    private PlayerActivities _activs;
    private GameManager _gameManager;
    private PlayerController pc;

    void Start()
    {
        _boosts = new List<Boost>();
        _moneyBonusTime = 0;
        _rockTime = 0;
        _shieldTime = 0;
        _stats = GetComponent<PlayerStatistics>();
        _activs = GetComponent<PlayerActivities>();
        _gameManager = FindObjectOfType<GameManager>();
        pc = gameObject.GetComponent<PlayerController>();
    }

	void FixedUpdate()
	{
        
        if (!pc.photonView.isMine || (pc.isBot && !PhotonNetwork.isMasterClient))
            return;
        var toRemove = _boosts.FindAll((boost) => boost.RemainingTime <= 0);
	    if (toRemove.Count != 0)
	    {
	        foreach (var boost in toRemove)
	        {
	            SendMessage(boost.SendCommand, boost.BoostingCoeff);
	        }
	        _boosts.RemoveAll((boost) => boost.RemainingTime <= 0);
	    }
	    for (int i = 0; i < _boosts.Count; ++i)
	    {
	        _boosts[i].RemainingTime -= Time.deltaTime;
	    }
        if (_moneyBonusTime > 0)
            _moneyBonusTime -= Time.deltaTime;
        else
            _moneyCoeff = 1;
        if (_rockTime > 0)
            _rockTime -= Time.deltaTime;
        else
            _stats.Rocks = 0;
        if (_shieldTime > 0)
            _shieldTime -= Time.deltaTime;
        else
            _stats.IsImmortaled = false;
        if (_magnetTime > 0)
        {
            _magnetTime -= Time.deltaTime;
        }
        else
            _stats.IsMagnetActive = false;
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PropRock" && gameObject.layer == LayerMask.NameToLayer("another_player"))
        {
            float coefficient = col.GetComponent<BoostProperties>().boostCoefficient;
            float timeOfAction = col.GetComponent<BoostProperties>().time;
            SendMessage("BoostSpeed", coefficient);
            _boosts.Add(new Boost("UnboostSpeed", coefficient, timeOfAction));
            Destroy(col.gameObject);
        }

        if (col.tag == "SpeedBooster")
        {
            float coefficient = col.GetComponent<BoostProperties>().boostCoefficient + float.Parse(PlayerInfo.TitleUserData["speed"]) / 100f;
            float timeOfAction = col.GetComponent<BoostProperties>().time;
            SendMessage("BoostSpeed", coefficient);
            _boosts.Add(new Boost("UnboostSpeed", coefficient, timeOfAction));
            Destroy(col.gameObject);
        }

        if (col.tag == "JumpBooster")
        {
            float coefficient = col.GetComponent<BoostProperties>().boostCoefficient + float.Parse(PlayerInfo.TitleUserData["jump"]) / 100f;
            float timeOfAction = col.GetComponent<BoostProperties>().time;
            SendMessage("BoostJump", coefficient);
            _boosts.Add(new Boost("UnboostJump", coefficient, timeOfAction));
            Destroy(col.gameObject);
        }
        if (col.tag == "MoneyBonus")
        {
            _moneyCoeff = col.GetComponent<BoostProperties>().boostCoefficient;
            _moneyBonusTime = col.GetComponent<BoostProperties>().time + float.Parse(PlayerInfo.TitleUserData["moneybonus"]);
            Destroy(col.gameObject);
        }
        
        if (col.tag == "Coin")
        {
            _stats.Scores += (int)_moneyCoeff;
            Destroy(col.gameObject);
        }

        if (col.tag == "Rock")
        {
            _rockTime = col.GetComponent<BoostProperties>().time;
            _stats.Rocks = (int)col.GetComponent<BoostProperties>().boostCoefficient;
            Destroy(col.gameObject);
        }

        if (col.tag == "Shield")
        {
            _shieldTime = col.GetComponent<BoostProperties>().time + float.Parse(PlayerInfo.TitleUserData["shield"]);
            _stats.IsImmortaled = true;
            Destroy(col.gameObject);
        }

        if (col.tag == "Magnet")
        {
            _magnetTime = col.GetComponent<BoostProperties>().time;
            _stats.IsMagnetActive = true;
            Destroy(col.gameObject);
        }

        if (col.tag == "Finish")
        {
            pc.stop();
            if (!finished)
            {
                Debug.Log("finished " + pc.PlayerName);
                _gameManager.places.Add(gameObject);
                Debug.Log("places count = " + _gameManager.places.Count.ToString());
                finished = true;
                int place = _gameManager.places.Count > 4 ? 4 : _gameManager.places.Count;
                _gameManager.finishPopup.photonView.RPC("setResult", PhotonTargets.All, place, gameObject.GetComponent<PlayerStatistics>().Scores, pc.PlayerName);
                if (!pc.isBot && pc.photonView.isMine)
                    _gameManager.finishPopup.switchOn();
            }
            
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "DeceleratingBarrier") && !_stats.IsImmortaled)
        {
            if (gameObject.GetComponent<PlayerController>().realSpeed <= collision.gameObject.GetComponent<BoostProperties>().boostCoefficient)
                return;
            gameObject.SendMessage("BoostSpeed", 0.5);
            _boosts.Add(new Boost("UnboostSpeed", 0.5f, 5.0f));
        }
    }
}
