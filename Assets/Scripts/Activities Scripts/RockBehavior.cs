using UnityEngine;
using System.Collections;

public class RockBehavior : MonoBehaviour
{
    public LayerMask objects;
    private bool collided;
    public Transform objectCheck;
    private float groundRadius = 0.2f;
	void Update ()
    {
        //TODO придумать, как сделать так, чтоб не реагировал на локального игрока
        collided = Physics2D.OverlapCircle(objectCheck.position, groundRadius, objects);
        if (collided)
        {
            Destroy(gameObject);
            //Debug.Assert(collided);
        }
	}
}
