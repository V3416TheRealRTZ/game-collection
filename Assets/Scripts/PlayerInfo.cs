using UnityEngine;
using System.Collections;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;

public static class PlayerInfo {
    private class Updatable<T>
    {
        public Updatable(T _value){ Value = _value; }
        public T Value { get; set; }

        private bool updating = false;
        public bool Updating
        {
            get { return updating; }
            set
            {
                if (updating && !value)
                    changed = true;
                updating = value;
            }
        }
        public bool changed = false;
    }

    private static string playFabId;
    private static string sessionTicket;
    private static string username = "";
    private static string displayName = "";

    public static string PlayFabId {  get; set; }
    public static string SessionTicket { get; set; }
    public static string Username { get; set; }
    public static string DisplayName { get; set; }


    private static Updatable<int> score = new Updatable<int>(-1);
    private static Updatable<int> money = new Updatable<int>(-1);
    private static Updatable<int> rating = new Updatable<int>(-1);
    private static Updatable<List<ItemInstance>> inventory = new Updatable<List<ItemInstance>>(new List<ItemInstance>());
    private static Updatable<Dictionary<string, string>> titleUserData = new Updatable<Dictionary<string, string>>(new Dictionary<string, string>());

    public static int Score { get { return score.Value; } set { score.Value = value; } }
    public static bool IsScoreChanged {
        get {
            if (score.changed) { 
                score.changed = false;
                return true;
            }
            return false;
        }
        private set { } }

    public static int Rating { get { return rating.Value; } set { rating.Value = value; } }
    public static bool IsRatingChanged
    {
        get
        {
            if (rating.changed)
            {
                rating.changed = false;
                return true;
            }
            return false;
        }
        private set { }
    }

    public static int Money { get { return money.Value; } set { money.Value = value; } }
    public static bool IsMoneyChanged
    {
        get
        {
            if (money.changed)
            {
                money.changed = false;
                return true;
            }
            return false;
        }
        private set { }
    }

    public static List<ItemInstance> Inventory { get { return inventory.Value; } private set { inventory.Value = value; } }
    public static bool IsInventoryChanged
    {
        get
        {
            if (inventory.changed)
            {
                inventory.changed = false;
                return true;
            }
            return false;
        }
        private set { }
    }
    public static void addItemToInventory(ItemInstance item)
    {
        inventory.Value.Add(item);
    }

    public static Dictionary<string,string> TitleUserData { get { return titleUserData.Value; } private set { titleUserData.Value = value; } }
    public static bool IsTitleUserDataChanged
    {
        get
        {
            if (titleUserData.changed)
            {
                titleUserData.changed = false;
                return true;
            }
            return false;
        }
        private set { }
    }

    public static void UpdateScore()
    {
        score.Updating = true;
        rating.Updating = true;
        GetLeaderboardAroundPlayerRequest req = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = "Score",
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(req, (GetLeaderboardAroundPlayerResult res) =>
        {
            if (res.Leaderboard != null)
                foreach(var person in res.Leaderboard)
                    if (person.PlayFabId == PlayFabId)
                    {
                        Score = person.StatValue;
                        Rating = person.Position + 1;
                        score.Updating = false;
                        rating.Updating = false;
                        return;
                    }
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));
    }
    public static void UpdateMoney()
    {
        money.Updating = true;
        AddUserVirtualCurrencyRequest reqCur = new AddUserVirtualCurrencyRequest()
        {
            VirtualCurrency = "GO",
            Amount = 0,
        };
        PlayFabClientAPI.AddUserVirtualCurrency(reqCur, (ModifyUserVirtualCurrencyResult res) =>
        {
            if (res != null)
                Money = res.Balance;
            money.Updating = false;
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));

    }
    public static void UpdateInventory()
    {
        inventory.Updating = true;
        GetUserInventoryRequest reqCur = new GetUserInventoryRequest() { };
        PlayFabClientAPI.GetUserInventory(reqCur, (GetUserInventoryResult res) =>
        {
            if (res != null)
            {
                Inventory = res.Inventory;
                Money = res.VirtualCurrency["GO"];
            }
            inventory.Updating = false;
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));
    }
    public static void UpdateTitleUserData()
    {
        titleUserData.Updating = true;
        GetUserDataRequest reqCur = new GetUserDataRequest() { };
        PlayFabClientAPI.GetUserData(reqCur, (GetUserDataResult res) =>
        {
            if (res != null)
            {
                Dictionary<string, string> newData = new Dictionary<string, string>();
                foreach (var data in res.Data)
                    newData.Add(data.Key, data.Value.Value);
                TitleUserData = newData;
            }
            titleUserData.Updating = false;
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));
    }

    
}
