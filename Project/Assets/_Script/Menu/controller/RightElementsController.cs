using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class RightElementsController : Controller
{
	public System.Action<UITheme>	 ActionUIThemeChanged	= delegate{};

	private Stack<UIWindowState> 	_backButtonStack 		= new Stack<UIWindowState> ();
	private Sequence				_rightPanelMoveSequence = null;
	private Sequence				_gameServicesOpenSequence = null;
	private Sequence				_gameServicesCloseSequence = null;
	private bool					_isGameServicesIconColorInited = false;
	private Color					_gameServicesGreenColor = new Color(77f/ 255f, 161f / 255f, 70f/255f, 1f);
	private Tween					_likeButtonTween = null;
	private Vector2					_initRightPanelPosition;

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();
					break;
				}

			case N.OnRightButtonPressed_:
				{
					RightElementId rightElementId = (RightElementId)data [0];

					OnButtonPressed (rightElementId);
					break;
				}

			case N.UIThemeChanged_:
				{
					UITheme uiTheme = (UITheme)data [0];

					//Send theme changed event to all RightElenentView
					ActionUIThemeChanged (uiTheme);

					if (!_isGameServicesIconColorInited || GooglePlayConnection.State != GPConnectionState.STATE_CONNECTED)
					{
						SetElementIconColor (RightElementId.ButtonGameServices, uiTheme.IconsColor);
						_isGameServicesIconColorInited = true;
					}
					break;
				}

			case N.UIWindowStateChanged_:
				{
					UIWindowState uiState = (UIWindowState)data [0];

					UpdateRightButtons (uiState);
					UpdateBackStack (uiState);
					break;
				}

			case N.OnGameServicesConnected_:
				{
					bool isSuccess = (bool)data [0];

					SetElementIconColor (RightElementId.ButtonGameServices, isSuccess ? _gameServicesGreenColor : ui.model.menuTheme.IconsColor);
					break;
				}

			case N.OnDailyGiftAvailable_:
				{
					bool isAvailable = (bool)data [0];

					SetLikeButtonDailyGiftAvailable (isAvailable);
					break;
				}
		}
	}

	private void OnStart()
	{
		RectTransform rightPanelRect = ui.model.rightElementsPanel.GetComponent<RectTransform> ();

		_initRightPanelPosition = rightPanelRect.anchoredPosition;
		 
		ui.model.mainMenuPanelModel.isGameServicesOpened = false;
		UpdateRightButtons (ui.model.uiWindowState);
	}

	private void MoveRightPanel(System.Action callbackHidden)
	{
		RectTransform rightPanelRect = ui.model.rightElementsPanel.GetComponent<RectTransform> ();

		if (_rightPanelMoveSequence != null && _rightPanelMoveSequence.IsActive ())
			_rightPanelMoveSequence.Kill ();
		
		_rightPanelMoveSequence = DOTween.Sequence ();

		_rightPanelMoveSequence
			.Append (rightPanelRect.DOPunchAnchorPos (new Vector2 (-rightPanelRect.rect.width, 0f), 0.5f, 0, 1f))
			.InsertCallback (0.08f, () =>
			{
				if (callbackHidden != null)
					callbackHidden ();
			})
			.OnComplete (() =>
			{
				rightPanelRect.anchoredPosition = _initRightPanelPosition;		
			}).OnKill(()=>
			{
				rightPanelRect.anchoredPosition = _initRightPanelPosition;
			});
		
			
		_rightPanelMoveSequence.Play ();
	}

	private void OnButtonPressed(RightElementId rightElementId)
	{
		UIWindowState uiState = UIWindowState.MainMenu;
		bool isChangeWindowState = true;
		bool isMoveRightPanel = false;

		if (_backButtonStack.Count == 0 && rightElementId != RightElementId.ButtonBack && rightElementId != RightElementId.ButtonGameServices)
		{
			isMoveRightPanel = true;
		}

		switch (rightElementId)
		{
			case RightElementId.ButtonStore:
				{
					uiState = UIWindowState.Store;
					break;
				}

			case RightElementId.ButtonSettings:
				{
					uiState = UIWindowState.Settings;
					break;
				}

			case RightElementId.ButtonPlayerSkin:
				{
					uiState = UIWindowState.PlayerSkin;
					break;
				}

			case RightElementId.ButtonLike:
				{
					uiState = UIWindowState.Like;
					break;
				}

		#region Google Play Services
			case RightElementId.ButtonGameServices:
				{
					isChangeWindowState = false;
					if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
					{
						if(ui.model.mainMenuPanelModel.isGameServicesOpened)
						{
							CloseGameServices();
						}else
						{
							OpenGameServices();
						}
					}else{
						SetElementIconColor(RightElementId.ButtonGameServices, _gameServicesGreenColor, ()=>
						{
							GooglePlayConnection.Instance.Connect();
							SetElementIconColor(RightElementId.ButtonGameServices, ui.model.menuTheme.IconsColor);
						});
					}
					break;
				}

			case RightElementId.ButtonGSAchievements:
				{
					isChangeWindowState = false;
					break;
				}

			case RightElementId.ButtonLeaderboard:
				{
					isChangeWindowState = false;
					break;
				}
		#endregion

			case RightElementId.ButtonBack:
				{
					OnBackPressed ();

					if (_backButtonStack.Count > 0)
					{
						uiState = _backButtonStack.Peek ();
					}
					else
					{
						uiState = UIWindowState.MainMenu;
						isMoveRightPanel = true;
					}
					break;
				}
		}

		if (isChangeWindowState)
		{
			if (!isMoveRightPanel)
			{
				ui.model.mainMenuPanelModel.isGameServicesOpened = false;
				ui.controller.GoToWindowState (uiState);
			}
			else
			{
				MoveRightPanel (()=>
				{
					ui.model.mainMenuPanelModel.isGameServicesOpened = false;
					ui.controller.GoToWindowState (uiState);
				});
			}
		}
	}

	private void SetLikeButtonDailyGiftAvailable (bool isGiftAvailable)
	{
		if (_likeButtonTween == null)
		{
			_likeButtonTween = ui.view.GetRightElement (RightElementId.ButtonLike).ElementIconRenderer.transform
				.DOPunchPosition (new Vector3 (0f, 3f, 0f), 0.28f, 1)
				.SetAutoKill(false)
				.SetRecyclable(true)
				.SetLoops(-1);
		}

		if (isGiftAvailable)
			_likeButtonTween.Play ();
		else
			_likeButtonTween.Rewind ();
	}

	private void SetElementIconColor(RightElementId elementId, Color color, System.Action onComplete = null)
	{
		Graphic iconRenderer = ui.view.GetRightElement (RightElementId.ButtonGameServices).ElementIconRenderer;

		if (iconRenderer != null)
			iconRenderer.DOColor(color, 0.5f)
				.OnComplete(()=>
				{
					if(onComplete != null)
						onComplete();
				});
		else
			Debug.LogErrorFormat ("SetElementIconColor. There is no icon for {0}", elementId);
	}

	private void OpenGameServices()
	{
		RightElementView leaderboardView = ui.view.GetRightElement (RightElementId.ButtonLeaderboard);
		RightElementView achievementsView = ui.view.GetRightElement (RightElementId.ButtonGSAchievements);

		if (!leaderboardView.gameObject.activeInHierarchy)
			leaderboardView.gameObject.SetActive (true);

		if (!achievementsView.gameObject.activeInHierarchy)
			achievementsView.gameObject.SetActive (true);
		
		if(_gameServicesOpenSequence == null)
		{
			_gameServicesOpenSequence = DOTween.Sequence();

			_gameServicesOpenSequence.SetRecyclable(true).SetAutoKill(false);
			_gameServicesOpenSequence
				.Append (leaderboardView.GetComponent<RectTransform> ().DOAnchorPosX (-160f, 0.3f))
				.Join (achievementsView.GetComponent<RectTransform> ().DOAnchorPosX (-320f, 0.3f))
				.Join (leaderboardView.GetComponent<CanvasGroup> ().DOFade (1f, 0.3f))
				.Join (achievementsView.GetComponent<CanvasGroup> ().DOFade (1f, 0.3f));
		}else{
			if (_gameServicesOpenSequence.IsActive ())
				_gameServicesOpenSequence.Rewind ();
			
			if (_gameServicesCloseSequence != null && _gameServicesCloseSequence.IsActive ())
				_gameServicesCloseSequence.Rewind ();
		}

		leaderboardView.GetComponent<CanvasGroup> ().alpha = 0f;
		achievementsView.GetComponent<CanvasGroup> ().alpha = 0f;
			

		ui.model.mainMenuPanelModel.isGameServicesOpened = true;
		_gameServicesOpenSequence.Play ();
	}

	private void CloseGameServices()
	{
		RightElementView leaderboardView = ui.view.GetRightElement (RightElementId.ButtonLeaderboard);
		RightElementView achievementsView = ui.view.GetRightElement (RightElementId.ButtonGSAchievements);

		if (!leaderboardView.gameObject.activeInHierarchy)
			leaderboardView.gameObject.SetActive (true);

		if (!achievementsView.gameObject.activeInHierarchy)
			achievementsView.gameObject.SetActive (true);

		if(_gameServicesCloseSequence == null)
		{
			_gameServicesCloseSequence = DOTween.Sequence();

			_gameServicesCloseSequence.SetRecyclable(true).SetAutoKill(false);
			_gameServicesCloseSequence
				.Append (leaderboardView.GetComponent<RectTransform> ().DOAnchorPosX (0f, 0.3f))
				.Join (achievementsView.GetComponent<RectTransform> ().DOAnchorPosX (0f, 0.3f))
				.Join (leaderboardView.GetComponent<CanvasGroup> ().DOFade (0f, 0.3f))
				.Join (achievementsView.GetComponent<CanvasGroup> ().DOFade (0f, 0.3f))
				.OnComplete(()=>{
					leaderboardView.gameObject.SetActive (false);
					achievementsView.gameObject.SetActive (false);
				});
		}else{
			if (_gameServicesOpenSequence.IsActive ())
				_gameServicesOpenSequence.Rewind ();

			if (_gameServicesCloseSequence != null && _gameServicesCloseSequence.IsActive ())
				_gameServicesCloseSequence.Rewind ();
		}

		ui.model.mainMenuPanelModel.isGameServicesOpened = false;
		_gameServicesCloseSequence.Play ();
	}

	private void UpdateRightButtons(UIWindowState uiState)
	{
		switch (uiState)
		{
			case UIWindowState.MainMenu:
				{
					Enable4Buttons (RightElementId.ButtonSettings, RightElementId.ButtonPlayerSkin, RightElementId.ButtonLike, RightElementId.ButtonGameServices);
					break;
				}

			case UIWindowState.Settings:
				{
					Enable4Buttons (RightElementId.ButtonBack, RightElementId.ButtonPlayerSkin, RightElementId.ButtonLike, RightElementId.ButtonStore);
					break;
				}

			case UIWindowState.PlayerSkin:
				{
					Enable4Buttons (RightElementId.ButtonBack, RightElementId.ButtonSettings, RightElementId.ButtonLike, RightElementId.ButtonStore);
					break;
				}

			case UIWindowState.Like:
				{
					Enable4Buttons (RightElementId.ButtonBack, RightElementId.ButtonPlayerSkin, RightElementId.ButtonStore, RightElementId.ButtonSettings);
					break;
				}

			case UIWindowState.Store:
				{
					Enable4Buttons (RightElementId.ButtonBack, RightElementId.ButtonPlayerSkin, RightElementId.ButtonLike, RightElementId.ButtonSettings);
					break;
				}
		}
	}

	private void Enable4Buttons(RightElementId button1, RightElementId button2, RightElementId button3, RightElementId button4)
	{
		foreach (RightElementView rightElementView in ui.view.rightElementsArray)
		{

			if (rightElementView.ElementId == button1 || rightElementView.ElementId == button2 || rightElementView.ElementId == button3 || rightElementView.ElementId == button4)
			{
				if (rightElementView.ElementId == button1)
					rightElementView.transform.SetSiblingIndex (0);
				else if(rightElementView.ElementId == button2)
					rightElementView.transform.SetSiblingIndex (1);
				else if(rightElementView.ElementId == button3)
					rightElementView.transform.SetSiblingIndex (3);
				else if(rightElementView.ElementId == button4)
					rightElementView.transform.SetSiblingIndex (4);

				SetButtonActive (rightElementView, true);
			}
			else
				SetButtonActive (rightElementView, false);
		}
	}

	private void SetButtonActive(RightElementView rightElementView, bool isActive)
	{
		if (isActive)
		{
			if (!rightElementView.gameObject.activeInHierarchy)
				rightElementView.gameObject.SetActive (isActive);
		}
		else
		{
			if (rightElementView.gameObject.activeInHierarchy )
				rightElementView.gameObject.SetActive (isActive);
		}
	}

	private void OnBackPressed()
	{
		if (_backButtonStack.Count <= 1)
		{
			ClearBackStack ();
		}
		else if (_backButtonStack.Count > 1)
		{
			_backButtonStack.Pop ();
		}

	}

	private void UpdateBackStack(UIWindowState uiState)
	{

		switch (uiState)
		{
			case UIWindowState.Store:
			case UIWindowState.Settings:
			case UIWindowState.PlayerSkin:
			case UIWindowState.Like:
				{
					if (_backButtonStack.Count == 0)
					{
						PushToBackStack (uiState);
					}
					else if (_backButtonStack.Count == 1)
					{
						PushToBackStack (uiState, true);
					}
					else
					{
						ClearBackStack ();
						PushToBackStack (uiState);
					}
					break;
				}

			case UIWindowState.Achievements:
				{
					PushToBackStack (uiState);
					break;
				}

			case UIWindowState.DailyGift:
				{
					PushToBackStack (uiState);
					break;
				}

			default:
				{
					break;
				}
		}
	}

	private void PushToBackStack (UIWindowState uiState, bool isPopLast = false)
	{
		if (isPopLast)
		{
			_backButtonStack.Pop();
		}
		
		_backButtonStack.Push (uiState);
	}

	private void ClearBackStack()
	{
		_backButtonStack.Clear ();
	}
}

