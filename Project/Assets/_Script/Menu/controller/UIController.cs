using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : Controller
{
	#region Declare controllers reference
	public UIGameController			UIGameController				{ get { return _UIGameController 		= SearchLocal<UIGameController>(		_UIGameController,			typeof(UIGameController).Name);	} }
	public UIMainMenuController		UIMenuController				{ get { return _UIMenuController		= SearchLocal<UIMainMenuController>(	_UIMenuController,			typeof(UIMainMenuController).Name);	} }
	public CenterButtonsController	CenterButtonsController			{ get { return _centerButtonsController	= SearchLocal<CenterButtonsController>(	_centerButtonsController,	typeof(CenterButtonsController).Name);	} }
	public RightButtonsController	RightButtonsController			{ get { return _rightButtonsController	= SearchLocal<RightButtonsController>(	_rightButtonsController,	typeof(RightButtonsController).Name);	} }

	private RightButtonsController	_rightButtonsController;
	private CenterButtonsController	_centerButtonsController;
	private UIGameController		_UIGameController;
	private UIMainMenuController	_UIMenuController;
	#endregion

	private MainMenuModel	UIMenuModel	{ get { return ui.model.UIMainMenuModel; } }
	private UIGameModel 	UIGameModel	{ get { return ui.model.UIGameModel; } }

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();
					break;
				}

			case N.GameStartPlay:
				{
					ui.model.uiState = UIState.InGame;
					break;
				}

			case N.GamePause:
				{
					ui.model.uiState = UIState.Pause;
					break;
				}

			case N.GameOver_:
				{
					GameOverData gameOverData = (GameOverData)data[0];

					ui.model.uiState = UIState.GameOver;
					break;
				}
		}
	}

	void OnStart()
	{
	}

	#region public methods
	public void SetMenuTheme(UITheme menuTheme)
	{
		ui.model.menuTheme = menuTheme;

		Notify (N.UIThemeChanged_, NotifyType.GAME, menuTheme);
	}

	public void GoToState(UIState uiState)
	{
		if (ui.model.uiState == uiState)
			return;

		DisableAllWindows ();

		DOVirtual.DelayedCall (0.15f, () =>
		{
			SetWindowActive(uiState, true);
		});

		ui.model.uiState = uiState;
	}
	#endregion

	private void DisableAllWindows()
	{
		foreach(string stateName in System.Enum.GetNames (typeof(UIState)))
		{
			UIState uiState = (UIState)System.Enum.Parse(typeof(UIState), stateName);

			SetWindowActive(uiState, false);
		}
	}

	private void SetWindowActive(UIState uiState, bool isActive)
	{
		float enableTime = 0.15f;
		float enableValue = isActive ? 1f : 0f;
		CanvasGroup windowCanvas = null;

		switch (uiState)
		{
			case UIState.Store:
				{
					windowCanvas = ui.model.storeWindow;
					break;
				}

			case UIState.Settings:
				{
					windowCanvas = ui.model.settingsWindow;
					break;
				}

			case UIState.PlayerSkin:
				{
					windowCanvas = ui.model.playerSkinWindow;
					break;
				}

			case UIState.MainMenu:
				{
					windowCanvas = ui.model.mainMenuWindow;
					break;
				}

			case UIState.Like:
				{
					windowCanvas = ui.model.likeWindow;
					break;
				}

			case UIState.DailyGift:
				{
					windowCanvas = ui.model.dailyGiftWindow;
					break;
				}

			case UIState.InGame:
				{
					windowCanvas = ui.model.inGameWindow;
					break;
				}

			case UIState.Pause:
				{
					windowCanvas = ui.model.pauseWindow;
					break;
				}

			case UIState.GameOver:
				{
					windowCanvas = ui.model.gameOverWindow;
					break;
				}

			case UIState.Achievements:
				{
					windowCanvas = ui.model.achievementsWindow;
					break;
				}
		}

		if (windowCanvas != null && (windowCanvas.alpha != 1f && isActive || windowCanvas.alpha != 0f && !isActive))
			windowCanvas.DOFade (enableValue, enableTime);
	}
		
	void UpdateText()
	{
	}

	public void OnStartGame(System.Action complete)
	{/*
		UpdateText();

		UIGameModel.canvasGroupInGame.DOFade(0,0.01f)
			.SetEase(Ease.Linear);

		UIMenuModel.canvasGroupStart.DOFade(0,0.5f)
			.SetDelay(0.2f)
			.SetEase(Ease.Linear)
			.OnComplete(() => {

				UIMenuModel.canvasGroupStart.alpha = 0;

				UIMenuModel.canvasGroupStart.gameObject.SetActive(false);

				UIGameModel.canvasGroupInGame.DOFade(1f,1).SetEase(Ease.Linear);

				if(complete!=null)
					complete();
			});*/
	}

	public void OnGameOver(System.Action complete)
	{
		//UpdateText();

		//UIMenuModel.canvasGroup.gameObject.SetActive(true);
		/*
		UIGameModel.canvasGroupInGame.DOFade(0,0.2f).SetEase(Ease.Linear);

		UIMenuModel.canvasGroupStart.DOFade(1f,1).SetEase(Ease.Linear).OnComplete(() => {

			UIMenuModel.canvasGroupStart.alpha = 1;

			//DOTween.KillAll();

			if(complete!=null)
				complete();
		});*/
	}
		
}
