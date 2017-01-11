using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayFab.ClientModels;

public class ShopItem : MonoBehaviour {

    public Image img;
    public Text titleText;
    public Text descriptionText;
    public Text priceText;
    public Button buyButton;
    public string itemId;
    public string itemClass;
    public CatalogItem catalogItem;
    // Use this for initialization

    void Awake()
    {
        GetComponent<Transform>().SetParent(GameObject.Find("Canvas/Scroll View/Viewport/Content").GetComponent<Transform>());
        buyButton.onClick.AddListener(() => GameObject.Find("Shop").GetComponent<Shop>().buyItem(this));
    }

    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Lock()
    {
        priceText.color = Color.red;
        buyButton.interactable = false;
    }

    public void Unlock()
    {
        priceText.color = Color.green;
        buyButton.interactable = true;
    }
}
