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

    private List<Boost> _boosts;
    
    void Start()
    {
        _boosts = new List<Boost>();
    }

	void Update ()
	{
	    var toRemove = _boosts.FindAll((boost) => boost.RemainingTime <= 0);
	    if (toRemove.Count != 0)
	    {
	        foreach (var boost in toRemove)
	        {
	            gameObject.SendMessage(boost.SendCommand, boost.BoostingCoeff);
	        }
	        _boosts.RemoveAll((boost) => boost.RemainingTime <= 0);
	    }
	    for (int i = 0; i < _boosts.Count; ++i)
	    {
	        _boosts[i].RemainingTime -= Time.deltaTime;
	    }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<PolygonCollider2D>().tag == "SpeedBooster")
        {
            Destroy(col.gameObject);
            float coefficient = col.GetComponent<BoostProperties>().boostCoefficient;
            float timeOfAction = col.GetComponent<BoostProperties>().time;
            gameObject.SendMessage("BoostSpeed", coefficient);
            _boosts.Add(new Boost("UnboostSpeed", coefficient, timeOfAction));
        }
        if (col.GetComponent<PolygonCollider2D>().tag == "JumpBooster")
        {
            Destroy(col.gameObject);
            float coefficient = col.GetComponent<BoostProperties>().boostCoefficient;
            float timeOfAction = col.GetComponent<BoostProperties>().time;
            gameObject.SendMessage("BoostJump", coefficient);
            _boosts.Add(new Boost("UnboostJump", coefficient, timeOfAction));
        }
        if (col.GetComponent<PolygonCollider2D>().tag == "MoneyBonus")
        {
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
