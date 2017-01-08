using UnityEngine;
using System.Collections;
using PlayFab.ClientModels;
using System.Collections.Generic;
using PlayFab;
using System;
using UnityEngine.UI;

public class PlayerController : Photon.MonoBehaviour
{
    public float jumpStrenght = 7000f;
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
    public string PlayfabId;
    public int PlayfabScore;

    public bool runnerStarted = false;

    void Awake()
    {
        stop();
    }

    void Start()
    {

        if (PhotonNetwork.room != null && photonView.isMine && !isBot)
        {
            PlayfabId = PlayerPrefs.GetString("PlayFabId");
            GetUserDataRequest req = new GetUserDataRequest()
            {
                Keys = new List<string> { "Score" },
                PlayFabId = PlayfabId
            };
            PlayFabClientAPI.GetUserPublisherData(req, (GetUserDataResult res) =>
            {
                UserDataRecord data;
                if (res.Data.TryGetValue("Score", out data))
                {
                    Debug.Log("Score = " + data.Value);
                    PlayfabScore = Convert.ToInt32(data.Value);
                }
            },
            (PlayFabError err) => Debug.Log(err.ErrorMessage));
        }
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
            gameObject.layer = LayerMask.NameToLayer("another_player");

        if (PlayerUiPrefab != null)
        {
            PlayerUiPrefab = Instantiate(PlayerUiPrefab) as GameObject;
            PlayerUiPrefab.GetComponent<PlayerUI>().setTarget(this);
        }
        else
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
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
            gameObject.GetComponent<PlayerActivities>().ThrowRock();

        if (runnerStarted)
        {
            go();
            runnerStarted = false;
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

    public void stop()
    {
        Debug.Log("Called stop by" + PlayerUiPrefab.GetComponent<PlayerUI>().PlayerNameText.GetComponent<Text>().text);
        realSpeed = 0;
        speed = 0;
    }

    public void go()
    {
        Debug.Log("Called go by" + PlayerUiPrefab.GetComponent<PlayerUI>().PlayerNameText.GetComponent<Text>().text);
        realSpeed = initSpeed;
        speed = initSpeed;
    }

    [PunRPC]
    public void changeToStarted()
    {
        Debug.Log("chageToStarted " + runnerStarted.ToString());
        foreach(var obj in FindObjectsOfType<PlayerController>())
        {
            obj.runnerStarted = true;
        }
        Debug.Log("runnerStarted = " + runnerStarted.ToString());
    }


}