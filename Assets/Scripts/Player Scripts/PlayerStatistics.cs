using UnityEngine;
using System.Collections;

public class PlayerStatistics : MonoBehaviour {
    public int Scores { get; set; }
    public int Rocks { get; set; }
    public bool IsImmortaled { get; set; }
    public bool IsMagnetActive { get; set; }
    public GUIContent canvas;
    private void Update()
    {

    }

    private void Start()
    {
        IsImmortaled = false;
        Scores = 0;
        Rocks = 0;
    }
}
