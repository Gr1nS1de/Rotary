using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

//http://www.gameanalytics.com/docs/ga-data
//Resource  event
/*
 * Суть полей:

    flowType - тип операции с ресурсом. Имеет два предустановленных значения
        Зарабатывать - тип source
        Тратить - тип sink
    itemType - отличается от itemType из бизнес эвента. Тут itemType  - это группа активностей на которые тратится валюта, например игрок покупает один раунд боя(itemType) за две единицы энергии. Бой - это группа активностей.
    itemId - точное указание активности внутри группы активностей. Например, игрок покупает один раунд боя(itemType) на карте номер два(itemId) за две единицы(amount) энергии(resourceCurrency).
    amount - количество единиц валюты.
    resourceCurrency - сама валюта.
 * */

public enum AnalyticsResoucesCurrency
{
    notdefined,
	Coin,
	Crystal,
	Magnet,
   	Life
}

public enum AnalyticsItemType
{
    notdefined,
    PurchaseInGame,	
	GameResult
}

public class CEAnalyticsResources : CustomEvent
{
    public GAResourceFlowType flowType;
    public AnalyticsItemType itemType;
    public string itemId;
    public int amount;
    public AnalyticsResoucesCurrency resourceCurrency;
}
