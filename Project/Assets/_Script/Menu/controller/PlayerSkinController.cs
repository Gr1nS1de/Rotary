using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSkinController : Controller
{
	private MainMenuPanelModel 		_mainMenuPanelModel			{ get { return ui.model.mainMenuPanelModel; } }

	private int[]					_currentSkinsArray;
	private List<PlayerSkinView>	_playerSkinsViewList;

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
					}
					break;
				}

			case N.OnPlayerBuySkin_:
				{
					string skinId = (string)data [0];
					int skinIndex = int.Parse(skinId.Split ('_') [1]);

					SetSkinActive (skinIndex);
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
			InitSkinsArray (skinsCount);

		for (int i = 0; i < skinsCount; i++)
		{
			PlayerSkinView playerSkinView = playerSkinsViewList [i];

			playerSkinView.OnInit (GetSkinPriceByIndex (i), Prefs.PlayerData.IsSkinActive (i));
		}

		_playerSkinsViewList = playerSkinsViewList;
	}
	#endregion

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

	private int GetSkinPriceByIndex(int skinIndex)
	{
		int skinPrice = 0;

		if (skinIndex >= 0 && skinIndex <= 3)
		{
			skinPrice = 30;
		}
		else if (skinIndex >= 4 && skinIndex <= 7)
		{
			skinPrice = 100;
		}
		else if (skinIndex >= 8 && skinIndex <= 11)
		{
			skinPrice = 200;
		}
		else if (skinIndex >= 12 && skinIndex <= 15)
		{
			skinPrice = 300;
		}
		else if (skinIndex >= 16 && skinIndex <= 19)
		{
			skinPrice = 400;
		}
		else if (skinIndex >= 20 && skinIndex <= 23)
		{
			skinPrice = 500;
		}
		else if (skinIndex >= 24 && skinIndex <= 27)
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
		int[] skinsArray = new int[skinsCount*2];
		int skinIndex = 0;

		for(int i = 0; i< skinsArray.Length; i+=2)
		{
			skinsArray [i] = skinIndex;
			skinsArray [i + 1] = 0;

			skinIndex++;
		}

		_currentSkinsArray = skinsArray;
		Prefs.PlayerData.InitSkinsArray (skinsArray);
	}

	private void SetSkinActive(int skinIndex)
	{

	}
}

