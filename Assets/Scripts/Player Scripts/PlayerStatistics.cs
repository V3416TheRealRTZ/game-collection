using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStatistics : MonoBehaviour {
    public int Scores;
    public int Rocks;
    public bool IsImmortaled;
    public bool IsMagnetActive;
    public GUIContent canvas;
    private void Update()
    {
        PlayerController pc = gameObject.GetComponent<PlayerController>();
        if ((!pc.photonView.isMine && PhotonNetwork.room != null)  || pc.isBot) 
            return;
        var jump = pc.jumpStrenght;
        var speed = pc.realSpeed;
        GameObject.Find("Canvas/Image/Text").GetComponent<Text>().text = 
            string.Format("Очков: {0}\nКамней: {1}\nБессмертие: {2}\nМагнит: {3}\nСкорость {4}\nСила прыжка {5}"
            , Scores, Rocks, IsImmortaled, IsMagnetActive, speed, jump);
    }

    private void Start()
    {
        IsImmortaled = false;
        Scores = 0;
        Rocks = 0;
    }
}
