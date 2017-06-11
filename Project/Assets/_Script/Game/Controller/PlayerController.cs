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

			case N.GameStart:
				{
					InitPlayer ();
					break;
				}

			case N.PlayerImpactItem__:
				{
					ItemView itemView = (ItemView)data [0];
					//Vector2 contactPoint = (Vector2)data [1];

					OnPlayerImpactItem (itemView);
					break;
				}

			case N.OnPlatformInvisible_:
				{
					PlatformView platformView = (PlatformView)data [0];

					if (_playerView.ScorePlatformsList.Contains (platformView))
						_playerView.ScorePlatformsList.Remove (platformView);
					
					break;
				}

			case N.OnPlayerSelectSkin__:
				{
					int skinId = (int)data [0];
					bool isAvailable = (bool)data [1];

					if(isAvailable)
						SetupPlayerSkin (skinId);

					break;
				}

			case N.UIWindowStateChanged_:
				{
					UIWindowState windowState = (UIWindowState)data [0];

					switch (windowState)
					{
						case UIWindowState.Pause:
							{
								GoToPlayerState (PlayerState.GamePause);
								break;
							}

						case UIWindowState.PlayerSkin:
							{
								GoToPlayerState (PlayerState.PlayerSkinWindow);
								break;
							}

						default:
							{
								GoToPlayerState (PlayerState.MainMenu);
								break;
							}
							break;
					}
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
		_playerModel.currentSkinId = Prefs.PlayerData.GetCurrentSkinId ();

		SetupPlayerSkin (_playerModel.currentSkinId);
	}

	private void GoToPlayerState(PlayerState playerState)
	{
		switch (playerState)
		{
			case PlayerState.MainMenu:
				{
					_playerView.SetStaticPlayer (false);
					break;
				}

			case PlayerState.PlayerSkinWindow:
				{
					Vector3 currentSkinImageWorldPosition = ui.model.mainMenuPanelModel.imageCurrentPlayerSkin.GetComponent<RectTransform>().TransformPoint(Camera.main.ScreenToWorldPoint( ui.model.mainMenuPanelModel.imageCurrentPlayerSkin.GetComponent<RectTransform>().rect.center));

					_playerView.SetStaticPlayer (true);
					_playerView.PlayerRenderer.transform.DOMove (currentSkinImageWorldPosition, 0.6f);
					break;
				}

			case PlayerState.GamePause:
				{
					
					break;
				}

			case PlayerState.GamePlay:
				{
					break;
				}
		}

		_playerModel.playerState = playerState;
	}

	private void SetupPlayerSkin(int skinId)
	{
		PlayerSkinView selectedSkinView = ui.view.GetPlayerSkinElement (skinId);

		_playerView.PlayerRenderer.sprite = selectedSkinView.SkinSprite;

		if (_playerModel.currentSkinId != skinId)
		{
			Prefs.PlayerData.SetCurrentSkin (skinId);
			_playerModel.currentSkinId = skinId;
		}
	}

	private void OnPlayerImpactItem(ItemView itemView)
	{
		ItemTypes itemType = itemView.ItemType;

		switch (itemType)
		{
			case ItemTypes.Coin:
				{
					
					break;
				}

			case ItemTypes.Crystal:
				{
					
					break;
				}

			default:
				{
					
					break;
				}
		}
	}

	private void InitPlayer()
	{
		Vector2 screenSize = GM.Instance.ScreenSize;
		Vector3 playerInitPosition = new Vector3(-screenSize.x / 2f, screenSize.y / 2f * 0.5f, 0f);
		Rigidbody2D playerRB = _playerView.PlayerRenderer.GetComponent<Rigidbody2D> ();

		_playerView.OnInit (playerInitPosition);

		playerRB.transform.DOMove (Vector3.zero, 0.5f)
			.OnComplete (() =>
			{

				playerRB.velocity = Vector2.zero;

				if (_playerModel.forceOnInit != 0f)
					playerRB.AddForce (new Vector2 (1f, 1f) * _playerModel.forceOnInit, ForceMode2D.Impulse);

				GoToPlayerState (PlayerState.GamePlay);
			});
		
	}

	private void ResetPlayer()
	{
		GoToPlayerState (PlayerState.MainMenu);
		_playerView.PlayerRenderer.transform.DOMove (Vector3.zero, 0.5f);
		_playerView.ScorePlatformsList.Clear ();
		_playerView.PlayerRenderer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}
		
}
