using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatisticTableEntry : MonoBehaviour {

    int place = 0;
    public string playerName = "";
    public int gold = -1;
    public int score = 0;
    int oldScore = 1200;
    int progress = 0;

    public Text placeT;
    public Text usernameT;
    public Text goldT;
    public Text scoreT;
    public Text scoreChangeT;
    // Use this for initialization

    public void updateInfo()
    {
        if (place == 0)
        {
            placeT.text = "Wait...";
            goldT.text = "-";
            scoreT.text = "-";
            scoreChangeT.text = "-";
        }
        else
        {
            placeT.text = place.ToString();
            goldT.text = gold.ToString();
            if (score != 0)
            {
                scoreT.text = score.ToString();
                int scoreChange = score - oldScore;
                scoreChangeT.text = scoreChange.ToString();
                scoreChangeT.text = scoreChange < 0 ? scoreChangeT.text+ " ↓" :scoreChangeT.text + " ↑";
                scoreChangeT.color = scoreChange < 0 ? Color.red : Color.green;
            }
        }
        usernameT.text = playerName;
    }

    void Start () {
        updateInfo();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void addUser(string name, int _oldScore)
    {
        playerName = name;
        oldScore = _oldScore;
    }

    public void setResult(int _place, int _gold)
    {
        place = _place;
        gold = _gold;
    }
}
