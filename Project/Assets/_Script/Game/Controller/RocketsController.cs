﻿using UnityEngine;
using System.Collections;

public class RocketsController : Controller
{
	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}
		}

	}

	private void OnStart()
	{

	}

}

