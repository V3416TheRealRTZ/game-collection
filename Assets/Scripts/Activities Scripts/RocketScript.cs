using UnityEngine;
using System.Collections;

public class RocketScript : MonoBehaviour
{
    public LayerMask mask;
    public Transform playerCheck;
    public bool playerOnRocket = false;
    private Rigidbody2D rig;
    public float time;
    public float Velocity;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

	// Update is called once per frame
    void FixedUpdate()
    {
        playerOnRocket = Physics2D.OverlapCircle(playerCheck.position, 0.2f, mask);
        if (playerOnRocket)
        {
            var player = Physics2D.OverlapCircle(playerCheck.position, 0.2f, mask);
            rig.velocity = new Vector2(Velocity, 0);
            rig.GetComponent<FixedJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
            time -= Time.deltaTime;
        }
        if (time <= 0)
        {
            Destroy(gameObject);
        }

    }
}


