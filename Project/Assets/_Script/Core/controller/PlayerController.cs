using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.GameOnStart:
				{
					OnStart ();

					break;
				}


		}
	}

	private void OnStart()
	{
		//game.view.cameraView.OnStart ();
	}
}
