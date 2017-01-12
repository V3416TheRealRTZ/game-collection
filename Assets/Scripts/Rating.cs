using UnityEngine;
using System.Collections;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.UI;

public class Rating : MonoBehaviour {

	// Use this for initialization
	void Start () {
        getRating();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void getRating()
    {
        GetLeaderboardRequest req = new GetLeaderboardRequest()
        {
            StatisticName = "Score",
        };
        PlayFabClientAPI.GetLeaderboard(req, (GetLeaderboardResult res) =>
        {
            if (res.Leaderboard != null)
                foreach (var person in res.Leaderboard)
                {
                    GameObject item = (GameObject)Instantiate(Resources.Load<GameObject>("RatingListEntry"), Vector3.zero, Quaternion.identity);
                    item.GetComponent<Transform>().SetParent(GameObject.Find("Canvas/Scroll View/Viewport/Content").GetComponent<Transform>());
                    item.transform.FindChild("Position").GetComponent<Text>().text = (1+person.Position).ToString();
                    item.transform.FindChild("Username").GetComponent<Text>().text = person.DisplayName.ToString();
                    item.transform.FindChild("Score").GetComponent<Text>().text = person.StatValue.ToString();
                }
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));
    }

    public void BackToMenu()
    {
        Loading.Load(LoadingScene.Lobby);
    }


}
