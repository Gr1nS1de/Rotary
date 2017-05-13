using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Design event
//http://www.gameanalytics.com/docs/ga-data

public enum AnalyticsEventName
{
    Undifined,
	GameStates_v1,
	BuySkin_v1,
	BuyTheme_v1,
	FirstBattles_v1
}

public class CEAnalytics : CustomEvent
{
    public AnalyticsEventName EventName;
    public float EventValue;
    //надо ли отправлять в формате 
    //GameAnalytics.NewDesignEvent (string eventName, float eventValue);
    //https://guides.gameanalytics.com/content/sdk?page=unity&step=9
    public bool IsHasEventValue = false;
    public IDictionary<string, object> Parameters = new Dictionary<string, object>();

    public string GetEventForSend()
    {
        string gaEvent = EventName.ToString();        
        foreach (var p in Parameters)
        {
            if (!System.String.IsNullOrEmpty(p.Key))
            {
                if (p.Value!=null)
                    gaEvent += ":" + p.Key + ":" + p.Value;
                else
                    gaEvent += ":" + p.Key;
            }
            else
                gaEvent += ":" + p.Value;
        }
        return gaEvent;
    }
}
