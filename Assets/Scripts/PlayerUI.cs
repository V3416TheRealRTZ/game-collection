using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [Tooltip("UI Text to display Player's Name")]
    public Text PlayerNameText;
    PlayerController _target;
    float _playerHeight = 0f;

    void Awake()
    {
        GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());       
    }

    public void setName(string name)
    {
        PlayerNameText.text = name;
        /*if (_target.isBot)
            PlayerNameText.text = "Player" + (int)Random.Range(0.0f, 1000.0f);
        else
            PlayerNameText.text = PlayerInfo.DisplayName;
        else if (PlayerPrefs.HasKey("DisplayName") && PlayerPrefs.GetString("DisplayName") != "")
            PlayerNameText.text = PlayerPrefs.GetString("DisplayName");
        else
            PlayerNameText.text = PlayerPrefs.GetString("Username");*/
    }

    void Start () {
        //setName();
	}
	
	// Update is called once per frame
	void Update () {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }
    }

    void LateUpdate()
    {
        if (_target == null)
            return;
        PlayerNameText.transform.position = _target.transform.position;
        PlayerNameText.transform.position = new Vector3(_target.transform.position.x, _target.transform.position.y + _playerHeight);
    }

    public void setTarget(PlayerController target)
    {
        _target = target;
        _playerHeight = 10;
    }
}
