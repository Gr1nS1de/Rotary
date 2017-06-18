using UnityEngine;
using System.Collections;
using SmartLocalization;
using DG.Tweening;

public class CenterElementsController : Controller
{
	public System.Action<UITheme> ActionUIThemeChanged = delegate{};

	private MainMenuPanelModel 		_mainMenuPanelModel			{ get { return ui.model.mainMenuPanelModel; } }

	private Tween					_dailyGiftBowTieTween		= null;
	private bool					_isStorePricesInited		= false;

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();
					break;
				}

			case N.OnCenterButtonPressed_:
				{
					CenterElementId centerElementId = (CenterElementId)data [0];

					OnButtonPressed (centerElementId);
					break;
				}

			case N.OnDailyGiftAvailable_:
				{
					bool isAvailable = (bool)data [0];

					SetDailyGiftAvailable (isAvailable);
					break;
				}

			case N.OnPurchasedDoubleCoin:
				{
					OnUpdateStorePricesLang ();
					break;
				}

			case N.PurchaseProductsLoaded_:
				{
					bool isSuccess = (bool)data[0];

					InitStorePrices (isSuccess);
					break;
				}

			case N.UIThemeChanged_:
				{
					UITheme uiTheme = (UITheme)data [0];

					ActionUIThemeChanged (uiTheme);
					break;
				}
		}
	}

	private void OnStart()
	{
		RegisterEvents();

	}

	private void RegisterEvents()
	{
		LanguageManager.Instance.OnChangeLanguage += OnLanguageChanged;
	}

	private void OnLanguageChanged(LanguageManager langManager)
	{
		OnUpdateStorePricesLang ();
	}
		
	private void OnButtonPressed(CenterElementId centerElementId)
	{
		switch (centerElementId)
		{
			case CenterElementId.Main_ButtonPlay:
				{
					Notify (N.GameStart);

					break;
				}

			case CenterElementId.Settings_ButtonCoinStore:
				{
					ui.controller.GoToWindowState (UIWindowState.Store);
					break;
				}

			case CenterElementId.Like_ButtonDailyGift:
				{
					ui.controller.GoToWindowState (UIWindowState.DailyGift);
					break;
				}

			case CenterElementId.ButtonShowRewardVideo:
				{
					ui.controller.RewardVideoController.ShowRewardedAd ();
					break;
				}

			case CenterElementId.Like_ButtonRate:
				{
					
					break;
				}

			case CenterElementId.Settings_ButtonLanguage:
				{
					Localization.InitLanguage(LanguageManager.Instance.CurrentlyLoadedCulture.languageCode == "ru" ? "en" : "ru");	
					break;
				}

			case CenterElementId.Settings_ButtonMusic:
				{
					
					break;
				}

			case CenterElementId.Settings_ButtonSFX:
				{
					break;
				}

			case CenterElementId.Store_ButtonCoinsPack_00:
				{
					Notify (N.PurchaseCoinsPack_00);
					break;
				}

			case CenterElementId.Store_ButtonCoinsPack_01:
				{
					Notify (N.PurchaseCoinsPack_01);
					break;
				}

			case CenterElementId.Store_ButtonDoubleCoin:
				{
					if(!core.playerDataModel.isDoubleCoin)
						Notify (N.PurchaseDoubleCoin);
					break;
				}
		}
	}

	private void SetDailyGiftAvailable(bool isGiftAvailable)
	{
		if (_dailyGiftBowTieTween == null)
		{
			_dailyGiftBowTieTween = ui.model.mainMenuPanelModel.imageDailyGiftBowTie.transform
				.DOPunchScale (new Vector3 (0.3f, 0.3f, 0f), 0.5f, 1)
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
			_isStorePricesInited = true;
		}
		else
		{
			
		}

		OnUpdateStorePricesLang ();
	}

	private void OnUpdateStorePricesLang()
	{
		if (_isStorePricesInited)
		{
			string[] doubleCoinTextSplitted = _mainMenuPanelModel.textDoubleCoin.text.Split ('-');
			string[] coinsPack_00TextSplitted = _mainMenuPanelModel.textCoinsPack_00.text.Split ('-');
			string[] coinsPack_01TextSplitted = _mainMenuPanelModel.textCoinsPack_01.text.Split ('-');

			_mainMenuPanelModel.textDoubleCoin.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_DOUBLE_COIN_NAME").ToUpper (), AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails (PurchaseController.DOUBLE_COIN).LocalizedPrice);
			_mainMenuPanelModel.textCoinsPack_00.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_COINS_PACK_00_NAME").ToUpper (), AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails (PurchaseController.COINS_PACK_00).LocalizedPrice);
			_mainMenuPanelModel.textCoinsPack_01.text = string.Format ("{0} - {1}", Localization.CheckKey ("TK_COINS_PACK_01_NAME").ToUpper (), AndroidInAppPurchaseManager.Client.Inventory.GetProductDetails (PurchaseController.COINS_PACK_01).LocalizedPrice);
		}
		else
		{
			_mainMenuPanelModel.textDoubleCoin.text = string.Format ("{0} - :(", Localization.CheckKey ("TK_DOUBLE_COIN_NAME").ToUpper());
			_mainMenuPanelModel.textCoinsPack_00.text = string.Format ("{0} - :(", Localization.CheckKey ("TK_COINS_PACK_00_NAME").ToUpper ());
			_mainMenuPanelModel.textCoinsPack_01.text = string.Format ("{0} - ;(", Localization.CheckKey ("TK_COINS_PACK_01_NAME").ToUpper ());
		}

		if (core.playerDataModel.isDoubleCoin)
		{
			_mainMenuPanelModel.textDoubleCoin.text = string.Format ("{0}", Localization.CheckKey ("TK_DOUBLE_COIN_NAME").ToUpper ());

			if(_mainMenuPanelModel.panelIsDoubleCoinBoughtMark.alpha < 1f)
				_mainMenuPanelModel.panelIsDoubleCoinBoughtMark.DOFade (1f, 0.5f);
		}
	}
}

