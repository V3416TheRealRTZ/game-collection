using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;
using Photon;

public class Login : Photon.MonoBehaviour {

    public string version = "0.0.1";

    public InputField _emailField;
    public InputField _passwordField;

    private Text _loginStatus;
    // Use this for initialization
    void Start () {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.ConnectUsingSettings(version);
        _loginStatus = GameObject.Find("Status").GetComponent<Text>();
        HideErrors();
    }

    private void HideErrors()
    {
        _emailField.transform.FindChild("Error").GetComponent<Text>().text = "";
        _passwordField.transform.FindChild("Error").GetComponent<Text>().text = "";
        _loginStatus.text = "";
    }

    public void LogIn()
    {
        var titleId = PlayFabSettings.TitleId;
        string email = _emailField.text;
        string password = _passwordField.text;
        LoginWithEmailAddressRequest req = new LoginWithEmailAddressRequest() { TitleId = titleId, Email = email, Password = password };
        PlayFabClientAPI.LoginWithEmailAddress(req, OnLoginCallback, OnApiCallError);
        _loginStatus.text = "Try to LogIn...";
        _loginStatus.color = Color.black;
    }

    void OnLoginCallback(LoginResult result)
    {
        Debug.Log(string.Format("Login Successful. Welcome Player: {0}!", result.PlayFabId));
        Debug.Log(string.Format("Your session ticket is: {0}", result.SessionTicket));

        Loading.Load(LoadingScene.Lobby);
    }

    void OnApiCallError(PlayFabError err)
    {
        HideErrors();
        _loginStatus.text = err.ErrorMessage;
        _loginStatus.color = Color.red;
        string http = string.Format("HTTP:{0}", err.HttpCode);
        string message = string.Format("ERROR:{0} -- {1}", err.Error, err.ErrorMessage);
        string details = string.Empty;

        if (err.ErrorDetails != null)
        {
            foreach (var detail in err.ErrorDetails)
            {
                details += string.Format("{0} \n", detail.ToString());
                if (detail.Key == "Email")
                {
                    foreach (var error in detail.Value)
                        _emailField.transform.FindChild("Error").GetComponent<Text>().text += error;
                }
                if (detail.Key == "Password")
                {
                    foreach (var error in detail.Value)
                        _passwordField.transform.FindChild("Error").GetComponent<Text>().text += error;
                }
            }

        }

        Debug.LogError(string.Format("{0}\n {1}\n {2}\n", http, message, details));
    }

    public void SingIn()
    {
        Loading.Load(LoadingScene.Register);
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }


    // Update is called once per frame
    void Update () {
	
	}
}
