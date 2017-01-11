using UnityEngine;
using System.Collections;
using PlayFab.ClientModels;
using System.Collections.Generic;
using PlayFab;
//using System;
using UnityEngine.UI;

public class PlayerController : Photon.PunBehaviour
{

    float initJumpStrenght = 1000f;
	public float jumpStrenght; 
    Animator anim;
    public float maxSpeed = 80f;
    public float minSpeed = 0f;
    public float realSpeed;
    public float speed;
    float initSpeed = 50f;
    Rigidbody2D rig;
    bool facedRight = true;
    public LayerMask whatIsGround;
    public LayerMask groundAndBonuses;
    public bool isBot;
    public float groundRadius = 0.2f;
    public float bonusRadius = 2f;
    bool grounded = false;
    public Transform groundCheck;
    public Transform bonusCheck;
    public GameObject PlayerUiPrefab;
    //public string PlayfabId;
    public int PlayfabScore;
    public string PlayerName;
    public bool stoped = true;

    void Awake()
    {
        stop();

    }

    void Start()
    {

        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        rig.inertia = 100f;
        if (!photonView.isMine && PhotonNetwork.room != null)
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            GetComponent<CircleCollider2D>().isTrigger = true;
            GetComponent<Rigidbody2D>().isKinematic = true;
            gameObject.layer = LayerMask.NameToLayer("another_player");
        }

        if (isBot)
        {
            gameObject.layer = LayerMask.NameToLayer("another_player");
            PlayerName = "Player" + ((int)Random.Range(0.0f, 1000.0f)).ToString();
        }
        else
        {
            PlayerName = PlayerInfo.DisplayName;
            PlayfabScore = PlayerInfo.Score;
            FindObjectOfType<GameManager>().finishPopup.photonView.RPC("addNewPlayer", PhotonTargets.AllBuffered, PlayerInfo.DisplayName, PlayerInfo.Score);
        }

        if (PlayerUiPrefab != null)
        {
            PlayerUiPrefab = Instantiate(PlayerUiPrefab) as GameObject;
            PlayerUiPrefab.GetComponent<PlayerUI>().setTarget(this);
            PlayerUiPrefab.GetComponent<PlayerUI>().setName(PlayerName);
            //PlayerName = PlayerUiPrefab.GetComponent<PlayerUI>().PlayerNameText.GetComponent<Text>().text;
        }
        else
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);

        if (photonView == null || (isBot && PhotonNetwork.isMasterClient))
            FindObjectOfType<GameManager>().finishPopup.photonView.RPC("addNewPlayer", PhotonTargets.All, PlayerName, 1200);

        /*if (PhotonNetwork.room != null && photonView.isMine && !isBot)
        {
            PlayfabScore = PlayerInfo.Score;
            FindObjectOfType<GameManager>().finishPopup.photonView.RPC("addNewPlayer", PhotonTargets.AllBuffered, PlayerInfo.DisplayName, PlayerInfo.Score);
        }*/
    }

	void FixedUpdate()
	{

        if (PhotonNetwork.room != null)
            if (!photonView.isMine || (!PhotonNetwork.isMasterClient && isBot))
                return;


        anim.SetFloat("Speed", Mathf.Abs(speed));
        rig.velocity = new Vector2(speed, rig.velocity.y);
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        
        anim.SetBool("Grounded", grounded);
        bool bonus = Physics2D.OverlapCircle(bonusCheck.position, bonusRadius, groundAndBonuses);
        if ((Input.GetKeyDown(KeyCode.Space) && grounded && !isBot) || (PhotonNetwork.isMasterClient && isBot && bonus && grounded))
            rig.AddForce(new Vector2(0, jumpStrenght));

        if (Input.GetKeyDown(KeyCode.Alpha1) && !isBot)
        /*{
            Debug.Log("before throw rock in pc");
            photonView.RPC("ThrowRock", PhotonTargets.MasterClient);
        }*/
            gameObject.GetComponent<PlayerActivities>().ThrowRock();
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

    [PunRPC]
    public void stop()
    {
        realSpeed = 0;
        speed = 0;
        jumpStrenght = 0;
        stoped = true;
    }

    [PunRPC]
    public void go()
    {
        Debug.Log("start in go");
        realSpeed = initSpeed;
        speed = initSpeed;
        jumpStrenght = initJumpStrenght;
        stoped = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	    if (stream.isWriting)
	    {
	        stream.Serialize(ref PlayfabScore);
	        stream.Serialize(ref PlayerName);
        }
        else
	    {
	        stream.Serialize(ref PlayfabScore);
	        stream.Serialize(ref PlayerName);
            PlayerUiPrefab.GetComponent<PlayerUI>().PlayerNameText.GetComponent<Text>().text = PlayerName;
        }
    }

    public override void OnLeftRoom()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        Debug.Log("Leave room");
        if (!gm.finished)
        {
            Debug.Log("Runner not finished");
            gm.finishPopup.photonView.RPC("leavePlayer", PhotonTargets.Others, PlayerName);
        }
    }

}
