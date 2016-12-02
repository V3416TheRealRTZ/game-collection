using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {

    #region Public Proporites

    public Transform startPoint;
    #endregion

    public void Start()
    {
        Debug.Log("Start game scene");
        //PhotonNetwork.SetSendingEnabled(1, false);
        //PhotonNetwork.SetReceivingEnabled(1, false);
        GameObject girl = PhotonNetwork.Instantiate("AdvGirl", startPoint.position, startPoint.rotation, 0);
        GameObject.Find("Camera").GetComponent<CameraScript>().setTarget(girl.transform);
    }

    #region Photon Messages

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("1 StartGame");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion
}
