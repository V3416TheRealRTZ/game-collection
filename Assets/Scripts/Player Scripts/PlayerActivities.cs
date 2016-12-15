using UnityEngine;
using System;
using System.Collections;

public class PlayerActivities : MonoBehaviour {
    void ThrowRock()
    {
        int signX = Math.Sign(transform.localScale.x);
        GameObject rock = null;
        if (gameObject.GetComponent<PlayerStatistics>().Rocks > 0)
        {
            --gameObject.GetComponent<PlayerStatistics>().Rocks;
            if (PhotonNetwork.room == null)
                rock = (GameObject)Instantiate(Resources.Load<GameObject>("PropellingRock"), transform.position, transform.rotation);
            else
                rock = PhotonNetwork.InstantiateSceneObject("PropellingRock", transform.position, transform.rotation, 0, null);
        }
        else
            return;
        
        rock.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(700 * signX, 700));
    }
}
