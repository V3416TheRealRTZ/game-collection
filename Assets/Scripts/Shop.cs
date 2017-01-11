using UnityEngine;
using System.Collections;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.UI;
using System.Collections.Generic;
//using PlayFab.Json;

public class Shop : MonoBehaviour {

    public Text moneyText;
    public string catalogVersion = "main";
    public List<ShopItem> items = new List<ShopItem>();
    public List<CatalogItem> allCatalogItems;
    public Text speedText;
    public Text jumpText;
    public Text moneybonusText;
    public Text shieldText;

    void Start () {
        if (moneyText != null) moneyText.text = PlayerInfo.Money.ToString();
        PlayerInfo.UpdateMoney();
        PlayerInfo.UpdateTitleUserData();
        getShopItems();
	}
	
	void Update () {
        if (PlayerInfo.IsMoneyChanged)
        {
            if (moneyText != null) moneyText.text = PlayerInfo.Money.ToString();
        }
        if (PlayerInfo.IsInventoryChanged)
            UpdateShopItems();

        if (PlayerInfo.IsTitleUserDataChanged)
        {
            UpdateUserDataInfo();
        }
	}

    public void BackToMenu()
    {
        Loading.Load(LoadingScene.Lobby);
    }

    public void getShopItems()
    {
        GetCatalogItemsRequest req = new GetCatalogItemsRequest()
        {
            CatalogVersion = catalogVersion,
        };
        PlayFabClientAPI.GetCatalogItems(req, (GetCatalogItemsResult res) =>
        {
            if (res != null)
            {
                allCatalogItems = res.Catalog;
                UpdateShopItems();
            }
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));
    }

	public void buyItem(ShopItem item)
	{
        PurchaseItemRequest req = new PurchaseItemRequest()
        {
            CatalogVersion = catalogVersion,
            ItemId = item.itemId,
            VirtualCurrency = "GO",
            Price = System.Convert.ToInt32(item.priceText.text)
        };
        PlayFabClientAPI.PurchaseItem(req, (PurchaseItemResult res) =>
        {
            if (res != null)
            {
                Debug.Log("bought + " + res.Items[0].DisplayName);
                PlayerInfo.UpdateMoney();
                items.Remove(item);
                saveBuyBonus(item.catalogItem);
                Destroy(item.gameObject);
                PlayerInfo.UpdateInventory();
                PlayerInfo.UpdateTitleUserData();
            }
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));
    }

    public bool isCanBuy(ShopItem item)
    {
        return System.Convert.ToInt32(item.priceText.text) <= PlayerInfo.Money;
    }

    public void LockUnlockItem(ShopItem item)
    {
        if (isCanBuy(item)) item.Unlock(); else item.Lock();
    }

    private bool isClassContainedInItems(CatalogItem inventoryItem)
    {
        return items.Find(item => { return item.itemClass == inventoryItem.ItemClass; }) != null;
    }

    private bool isContainedInInventory(CatalogItem inventoryItem)
    {
        return PlayerInfo.Inventory.Find(item => { return item.ItemId == inventoryItem.ItemId; }) != null;
    }

    private void UpdateShopItems()
    {
        foreach(var shopItem in items)
        {
            Destroy(shopItem.gameObject);
        }
        items.Clear();
        foreach (var itemInfo in allCatalogItems)
        {
            if (isClassContainedInItems(itemInfo) || isContainedInInventory(itemInfo))
                continue;

            GameObject i = (GameObject)Instantiate(Resources.Load<GameObject>("ShopItem"), Vector3.zero, Quaternion.identity);
            ShopItem item = i.GetComponent<ShopItem>();
            item.titleText.text = itemInfo.DisplayName;
            item.descriptionText.text = itemInfo.Description;
            item.priceText.text = itemInfo.VirtualCurrencyPrices["GO"].ToString();
            item.itemId = itemInfo.ItemId;
            item.itemClass = itemInfo.ItemClass;
            item.catalogItem = itemInfo;
            LockUnlockItem(item);
            items.Add(item);
        }
    }

    private void saveBuyBonus(CatalogItem item)
    {
        Dictionary<string, string> customData = PlayFab.Json.JsonWrapper.DeserializeObject<Dictionary<string, string>>(item.CustomData);
        if (customData.ContainsKey("addable"))
        {
            int addable = System.Convert.ToInt32(customData["addable"]);
            int newValue = addable;
            if (PlayerInfo.TitleUserData.ContainsKey(item.ItemClass))
            {
                newValue = System.Convert.ToInt32(PlayerInfo.TitleUserData[item.ItemClass]) + addable;
                PlayerInfo.TitleUserData[item.ItemClass] = newValue.ToString();
            }
            else
                PlayerInfo.TitleUserData.Add(item.ItemClass, newValue.ToString());
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(item.ItemClass, newValue.ToString());
            UpdateUserDataRequest reqCur = new UpdateUserDataRequest()
            {
                Data = data                
            };
            PlayFabClientAPI.UpdateUserData(reqCur, (UpdateUserDataResult res) =>
            {
                Debug.Log("updated user data");
            },
            (PlayFabError err) => Debug.Log(err.ErrorMessage));

        }

    }

    private void UpdateUserDataInfo()
    {
        string speed = PlayerInfo.TitleUserData.ContainsKey("speed") ? PlayerInfo.TitleUserData["speed"] : "0";
        string jump = PlayerInfo.TitleUserData.ContainsKey("jump") ? PlayerInfo.TitleUserData["jump"] : "0";
        string moneybonus = PlayerInfo.TitleUserData.ContainsKey("moneybonus") ? PlayerInfo.TitleUserData["moneybonus"] : "0";
        string shield = PlayerInfo.TitleUserData.ContainsKey("shield") ? PlayerInfo.TitleUserData["shield"] : "0";
        
        speedText.text = "+" + speed + "%";
        jumpText.text = "+" + jump + "%";
        moneybonusText.text = "+" + moneybonus + "s";
        shieldText.text = "+" + shield + "s";
    }
}



