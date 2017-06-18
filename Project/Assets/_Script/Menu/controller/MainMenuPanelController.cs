using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SmartLocalization;
using DG.Tweening;

public class MainMenuPanelController : Controller
{
	private MainMenuPanelModel 		_mainMenuPanelModel			{ get { return ui.model.mainMenuPanelModel; } }

	private bool					_isHourGiftActive			= false;
	private bool					_isStorePricesInited		= false;
	private Tween					_dailyGiftBowTieTween		= null;

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

			case N.PurchaseProductsLoaded_:
				{
					bool isSuccess = (bool)data[0];

					InitStorePrices (isSuccess);
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

			case N.OnDailyGiftAvailable_:
				{
					bool isAvailable = (bool)data [0];

					SetDailyGiftAvailable (isAvailable);
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

	}

	private void RegisterEvents()
	{
		LanguageManager.Instance.OnChangeLanguage += OnLanguageChanged;
	}

	private void OnLanguageChanged(LanguageManager langManager)
	{
		OnUpdateStorePricesLang ();
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

	private void SetDailyGiftAvailable(bool isGiftAvailable)
	{
		if (_dailyGiftBowTieTween == null)
		{
			_dailyGiftBowTieTween = ui.model.mainMenuPanelModel.imageDailyGiftBowTie.transform
				.DOPunchScale (new Vector3 (0.1f, 0.1f, 0f), 0.5f, 1)
				.SetAutoKill(false)
				.SetRecyclable(true)
				.SetLoops(-1);
		}

		if (isGiftAvailable)
		{
			_dailyGiftBowTieTween.Play ();
			ui.model.mainMenuPanelModel.imageDailyGiftBowTie.DOFade (1f, 0.3f);
		}
		else
		{
			_dailyGiftBowTieTween.Rewind ();
			ui.model.mainMenuPanelModel.imageDailyGiftBowTie.DOFade (0f, 0.3f);
		}
	}

	private void InitStorePrices(bool isSuccessConnection)
	{
		if (isSuccessConnection)
		{
			_mainMenuPanelModel.textDoubleCoin.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_DOUBLE_COIN_NAME").ToUpper (), AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails (PurchaseController.DOUBLE_COIN).LocalizedPrice);
			_mainMenuPanelModel.textCoinsPack_00.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_COINS_PACK_00_NAME").ToUpper (), AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails (PurchaseController.COINS_PACK_00).LocalizedPrice);
			_mainMenuPanelModel.textCoinsPack_01.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_COINS_PACK_01_NAME").ToUpper (), AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails (PurchaseController.COINS_PACK_01).LocalizedPrice);

			_isStorePricesInited = true;
		}
		else
		{
			OnUpdateStorePricesLang ();
		}

	}

	private void OnUpdateStorePricesLang()
	{
		if (_isStorePricesInited)
		{
			string[] doubleCoinTextSplitted = _mainMenuPanelModel.textDoubleCoin.text.Split ('-');
			string[] coinsPack_00TextSplitted = _mainMenuPanelModel.textCoinsPack_00.text.Split ('-');
			string[] coinsPack_01TextSplitted = _mainMenuPanelModel.textCoinsPack_01.text.Split ('-');

			_mainMenuPanelModel.textDoubleCoin.text = string.Format ("{0} -{1}", Localization.CheckKey ("TK_DOUBLE_COIN_NAME").ToUpper (), doubleCoinTextSplitted [1]);
			_mainMenuPanelModel.textCoinsPack_00.text = string.Format ("{0} -{1}", Localization.CheckKey ("TK_COINS_PACK_00_NAME").ToUpper (), coinsPack_00TextSplitted [1]);
			_mainMenuPanelModel.textCoinsPack_01.text = string.Format ("{0} -{1}", Localization.CheckKey ("TK_COINS_PACK_01_NAME").ToUpper (), coinsPack_01TextSplitted [1]);
		}
		else
		{
			_mainMenuPanelModel.textDoubleCoin.text = string.Format ("{0} - :(", Localization.CheckKey ("TK_DOUBLE_COIN_NAME").ToUpper());
			_mainMenuPanelModel.textCoinsPack_00.text = string.Format ("{0} - :(", Localization.CheckKey ("TK_COINS_PACK_00_NAME").ToUpper ());
			_mainMenuPanelModel.textCoinsPack_01.text = string.Format ("{0} - ;(", Localization.CheckKey ("TK_COINS_PACK_01_NAME").ToUpper ());
		}
	}
}
