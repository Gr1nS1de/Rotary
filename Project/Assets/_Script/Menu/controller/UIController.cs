﻿using UnityEngine;
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

			case N.GameStart:
				{
					GoToMainState (UIPanelState.InGame);
					break;
				}

			case N.GameOver_:
				{
					//GameOverData gameOverData = (GameOverData)data[0];

					GoToMainState (UIPanelState.MainMenu);
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

		Notify (N.UIThemeChanged_, NotifyType.UI, menuTheme);
	}

	public void GoToWindowState(UIWindowState uiState)
	{
		//Debug.LogFormat ("GoToState {0}", uiState);

		if (ui.model.uiState == uiState)
			return;

		DisableAllWindows ();

		//Wait for disable all windows	
		DOVirtual.DelayedCall (0.15f, () =>
		{
			SetWindowActive(uiState, true);
		});

		ui.model.uiState = uiState;

		Notify (N.UIStateChanged_, NotifyType.UI, uiState);
	}
	#endregion

	private void GoToMainState(UIPanelState uiMainState)
	{
		switch (uiMainState)
		{
			case UIPanelState.MainMenu:
				{
					SetActivePanel (true, ui.model.mainMenuPanel);
					SetActivePanel (false, ui.model.inGamePanel);
					break;
				}

			case UIPanelState.InGame:
				{
					SetActivePanel (false, ui.model.mainMenuPanel);
					SetActivePanel (true, ui.model.inGamePanel);
					break;
				}
		}
	}

	private void SetActivePanel(bool isActive, CanvasGroup panelCanvas)
	{
		float enableTime = 0.15f;
		float enableValue = isActive ? 1f : 0f;

		if (isActive)
		{
			panelCanvas.alpha = 0f;

			if (!panelCanvas.gameObject.activeInHierarchy)
				panelCanvas.gameObject.SetActive (isActive);

			panelCanvas.DOFade (enableValue, enableTime);
		}
		else
		{
			if (panelCanvas.gameObject.activeInHierarchy)
			{
				panelCanvas.DOFade (enableValue, enableTime)
					.OnComplete (() =>
					{
						panelCanvas.gameObject.SetActive (isActive);
					});
			}
		}
	}

	//Скрываем все дочерние окна из набора UIState
	private void DisableAllWindows()
	{
		foreach(string stateName in System.Enum.GetNames (typeof(UIWindowState)))
		{
			UIWindowState uiState = (UIWindowState)System.Enum.Parse(typeof(UIWindowState), stateName);

			SetWindowActive(uiState, false);
		}
	}

	//Метод для скрытия/открытия окон UIState
	private void SetWindowActive(UIWindowState uiState, bool isActive)
	{
		float enableTime = 0.15f;
		float enableValue = isActive ? 1f : 0f;
		CanvasGroup windowCanvas = null;

		switch (uiState)
		{
			case UIWindowState.Store:
				{
					windowCanvas = ui.model.storeWindow;
					break;
				}

			case UIWindowState.Settings:
				{
					windowCanvas = ui.model.settingsWindow;
					break;
				}

			case UIWindowState.PlayerSkin:
				{
					windowCanvas = ui.model.playerSkinWindow;
					break;
				}

			case UIWindowState.MainMenu:
				{
					windowCanvas = ui.model.mainMenuWindow;
					break;
				}

			case UIWindowState.Like:
				{
					windowCanvas = ui.model.likeWindow;
					break;
				}

			case UIWindowState.DailyGift:
				{
					windowCanvas = ui.model.dailyGiftWindow;
					break;
				}
					
			case UIWindowState.Pause:
				{
					windowCanvas = ui.model.pauseWindow;
					break;
				}

			case UIWindowState.GameOver:
				{
					windowCanvas = ui.model.gameOverWindow;
					break;
				}

			case UIWindowState.Achievements:
				{
					windowCanvas = ui.model.achievementsWindow;
					break;
				}

			default:
				{
					//Debug.LogFormat ("There is no state for {0}", uiState);
					break;
				}
		}

		if (windowCanvas != null)
		{
			if (isActive)
			{
				windowCanvas.alpha = 0f;

				if (!windowCanvas.gameObject.activeInHierarchy)
					windowCanvas.gameObject.SetActive (isActive);
				
					windowCanvas.DOFade (enableValue, enableTime);
			}
			else
			{
				if (windowCanvas.gameObject.activeInHierarchy)
				{
					windowCanvas.DOFade (enableValue, enableTime)
						.OnComplete (() =>
						{
							windowCanvas.gameObject.SetActive (isActive);
						});
				}
			}
		}
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
