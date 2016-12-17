using UnityEngine;
using System.Collections;

public class RockBehavior : Photon.PunBehaviour
{
    public LayerMask objects;
    private bool collided;
    public Transform objectCheck;
    private float groundRadius = 0.2f;

	void Update()
    {
        //TODO придумать, как сделать так, чтоб не реагировал на локального игрока
        collided = Physics2D.OverlapCircle(objectCheck.position, groundRadius, objects);
        if (collided)
        {
            if (PhotonNetwork.room == null)
                Destroy(gameObject);
            else if (PhotonNetwork.isMasterClient)
                PhotonNetwork.Destroy(gameObject);
            //Destroy(gameObject);
            //Debug.Assert(collided);
        }
	}
}
