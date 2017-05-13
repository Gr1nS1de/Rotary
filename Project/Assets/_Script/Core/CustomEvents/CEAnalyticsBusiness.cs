using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://www.gameanalytics.com/docs/ga-data
//Business event

public enum AnalyticsBusinesItemType
{
    notdefined, 
	coin,
	crystal,
	life
}
public class CEAnalyticsBusiness : CustomEvent
{
    public string cartType;
    public AnalyticsBusinesItemType itemType;
    public string itemId;
    public int amount;
    public string currency;
}
