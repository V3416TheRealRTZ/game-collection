using UnityEngine;
using System;
using System.Collections;

public class PlayerActivities : MonoBehaviour {

    public GameObject rock;
    void ThrowRock()
    {
        int signX = Math.Sign(transform.localScale.x);
        GameObject newRock = null;
        if (gameObject.GetComponent<PlayerStatistics>().Rocks > 0)
        {
            --gameObject.GetComponent<PlayerStatistics>().Rocks;
            //newRock = (GameObject)Instantiate(rock, transform.position, transform.rotation);
            newRock = PhotonNetwork.InstantiateSceneObject("PropellingRock", transform.position, transform.rotation, 0, null);
        }
        else
            return;
        
        newRock.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(700 * signX, 700));
    }
}
