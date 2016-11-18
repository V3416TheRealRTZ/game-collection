using UnityEngine;
using System.Collections;

public class PlayerController : Photon.PunBehaviour
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

    void Start ()
	{
        anim = GetComponent<Animator>();
	    rig = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void FixedUpdate ()
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
}