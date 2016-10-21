using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float jumpStrenght = 700f;
    private Animator anim;
    public float speed = 10f;
    private Rigidbody2D rig;
    private bool facedRight = true;
    public LayerMask whatIsGround;
    public float groundRadius = 0.2f;
    public bool grounded = false;
    public Transform groundCheck;
	// Use this for initialization
	void Start ()
	{
	    anim = GetComponent<Animator>();
	    rig = GetComponent<Rigidbody2D>();
	    rig.inertia = 100f;
	}
	// Update is called once per frame
	void FixedUpdate ()
	{
        if (!isLocalPlayer)
            return;

	    float move = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(move));
        rig.velocity = new Vector2(move * speed, rig.velocity.y);
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("Grounded", grounded);
        if (move < 0 && facedRight)
            Flip();
        else if (move > 0 && !facedRight)
            Flip();
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
	    {
	        rig.AddForce(new Vector2(0, jumpStrenght));
	    }
	}

    void BoostSpeed(float boost)
    {
        speed *= boost;
    }

    void UnboostSpeed(float boost)
    {
        speed /= boost;
    }

    void BoostJump(float boost)
    {
        jumpStrenght *= boost;
    }

    void UnboostJump(float boost)
    {
        jumpStrenght /= boost;
    }

    void Flip()
    {
        facedRight = !facedRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "VietnamStar")
        {
            Vector2 vect = transform.position - collision.transform.position;
            float xBoost = collision.gameObject.GetComponent<VietnamStarScript>().xBoost;
            float yBoost = collision.gameObject.GetComponent<VietnamStarScript>().yBoost;
            //rig.AddForce(new Vector2(vect.x * xBoost, vect.y * yBoost), ForceMode2D.Impulse);
            vect.x = vect.x * xBoost;
            vect.y = (vect.y + 0.125f) * yBoost;
            rig.AddRelativeForce(vect);
            //rig.velocity = new Vector2(vect.x * xBoost, vect.y * yBoost);
        }
    }
}