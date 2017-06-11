using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerPocketController : Controller
{
	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.RCAwakeLoad:
				{
					break;
				}
		}

	}

}

