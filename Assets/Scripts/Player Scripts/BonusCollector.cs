using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BonusCollector : MonoBehaviour
{
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
    private float _moneyBonusTime;
    private float _moneyCoeff;
    private List<Boost> _boosts;
    private PlayerStatistics _stats;
    private PlayerActivities _activs;
    
    void Start()
    {
        _boosts = new List<Boost>();
        _moneyBonusTime = 0;
        _rockTime = 0;
        _stats = GetComponent<PlayerStatistics>();
        _activs = GetComponent<PlayerActivities>();
    }

	void FixedUpdate()
	{
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
            //SendMessage("SetRocks", rocks);
            _stats.Rocks = 0;
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Collider2D>().tag == "SpeedBooster")
        {
            float coefficient = col.GetComponent<BoostProperties>().boostCoefficient;
            float timeOfAction = col.GetComponent<BoostProperties>().time;
            SendMessage("BoostSpeed", coefficient);
            _boosts.Add(new Boost("UnboostSpeed", coefficient, timeOfAction));
            Destroy(col.gameObject);
        }
        if (col.GetComponent<Collider2D>().tag == "JumpBooster")
        {
            
            float coefficient = col.GetComponent<BoostProperties>().boostCoefficient;
            float timeOfAction = col.GetComponent<BoostProperties>().time;
            SendMessage("BoostJump", coefficient);
            _boosts.Add(new Boost("UnboostJump", coefficient, timeOfAction));
            Destroy(col.gameObject);
        }
        if (col.GetComponent<Collider2D>().tag == "MoneyBonus")
        {
            _moneyCoeff = col.GetComponent<BoostProperties>().boostCoefficient;
            _moneyBonusTime = col.GetComponent<BoostProperties>().time;
            Destroy(col.gameObject);
        }
        
        if (col.GetComponent<Collider2D>().tag == "Coin")
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "DeceleratingBarrier")
        {
            gameObject.SendMessage("BoostSpeed", 0.5);
            _boosts.Add(new Boost("UnboostSpeed", 0.5f, 5.0f));
        }
    }
}
