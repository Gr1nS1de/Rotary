using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerDataController : Controller
{
	private GameModel 				_gameModel	{ get { return game.model;}}

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.RCAwakeLoad:
				{
					OnAwakeInit ();
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

			case N.OnPlayerBuySkin_:
				{
					int skinId = (int)data [0];

					OnPlayerBuySkin (skinId);

					break;
				}

			case N.OnPurchasedDoubleCoin:
				{
					Prefs.PlayerData.SetDoubleCoin ();
					_gameModel.isDoubleCoin = true;
					break;
				}

			case N.OnPurchasedCoinsPack_00:
				{
					UpdatePlayerItemCount (ItemTypes.Coin, 3000);
					break;
				}

			case N.OnPurchasedCoinsPack_01:
				{
					UpdatePlayerItemCount (ItemTypes.Coin, 15000);
					break;
				}

			case N.OnPlayerGetGift_:
				{
					DailyGiftElementId dailyGiftElementId = (DailyGiftElementId)data [0];

					OnClickDailyGiftElement(dailyGiftElementId);
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
		_gameModel.playerRecord = Prefs.PlayerData.GetRecord ();
		_gameModel.coinsCount = Prefs.PlayerData.GetCoinsCount ();
		_gameModel.crystalsCount = Prefs.PlayerData.GetCrystalsCount ();
		_gameModel.isDoubleCoin = Prefs.PlayerData.GetDoubleCoin () == 1;
		_gameModel.playedGamesCount = Prefs.PlayerData.GetPlayedGamesCount();
	}

	private void UpdatePlayerItemCount(ItemTypes itemType, int count, bool isNotify = true)
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

					game.model.coinsCount += count;
					break;
				}

			case ItemTypes.Crystal:
				{
					if (count > 0)
						Prefs.PlayerData.CreditCrystals (absCount);
					else
						Prefs.PlayerData.DebitCrystals (absCount);

					game.model.crystalsCount += count;
					break;
				}
					
		}

		if(isNotify)
			Notify (N.PlayerItemCountChange__, NotifyType.ALL, itemType, count);
	}

	private void OnClickDailyGiftElement(DailyGiftElementId elementId)
	{
		switch (elementId)
		{
			case DailyGiftElementId.GiftHour_00:
				{
					UpdatePlayerItemCount (ItemTypes.Coin, 10);
					break;
				}

			default:
				{
					int coinsGiftCount = Mathf.Clamp( Prefs.PlayerTimers.GetDaysReturn () * 10, 10, 100);

					UpdatePlayerItemCount (ItemTypes.Coin, coinsGiftCount);

					break;
				}

		}

	}

	private void OnAddScore()
	{
		_gameModel.currentScore++;

		if (_gameModel.currentScore > _gameModel.playerRecord)
		{
			OnNewRecord (_gameModel.currentScore);
		}

		Notify (N.GameAddScore);
	}

	private void OnNewRecord(int score)
	{
		Prefs.PlayerData.SetRecord (score);
		_gameModel.playerRecord = score;
		Notify (N.OnPlayerNewRecord_, NotifyType.ALL, score);
	}


	private void OnPlayerBuySkin(int skinId)
	{
		int skinPrice = ui.view.GetPlayerSkinElement (skinId).SkinPrice;

		UpdatePlayerItemCount (ItemTypes.Coin, -skinPrice);
	}

	private void OnPlayerImpactItem(ItemView itemView)
	{
		ItemTypes itemType = itemView.ItemType;

		switch (itemType)
		{
			case ItemTypes.Coin:
				{
					int coinsCount = (_gameModel.isDoubleCoin ? 2 : 1);

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

		Prefs.PlayerData.IncreaseSkinPlayedGamesStatistics (_gameModel.playerModel.currentSkinId);

		_gameModel.playedGamesCount = currentPlayedGamesCount;
	}
		
}

