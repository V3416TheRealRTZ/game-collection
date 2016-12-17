using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStatistics : MonoBehaviour {
    public int Scores { get; set; }
    public int Rocks { get; set; }
    public bool IsImmortaled { get; set; }
    public bool IsMagnetActive { get; set; }
    public GUIContent canvas;
    private void Update()
    {
        var jump = gameObject.GetComponent<PlayerController>().jumpStrenght;
        var speed = gameObject.GetComponent<PlayerController>().realSpeed;
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
