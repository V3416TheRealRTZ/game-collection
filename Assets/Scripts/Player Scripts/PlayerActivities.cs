﻿using UnityEngine;
using System;
using System.Collections;

public class PlayerActivities : Photon.PunBehaviour {
    public float ThrowStrenght;
    public float time = 0.0f;
    public float delta = 0.25f;

    private void FixedUpdate()
    {
        time -= Time.deltaTime;
        time = time < 0 ? 0 : time;
    }

    public void ThrowRock()
    {
        Debug.Log("in throw rock");
        int signX = Math.Sign(transform.localScale.x);
        GameObject rock = null;
        if (gameObject.GetComponent<PlayerStatistics>().Rocks > 0 && time <= 0.0f)
        {
            --gameObject.GetComponent<PlayerStatistics>().Rocks;
            if (PhotonNetwork.room == null)
                rock = (GameObject)Instantiate(Resources.Load<GameObject>("PropellingRock"), transform.position, transform.rotation);
            else
                rock = PhotonNetwork.Instantiate("PropellingRock", transform.position, transform.rotation, 0);
            time += delta;
        }
        else
            return;

        /*foreach (var r in FindObjectsOfType<RockBehavior>())
        {
            r.gameObject.layer = LayerMask.NameToLayer("rock");

            r.gameObject.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(ThrowStrenght * signX, ThrowStrenght));
        }*/
        rock.layer = LayerMask.NameToLayer("rock");

        rock.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(ThrowStrenght * signX, ThrowStrenght));
    }

    
}
