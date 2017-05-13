using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

//http://www.gameanalytics.com/docs/ga-data
//Progression event

public class CEAnalyticsProgression : CustomEvent
{
	public GAProgressionStatus progressionStatus;
	public string progression01;
	public string progression02;
	public string progression03;
	public int value;
}
