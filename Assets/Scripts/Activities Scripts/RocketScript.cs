using UnityEngine;
using System.Collections;

public class RocketScript : MonoBehaviour
{
    public LayerMask mask;
    public Transform playerCheck;
    private Collider2D playerOnRocket;
    private Rigidbody2D rig;
    public float time;
    public float Velocity;
    public float playerRadius = 2.0f;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

	// Update is called once per frame
    void FixedUpdate()
    {
        playerOnRocket = Physics2D.OverlapCircle(playerCheck.position, playerRadius, mask);
        if (playerOnRocket)
        {
            rig.velocity = new Vector2(Velocity, 0);
            rig.GetComponent<FixedJoint2D>().connectedBody = playerOnRocket.GetComponent<Rigidbody2D>();
            time -= Time.deltaTime;
        }
        if (time <= 0)
        {
            Destroy(gameObject);
        }

    }
}


