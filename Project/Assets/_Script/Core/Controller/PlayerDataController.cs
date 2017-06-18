using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerDataController : Controller
{
	private PlayerDataModel 				_playerDataModel	{ get { return core.playerDataModel;}}

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.RCAwakeLoad:
				{
					OnAwakeInit ();
					break;
				}

			case N.GameStart:
				{
					_playerDataModel.currentScore = 0;
					break;
				}

			case N.PlayerImpactItem__:
				{
					ItemView itemView = (ItemView)data [0];
					Vector2 contactPoint = (Vector2)data [1];

					OnPlayerImpactItem (itemView);
					break;
				}

			case N.PlayerLeftPlatform_:
				{
					PlatformView platformView = (PlatformView)data [0];

					OnAddScore ();
					break;
				}

			case N.OnEndShowAdVideo_:
				{
					bool isSuccess = (bool)data [0];

					if (isSuccess)
					{
						int rewardCoinsCount = 50;

						UpdatePlayerItemCount (ItemTypes.Coin, rewardCoinsCount);
					}
					break;
				}

			case N.OnPlayerGetDailyGift__:
				{
					DailyGiftElementId dailyGiftElementId = (DailyGiftElementId)data [0];
					int giftCoinsCount = (int)data [1];

					OnClickDailyGiftElement(dailyGiftElementId, giftCoinsCount);
					break;
				}

			case N.GameOver_:
				{
					GameOverData gameOverData = (GameOverData)data[0];

					IncreasePlayedGamesCount ();
					UpdatePlayerItemCount (ItemTypes.Coin, 0);//Just call update notification
					break;
				}

		}

	}

	private void OnAwakeInit()
	{
		_playerDataModel.playerRecord = Prefs.PlayerData.GetRecord ();
		_playerDataModel.coinsCount = Prefs.PlayerData.GetCoinsCount ();
		_playerDataModel.crystalsCount = Prefs.PlayerData.GetCrystalsCount ();
		_playerDataModel.isDoubleCoin = Prefs.PlayerData.GetDoubleCoin () == 1;
		_playerDataModel.playedGamesCount = Prefs.PlayerData.GetPlayedGamesCount();
	}

	#region public methods
	public void UpdatePlayerItemCount(ItemTypes itemType, int count, bool isNotify = true)
	{
		int absCount = Mathf.Abs (count);

		switch(itemType)
		{
			case ItemTypes.Coin:
				{
					if (count > 0)
						Prefs.PlayerData.CreditCoins (absCount);
					else
						Prefs.PlayerData.DebitCoins (absCount);

					_playerDataModel.coinsCount += count;
					break;
				}

			case ItemTypes.Crystal:
				{
					if (count > 0)
						Prefs.PlayerData.CreditCrystals (absCount);
					else
						Prefs.PlayerData.DebitCrystals (absCount);

					_playerDataModel.crystalsCount += count;
					break;
				}
					
		}

		if(isNotify)
			Notify (N.PlayerItemCountChange__, NotifyType.ALL, itemType, count);
	}

	public void OnPlayerBuySkin(int skinId)
	{
		int skinPrice = ui.view.GetPlayerSkinElement (skinId).SkinPrice;

		UpdatePlayerItemCount (ItemTypes.Coin, -skinPrice);
	}

	public void ActivateDoubleCoin()
	{
		Prefs.PlayerData.SetDoubleCoin ();
		_playerDataModel.isDoubleCoin = true;
	}
	#endregion

	private void OnClickDailyGiftElement(DailyGiftElementId elementId, int giftCoinsCount)
	{
		switch (elementId)
		{
			case DailyGiftElementId.GiftHour_00:
				{
					UpdatePlayerItemCount (ItemTypes.Coin, giftCoinsCount);
					break;
				}

			default:
				{
					UpdatePlayerItemCount (ItemTypes.Coin, giftCoinsCount);

					break;
				}

		}

	}

	private void OnAddScore()
	{
		_playerDataModel.currentScore++;

		if (_playerDataModel.currentScore > _playerDataModel.playerRecord)
		{
			OnNewRecord (_playerDataModel.currentScore);
		}

		Notify (N.GameAddScore);
	}

	private void OnNewRecord(int score)
	{
		Prefs.PlayerData.SetRecord (score);
		_playerDataModel.playerRecord = score;
		Notify (N.OnPlayerNewRecord_, NotifyType.ALL, score);
	}

	private void OnPlayerImpactItem(ItemView itemView)
	{
		ItemTypes itemType = itemView.ItemType;

		switch (itemType)
		{
			case ItemTypes.Coin:
				{
					int coinsCount = (_playerDataModel.isDoubleCoin ? 2 : 1);

					UpdatePlayerItemCount(ItemTypes.Coin, coinsCount, false);
					break;
				}

			case ItemTypes.Crystal:
				{
					int crystalsCount = itemView.CrystalFractureCount;

					UpdatePlayerItemCount (ItemTypes.Crystal, crystalsCount, false);
					break;
				}
		}
	}

	private void IncreasePlayedGamesCount()
	{
		int currentPlayedGamesCount = 1;

		if (!PlayerPrefs.HasKey (Prefs.PlayerData.GamesPlayedCount))
		{
			PlayerPrefs.SetInt (Prefs.PlayerData.GamesPlayedCount, currentPlayedGamesCount);
		}
		else
		{
			Prefs.PlayerData.IncreasePlayedGamesCount ();

			currentPlayedGamesCount = Prefs.PlayerData.GetPlayedGamesCount();
		}

		Prefs.PlayerData.IncreaseSkinPlayedGamesStatistics (game.model.playerModel.currentSkinId);

		_playerDataModel.playedGamesCount = currentPlayedGamesCount;
	}
		
}

