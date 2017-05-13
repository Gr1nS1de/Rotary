using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;

public class AnalyticsManager
{
	public AnalyticsManager()
    {        
        //подписка на игровые события
        CustomEventDelegate.ActionCustomEvent += CustomEvent;
    }

    private void CustomEvent(CustomEvent cEvent)
	{
		////Design event
		if (cEvent is CEAnalytics)
		{
			var aEvent = (CEAnalytics)cEvent;
			//логировние полученых параметров
			//Debug.Log("Analytic Design Event = " + aEvent.EventName);                

			if (aEvent.IsHasEventValue)
			{
				GameAnalytics.NewDesignEvent (aEvent.GetEventForSend (), aEvent.EventValue);
				Debug.Log ("send GA DesignEvent = " + aEvent.GetEventForSend () + " Value = " + aEvent.EventValue);
			}
			else
			{
				GameAnalytics.NewDesignEvent (aEvent.GetEventForSend ());
				Debug.Log ("send GA DesignEvent = " + aEvent.GetEventForSend ());
			}                                    
		}

		if (cEvent is CEAnalyticsBusiness)
		{
			var aEvent = (CEAnalyticsBusiness)cEvent;
			//логировние полученых параметров
			//преоразовать валюты к стандарту A-Z, 3 characters and in the standard           
			Debug.LogFormat ("send GA Busines Event cartType = {0}; itemType = {1}; itemId = {2}; amount = {3}, currency = {4}", aEvent.cartType, aEvent.itemType, aEvent.itemId, aEvent.amount, aEvent.currency);
			GameAnalytics.NewBusinessEvent (aEvent.currency, aEvent.amount, aEvent.itemType.ToString (), aEvent.itemId, aEvent.cartType);
		}
//
		if (cEvent is CEAnalyticsResources)
		{
			var aEvent = (CEAnalyticsResources)cEvent;
			Debug.LogFormat ("send GA Resources Event flowType = {0}; itemType = {1}; itemId = {2}; amount = {3}, resourceCurrency = {4}", aEvent.flowType, aEvent.itemType, aEvent.itemId, aEvent.amount, aEvent.resourceCurrency);
			GameAnalytics.NewResourceEvent (aEvent.flowType, aEvent.resourceCurrency.ToString (), aEvent.amount, aEvent.itemType.ToString (), aEvent.itemId);
		}

		if (cEvent is CEAnalyticsProgression)
		{
			var aEvent = (CEAnalyticsProgression)cEvent;
			Debug.LogFormat ("send GA Progression Event progressionStatus = {0}; progression01 = {1}; progression02 = {2}; progression03 = {3}, value = {4}", aEvent.progressionStatus, aEvent.progression01, aEvent.progression02, aEvent.progression03, aEvent.value);
			GameAnalytics.NewProgressionEvent (aEvent.progressionStatus, aEvent.progression01, aEvent.progression02, aEvent.progression03, aEvent.value);
		}

		//для отправки в аналитику серверных ошибок
		if (cEvent is CEError)
		{
			var aEvent = (CEError)cEvent;
			//формат тип critical, Server error executePayment:200
			string sendString = "Server error " + aEvent.ServerResponseEvent + ":" + aEvent.IDErrorServ;
			GameAnalytics.NewErrorEvent (GAErrorSeverity.Critical, sendString);
//            Debug.LogError("ErrorEvent " + sendString);
		}
	}

    string GetGAConvertNameValute(string value)
    {
        string convertUpper = value.ToUpper();
        string convertValue = "";

        for (int i = 0; i < 3; i++)
        {
            if (i < convertUpper.Length)
                convertValue += convertUpper[i];
            else
                convertValue += "X";
        }

        return convertValue;
    }
}
