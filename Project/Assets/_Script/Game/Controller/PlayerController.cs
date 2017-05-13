using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

			case N.GameStartPlay:
				{
					InitPlayer ();
					break;
				}

			case N.OnPlatformInvisible_:
				{
					PlatformView platformView = (PlatformView)data [0];

					if (_playerView.ScorePlatformsList.Contains (platformView))
						_playerView.ScorePlatformsList.Remove (platformView);
					
					break;
				}
					
			case N.GameOver_:
				{
					//GameOverData gameOverData = (GameOverData)data[0];

					ResetPlayer ();
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
		Vector2 screenSize = GM.Instance.ScreenSize;
		Vector3 playerInitPosition = new Vector3((-screenSize.x / 2f) + screenSize.x * _playerModel.initScreenPosX, screenSize.y / 2f * 0.5f, 0f);

		_playerView.OnInit (playerInitPosition);
	}

	private void ResetPlayer()
	{
		_playerView.transform.DOMove (Vector3.zero, 0.5f);
		_playerView.ScorePlatformsList.Clear ();
		_playerView.GetComponent<ConstantForce2D> ().force = Vector2.zero;
		_playerView.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}
		
}
