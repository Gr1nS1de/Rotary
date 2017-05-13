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

//https://ussrgames.atlassian.net/wiki/pages/viewpage.action?pageId=23035961#id-%D0%98%D0%B3%D1%80%D0%BE%D0%B2%D0%B0%D1%8F%D0%B0%D0%BD%D0%B0%D0%BB%D0%B8%D1%82%D0%B8%D0%BA%D0%B0-%D0%9F%D0%BE%D0%BB%D1%83%D1%87%D0%B5%D0%BD%D0%B8%D0%B5%D1%80%D0%B5%D1%81%D1%83%D1%80%D1%81%D0%BE%D0%B2%D0%BF%D0%BE%D0%BE%D0%BA%D0%BE%D0%BD%D1%87%D0%B0%D0%BD%D0%B8%D1%8E%D0%B1%D0%BE%D1%8F
public enum AnalyticsResoucesCurrency
{
    notdefined,
	coin,
	crystal,
   	life
}

public enum AnalyticsItemType
{
    notdefined,
    purchaseInGame,
    purchaseShop,
    battle,
    battleSquad,
    purchaseWeapon,
    purchaseArmor,
	purchaseWeaponUpgrade,
	purchaseArmorUpgrade,
    purchaseWeaponForceUpgrade,
    purchaseArmorForceUpgrade,
    purchaseWeaponCamouflage,
    purchaseArmorCamouflage,
	bonus,
	peroidTops,
	purchasePlayer,
	purchaseSquad,
}

public class CEAnalyticsResources : CustomEvent
{
    public GAResourceFlowType flowType;
    public AnalyticsItemType itemType;
    public string itemId;
    public int amount;
    public AnalyticsResoucesCurrency resourceCurrency;
}
