using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour
{
    public Transform coinsGrab;
    bool isPlayer;
    float radius = 2.0f;
    public LayerMask whoIsPlayer;
	
    // Update is called once per frame
	void FixedUpdate ()
    {
        var obj = Physics2D.OverlapCircle(coinsGrab.position, radius, whoIsPlayer);
        isPlayer = obj;
        if (isPlayer)
        {
            if (obj.GetComponent<PlayerStatistics>().IsMagnetActive)
            {
                gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
                var playerXPos = obj.gameObject.transform.position.x;
                var playerYPos = obj.gameObject.transform.position.y;
                //TODO придумать, как плавно передвинуть монету к игроку.
                gameObject.GetComponent<Rigidbody2D>().MovePosition(new Vector2(playerXPos, playerYPos));
            }
        }
    }
}
