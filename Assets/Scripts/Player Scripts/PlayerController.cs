using UnityEngine;
using System.Collections;

public class PlayerController : Photon.PunBehaviour
{
    public float jumpStrenght = 700f;
    Animator anim;
    public float maxSpeed = 30f;
    public float minSpeed = 1.5f;
    float realSpeed;
    public float speed = 10f;
    Rigidbody2D rig;
    bool facedRight = true;
    public LayerMask whatIsGround;
    public float groundRadius = 0.2f;
    bool grounded = false;
    public Transform groundCheck;


    void Start()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        rig.inertia = 100f;
        realSpeed = speed;
        if (!photonView.isMine)
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            GetComponent<CircleCollider2D>().isTrigger = true;
            GetComponent<Rigidbody2D>().isKinematic = true;
        }

    }

	void FixedUpdate()
	{
        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SendMessage("ThrowRock");
        }

	}

    void BoostSpeed(float boost)
    {
        realSpeed *= boost;
        if (realSpeed > maxSpeed)
        {
            speed = maxSpeed;
            return;
        }
        if (realSpeed < minSpeed)
        {
            speed = minSpeed;
            return;
        }
        speed = realSpeed;

    }

    void UnboostSpeed(float boost)
    {
        realSpeed /= boost;
        if (realSpeed > maxSpeed)
        {
            speed = maxSpeed;
            return;
        }
        if (realSpeed < minSpeed)
        {
            speed = minSpeed;
            return;
        }
        speed = realSpeed;
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

    // public override void OnStartLocalPlayer()
    // {
    //     GetComponent<SpriteRenderer>().color = Color.red;
    // }

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
            //rig.AddRelativeForce(vect);
            rig.AddForceAtPosition(vect, collision.transform.position);
            //rig.velocity = new Vector2(vect.x * xBoost, vect.y * yBoost);
        }
    }
}