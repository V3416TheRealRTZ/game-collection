using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;

public class FinishPopup : Photon.PunBehaviour {

    public GameObject actions;
    public GameObject finishPopup; 

    public StatisticTableEntry[] fields = new StatisticTableEntry[4];
    private int players = 0;
    private int leavedPlayers = 0;
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    [PunRPC]
    public void switchOn()
    {
        Debug.Log("Table switched on");
        var pos = finishPopup.transform.position;
        finishPopup.transform.position = new Vector3(pos.x, pos.y, 0);
    }

    public void switchOff()
    {
        Debug.Log("Table switched off");
        var pos = finishPopup.transform.position;
        finishPopup.transform.position = new Vector3(pos.x, pos.y, 10000f);
    }

    [PunRPC]
    public void addNewPlayer(string name, int oldScore)
    {
        if (players + leavedPlayers >= 4)
            return;

        fields[players].addUser(name, oldScore);
        updateTable();
        players++;
    }

    [PunRPC]
    public void leavePlayer(string name)
    {
        if (players + leavedPlayers >= 4)
            return;
        setResult(4-leavedPlayers, 0, name);
        updateTable();
        leavedPlayers++;
    }

    void updateTable()
    {
        foreach(var field in fields)
        {
            field.updateInfo();
        }
    }

    [PunRPC]
    public void setResult(int place, int gold, string name)
    {
        for(int i=0; i < fields.Length; i++)
        {
            if (fields[i].playerName == name)
            {
                fields[i].setResult(place, gold);
                int index = fields[i].transform.GetSiblingIndex();
                if (place != index)
                    fields[i].transform.SetSiblingIndex(place);
                updateTable();
                return;
            }
        }

    }

    [PunRPC]
    public void setNewScore(int _score, string name)
    {
        for (int i = 0; i < fields.Length; i++)
            if (fields[i].playerName == name)
            {
                fields[i].score = _score;
                updateTable();
                break;
            }
    }

    [PunRPC]
    public void activateActions()
    {
        Debug.Log("activateActions");
        actions.SetActive(true);
    }

    }
