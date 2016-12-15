﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class Register : MonoBehaviour {


    public InputField _usernameField;
    public InputField _emailField;
    public InputField _passwordField;
    private Text _loginStatus;
    void Start()
    {
        _loginStatus = GameObject.Find("Status").GetComponent<Text>();
        HideErrors();
    }

    private void HideErrors()
    {
        _usernameField.transform.FindChild("Error").GetComponent<Text>().text = "";
        _emailField.transform.FindChild("Error").GetComponent<Text>().text = "";
        _passwordField.transform.FindChild("Error").GetComponent<Text>().text = "";
        _loginStatus.text = "";
    }

    public void SignIn()
    {
        var titleId = PlayFabSettings.TitleId;
        Debug.Log(titleId);
        string email = _emailField.text;
        Debug.Log(email);
        string username = _usernameField.text;
        string password = _passwordField.text;
        RegisterPlayFabUserRequest req = new RegisterPlayFabUserRequest() { TitleId = titleId, Email = email, Password = password, Username = username, DisplayName = username };
        PlayFabClientAPI.RegisterPlayFabUser(req, OnRegisterCallback, OnApiCallError);
        _loginStatus.text = "Try to register...";
        _loginStatus.color = Color.black;
    }

    void OnRegisterCallback(RegisterPlayFabUserResult result)
    {
        Debug.Log(string.Format("Register Successful. Welcome Player: {0}!", result.PlayFabId));
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
                if (detail.Key == "Username")
                {
                    foreach (var error in detail.Value)
                        _usernameField.transform.FindChild("Error").GetComponent<Text>().text += error + "\n";
                }
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
}
