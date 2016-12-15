using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {

    private bool _isLeaving = false;

    #region Public Proporites

    public Transform startPoint;
    #endregion

    public void Start()
    {

        Debug.Log("Start game scene");
        //PhotonNetwork.SetSendingEnabled(1, false);
        //PhotonNetwork.SetReceivingEnabled(1, false);
        GameObject girl = null;
        if (PhotonNetwork.room == null)
            girl = (GameObject)Instantiate(Resources.Load<GameObject>("AdvGirl"), startPoint.position, startPoint.rotation);
        else
            girl = PhotonNetwork.Instantiate("AdvGirl", startPoint.position, startPoint.rotation, 0);
        GameObject.Find("Camera").GetComponent<CameraScript>().setTarget(girl.transform);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !_isLeaving)
        {
            _isLeaving = true;
            LeaveRoom();
        }

    }

    #region Photon Messages

    public void OnLeftRoom()
    {
        Debug.Log("Leave room");
    }

    public void OnConnectedToMaster()
    {
        Loading.Load(LoadingScene.Lobby);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion
}
