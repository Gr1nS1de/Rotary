using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
	private PlayerModel _playerModel	{ get { return game.model.playerModel; } }
	private PlayerView 	_playerView		{ get { return game.view.playerView; } }

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.GamePlay:
				{
					InitPlayer ();
					break;
				}
					

		}
	}

	private void OnStart()
	{
		//game.view.cameraView.OnStart ();
	}

	private void InitPlayer()
	{
		_playerView.OnInit ();
	}
		
}
