using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSkinWindowController : Controller
{
	private MainMenuPanelModel 		_mainMenuPanelModel			{ get { return ui.model.mainMenuPanelModel; } }

	private int[]					_currentSkinsArray;
	private List<PlayerSkinView>	_playerSkinsViewList;
	private bool					_isStoreInited				= false;
	private int						_currentSkinId				= 0;

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();
					break;
				}

			case N.UIWindowStateChanged_:
				{
					UIWindowState windowState = (UIWindowState)data [0];

					if (windowState == UIWindowState.PlayerSkin)
					{
						UpdateAvailableSkins ();
						ui.model.mainMenuPanelModel.textPlayerSkinGeneralGamesPlayed.text = string.Format("{0}", game.model.playedGamesCount);
						UpdatePlayerSkinStatistics (_currentSkinId);
					}
					break;
				}

			case N.OnPlayerBuySkin_:
				{
					int skinId = (int)data [0];

					SetSkinActive (skinId);
					UpdateAvailableSkins ();
					break;
				}

			case N.PurchaseProductsLoaded_:
				{
					bool isSuccess = (bool)data[0];

					_isStoreInited = isSuccess;
					break;
				}

			case N.OnPlayerSelectSkin__:
				{
					int skinId = (int)data [0];
					bool isAvailable = (bool)data [1];

					if (!isAvailable && _isStoreInited)
					{
						ui.controller.GoToWindowState (UIWindowState.Store);
					}
					else if (isAvailable)
					{
						UpdatePlayerSkinStatistics (skinId);
					}

					break;
				}
		}
	}

	private void OnStart()
	{


	}

	#region public methods
	public void InitPlayerSkins(List<PlayerSkinView> playerSkinsViewList)
	{
		int skinsCount = playerSkinsViewList.Count;

		if (Prefs.PlayerData.GetSkinsArray ().Length == 0)
		{
			InitSkinsArray (skinsCount);

			SetSkinActive (0);
		}

		for (int i = 0; i < skinsCount; i++)
		{
			PlayerSkinView playerSkinView = playerSkinsViewList [i];

			playerSkinView.OnInit (GetSkinPriceById (playerSkinView.SkinId), Prefs.PlayerData.IsSkinActive (playerSkinView.SkinId));
		}

		_playerSkinsViewList = playerSkinsViewList;
	}
	#endregion

	private void UpdatePlayerSkinStatistics(int skinId)
	{
		ui.model.mainMenuPanelModel.textPlayerCurrentSkinGamesPlayed.text = string.Format("{0}", Prefs.PlayerData.GetSkinStatisticGamesPlayed (skinId));

	}

	private void UpdateAvailableSkins()
	{
		for(int i = 0; i < _playerSkinsViewList.Count; i++)
		{
			PlayerSkinView playerSkinView = _playerSkinsViewList [i];

			if (playerSkinView.IsActive)
				continue;
			
			if (playerSkinView.SkinPrice <= game.model.coinsCount)
			{
				playerSkinView.SetAvailable (true);
			}
			else
			{
				playerSkinView.SetAvailable (false);
			}
		}
	}

	private int GetSkinPriceById(int skinId)
	{
		int skinPrice = 0;

		if (skinId >= 0 && skinId <= 3)
		{
			skinPrice = 40;
		}
		else if (skinId >= 4 && skinId <= 7)
		{
			skinPrice = 100;
		}
		else if (skinId >= 8 && skinId <= 11)
		{
			skinPrice = 200;
		}
		else if (skinId >= 12 && skinId <= 15)
		{
			skinPrice = 300;
		}
		else if (skinId >= 16 && skinId <= 19)
		{
			skinPrice = 400;
		}
		else if (skinId >= 20 && skinId <= 23)
		{
			skinPrice = 500;
		}
		else if (skinId >= 24 && skinId <= 27)
		{
			skinPrice = 1000;
		}
		else
		{
			skinPrice = 2000;
		}

		return skinPrice;
	}

	private void InitSkinsArray(int skinsCount)
	{
		int[] skinsArray = new int[skinsCount * 2];
		int skinIndex = 0;

		for(int i = 0; i< skinsArray.Length; i+=2)
		{
			skinsArray [i] = skinIndex;
			skinsArray [i + 1] = 0;

			skinIndex++;
		}

		_currentSkinsArray = skinsArray;
		Prefs.PlayerData.InitSkinsGamesPlayedStatistics (skinsArray);
		Prefs.PlayerData.InitSkinsArray (skinsArray);
	}

	private void SetSkinActive(int skinId)
	{
		Prefs.PlayerData.SetActiveSkin (skinId, true);

		_playerSkinsViewList [skinId].SetSkinActive ();
		_currentSkinsArray = Prefs.PlayerData.GetSkinsArray ();
	}
}

