using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SmartLocalization;
using DG.Tweening;

public class MainMenuPanelController : Controller
{
	private MainMenuPanelModel 		_mainMenuPanelModel			{ get { return ui.model.mainMenuPanelModel; } }

	private bool					_isHourGiftActive			= false;

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.GameAddScore:
				{
					break;
				}

			case N.PurchaseProductsLoaded:
				{
					InitStorePrices ();
					break;
				}

			case N.OnStartShowAdVideo:
				{
					break;
				}

			case N.OnEndShowAdVideo_:
				{
					
					break;
				}

			case N.PlayerItemCountChange__:
				{
					//ItemTypes itemType = (ItemTypes)data [0];
					//int count = (int)data [1];

					UpdateLeftStatistics ();
					break;
				}

			case N.GameOver_:
				{
					//GameOverData gameOverData = (GameOverData)data[0];

					OnGameOver ();
					break;
				}
		}
	}

	private void OnStart()
	{
		//TODO: Update statistics with pop animation.
		RegisterEvents();
		UpdateLeftStatistics ();
		InitStorePrices ();

	}

	private void RegisterEvents()
	{
		LanguageManager.Instance.OnChangeLanguage += OnLanguageChanged;
	}

	private void OnLanguageChanged(LanguageManager langManager)
	{
		InitStorePrices ();
	}
		
	private void OnGameOver()
	{
		UpdateLeftStatistics ();
	}

	private void UpdateLeftStatistics()
	{
		_mainMenuPanelModel.textRecord.text = string.Format("{0}", Utils.SweetMoney(game.model.playerRecord));
		_mainMenuPanelModel.textCoinsCount.text = string.Format("{0}", Utils.SweetMoney(game.model.coinsCount));
		_mainMenuPanelModel.textCrystalsCount.text = string.Format("{0}", Utils.SweetMoney(game.model.crystalsCount));

		Utils.RebuildLayoutGroups (_mainMenuPanelModel.textCoinsCount.transform.parent.parent.GetComponent<RectTransform>());
	}

	private void InitStorePrices()
	{
		if (AndroidInAppPurchaseManager.Client.IsInventoryLoaded)
		{
			_mainMenuPanelModel.textDoubleCoin.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_DOUBLE_COIN_NAME").ToUpper (), AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails (PurchaseController.DOUBLE_COIN).LocalizedPrice);
			_mainMenuPanelModel.textCoinsPack_00.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_COINS_PACK_00_NAME").ToUpper (), AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails (PurchaseController.COINS_PACK_00).LocalizedPrice);
			_mainMenuPanelModel.textCoinsPack_01.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_COINS_PACK_01_NAME").ToUpper (), AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails (PurchaseController.COINS_PACK_01).LocalizedPrice);
		}
		else
		{
			_mainMenuPanelModel.textDoubleCoin.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_DOUBLE_COIN_NAME").ToUpper(), Localization.CheckKey("TK_DOUBLE_COIN_PRICE_NAME"));
			_mainMenuPanelModel.textCoinsPack_00.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_COINS_PACK_00_NAME").ToUpper (),  Localization.CheckKey("TK_COINS_PACK_00_PRICE_NAME"));
			_mainMenuPanelModel.textCoinsPack_01.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_COINS_PACK_01_NAME").ToUpper (),  Localization.CheckKey("TK_COINS_PACK_01_PRICE_NAME"));

		}
	}
}
