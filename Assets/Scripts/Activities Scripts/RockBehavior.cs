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
        collided = Physics2D.OverlapCircle(objectCheck.position, groundRadius, objects);
        if (collided)
        {
            Debug.Log("before rock destroy");
            if (PhotonNetwork.room == null)
                Destroy(gameObject);
            else if (photonView.isMine)
                PhotonNetwork.Destroy(gameObject);
        }
	}
}
