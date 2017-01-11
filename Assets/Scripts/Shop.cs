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

	void Start () {
        if (moneyText != null) moneyText.text = PlayerInfo.Money.ToString();
        PlayerInfo.UpdateMoney();
        getShopItems();
	}
	
	void Update () {
        if (PlayerInfo.IsMoneyChanged)
        {
            if (moneyText != null) moneyText.text = PlayerInfo.Money.ToString();
        }
        if (PlayerInfo.IsInventoryChanged)
            UpdateShopItems();
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
                /*
                foreach (var itemInfo in res.Catalog)
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
                    LockUnlockItem(item);
                    items.Add(item);
                }
                */
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
}
