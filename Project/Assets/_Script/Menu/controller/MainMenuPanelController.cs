using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SmartLocalization;
using DG.Tweening;

public class MainMenuPanelController : Controller
{
	private MainMenuPanelModel 		_mainMenuPanelModel					{ get { return ui.model.mainMenuPanelModel; } }
	private PlayerDataModel 		_playerDataModel					{ get { return core.playerDataModel;}}

	private Sequence				_leftStatisticsTabCountingSequence 	= null;
	private Vector2					_initAddCoinsTextPosition;
	private Vector2					_initAddCrystalsTextPosition;

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

			case N.OnPlayerBuySkin_:
			case N.OnPurchasedCoinsPack_00:
			case N.OnPurchasedCoinsPack_01:
			case N.OnPurchasedDoubleCoin:
				{
					InitLeftStatistics ();
					break;
				}

			case N.OnPlayerGetDailyGift__:
				{
					DailyGiftElementId dailyGiftElementId = (DailyGiftElementId)data [0];
					int giftCoinsCount = (int)data [1];

					StartItemCountUpdateAnimation (ItemType.Coin, giftCoinsCount);
					break;
				}

			case N.GameOver_:
				{
					GameOverData gameOverData = (GameOverData)data[0];

					OnGameOver (gameOverData);
					break;
				}
		}
	}

	private void OnStart()
	{
		_initAddCoinsTextPosition = _mainMenuPanelModel.textCoinsAdd.GetComponent<RectTransform> ().anchoredPosition;
		_initAddCrystalsTextPosition = _mainMenuPanelModel.textCrystalsAdd.GetComponent<RectTransform> ().anchoredPosition;

		//TODO: Update statistics with pop animation.
		InitLeftStatistics ();
	}
		
	private void OnGameOver(GameOverData gameOverData)
	{
		StartLeftStatisticsBarGameOverAnimation (gameOverData);
	}

	private void InitLeftStatistics()
	{
		_mainMenuPanelModel.textRecord.text = string.Format("{0}", Utils.SweetMoney(_playerDataModel.playerRecord));
		_mainMenuPanelModel.textCoinsCount.text = string.Format("{0}", Utils.SweetMoney(_playerDataModel.coinsCount));
		_mainMenuPanelModel.textCrystalsCount.text = string.Format("{0}", Utils.SweetMoney(_playerDataModel.crystalsCount));

		Utils.RebuildLayoutGroups (_mainMenuPanelModel.textCoinsCount.transform.parent.parent.GetComponent<RectTransform>());
	}

	private void StartLeftStatisticsBarGameOverAnimation(GameOverData gameOverData)
	{
		//Hide new record text
		_mainMenuPanelModel.textNewRecord.DOFade (0f, 0.1f);

		if (_leftStatisticsTabCountingSequence != null && _leftStatisticsTabCountingSequence.IsActive ())
			_leftStatisticsTabCountingSequence.Kill ();

		_leftStatisticsTabCountingSequence = DOTween.Sequence ();

		_leftStatisticsTabCountingSequence
			.AppendInterval(0.2f)
			.Append (_mainMenuPanelModel.textLastScore.DOFade (1f, 0.5f))
			.Join (_mainMenuPanelModel.textLastScore.transform.DOPunchScale (new Vector3 (0.2f, 0.2f, 0f), 0.5f, 1))
			.AppendInterval(0.5f);

		if (gameOverData.ScoreCount > 0)
		{
			_mainMenuPanelModel.textLastScore.text = string.Format("{0}: <size=25>{1}</size>", Localization.CheckKey("TK_SCORE_NAME").ToUpper(), gameOverData.ScoreCount);

			if(int.Parse(_mainMenuPanelModel.textRecord.text) < gameOverData.ScoreCount)
			{
				_mainMenuPanelModel.textRecord.text = string.Format("{0}", Utils.SweetMoney(gameOverData.ScoreCount));
				_leftStatisticsTabCountingSequence
					.Append(_mainMenuPanelModel.textNewRecord.DOFade (1f, 0.5f))
					.Join (_mainMenuPanelModel.textNewRecord.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 1f, 3).SetEase(Ease.Flash))
					.AppendInterval(0.5f);
			}
			/*
			_leftStatisticsTabCountingSequence
				.Append(DOVirtual.Float (0f, (float)gameOverData.ScoreCount, Mathf.Clamp(gameOverData.ScoreCount % 10, 0.5f, 2f), (val ) =>
				{
					int intVal = (int)val;	

					_mainMenuPanelModel.textLastScore.text = string.Format("{0}: <size=25>{1}</size>", Localization.CheckKey("TK_SCORE_NAME").ToUpper(), Utils.SweetMoney(intVal));
				
					if(int.Parse(_mainMenuPanelModel.textRecord.text) < intVal)
					{
						_mainMenuPanelModel.textRecord.text = string.Format("{0}", Utils.SweetMoney(intVal));
					}
				
				}).SetEase(Ease.Linear))
				.Append(_mainMenuPanelModel.textLastScore.GetComponent<RectTransform>().DOShakeAnchorPos(0.3f, new Vector2(10f, 0f), 5, 180f))
				.AppendInterval(0.5f);
				*/
		}

		if (gameOverData.CoinsCount > 0)
		{
			StartItemCountUpdateAnimation (ItemType.Coin, gameOverData.CoinsCount, _leftStatisticsTabCountingSequence);
		}

		if (gameOverData.CrystalsCount > 0)
		{
			StartItemCountUpdateAnimation (ItemType.Crystal, gameOverData.CrystalsCount, _leftStatisticsTabCountingSequence);
		}
	}

	private void StartItemCountUpdateAnimation(ItemType itemType, int count, Sequence countingSequence = null)
	{
		if (countingSequence == null)
		{
			if (DOTween.IsTweening (this))
				DOTween.Kill (this);
			
			countingSequence = DOTween.Sequence ();

			countingSequence.SetId (this);
		}

		switch (itemType)
		{
			case ItemType.Coin:
				{
					RectTransform textCoinsAddRectTransform = _mainMenuPanelModel.textCoinsAdd.GetComponent<RectTransform> ();
					Vector2 moveOffset = new Vector2 (0f, 20f);

					_mainMenuPanelModel.textCoinsAdd.text = string.Format("+{0}", Utils.SweetMoney(count));
					textCoinsAddRectTransform.anchoredPosition = _initAddCoinsTextPosition + moveOffset;

					countingSequence
						.Append(textCoinsAddRectTransform.DOAnchorPosY(_initAddCoinsTextPosition.y, 0.5f))
						.Join(_mainMenuPanelModel.textCoinsAdd.DOFade(1f, 0.5f))
						.AppendInterval(0.5f)
						.Append(DOVirtual.Float ((float)count, 0f, Mathf.Clamp(count % 10, 0.5f, 1.5f), (val ) =>
						{
							int intVal = (int)val;

							_mainMenuPanelModel.textCoinsAdd.text = string.Format("+{0}", Utils.SweetMoney(intVal));
							_mainMenuPanelModel.textCoinsCount.text = string.Format("{0}", Utils.SweetMoney(_playerDataModel.coinsCount - intVal));

							Utils.RebuildLayoutGroups (_mainMenuPanelModel.textCoinsCount.transform.parent.parent.GetComponent<RectTransform>());

						}).SetEase(Ease.Linear))
						.Append(textCoinsAddRectTransform.DOAnchorPosY(_initAddCoinsTextPosition.y - moveOffset.y, 0.3f))
						.Join(_mainMenuPanelModel.textCoinsAdd.DOFade(0f, 0.3f))
						.AppendCallback (() =>
						{
							textCoinsAddRectTransform.anchoredPosition = _initAddCoinsTextPosition;
						}).OnKill(()=>
						{
							_mainMenuPanelModel.textCoinsCount.text = string.Format("{0}", Utils.SweetMoney(_playerDataModel.coinsCount));
						});
					break;
				}

			case ItemType.Crystal:
				{
					RectTransform textCrystalsAddRectTransform = _mainMenuPanelModel.textCrystalsAdd.GetComponent<RectTransform> ();
					Vector2 moveOffset = new Vector2 (0f, 20f);

					_mainMenuPanelModel.textCrystalsAdd.text = string.Format("+{0}", Utils.SweetMoney(count));
					textCrystalsAddRectTransform.anchoredPosition = _initAddCrystalsTextPosition + moveOffset;

					countingSequence
						.Append (textCrystalsAddRectTransform.DOAnchorPosY (_initAddCrystalsTextPosition.y, 0.5f))
						.Join (_mainMenuPanelModel.textCrystalsAdd.DOFade (1f, 0.5f))
						.AppendInterval(0.5f)
						.Append (DOVirtual.Float ((float)count, 0f, Mathf.Clamp (count % 10, 0.5f, 1.5f), (val ) =>
						{
							int intVal = (int)val;

							_mainMenuPanelModel.textCrystalsAdd.text = string.Format ("+{0}", Utils.SweetMoney (intVal));
							_mainMenuPanelModel.textCrystalsCount.text = string.Format ("{0}", Utils.SweetMoney(_playerDataModel.crystalsCount - intVal));

							Utils.RebuildLayoutGroups (_mainMenuPanelModel.textCoinsCount.transform.parent.parent.GetComponent<RectTransform>());
						}).SetEase (Ease.Linear))
						.Append (textCrystalsAddRectTransform.DOAnchorPosY (_initAddCrystalsTextPosition.y - moveOffset.y, 0.5f))
						.Join (_mainMenuPanelModel.textCrystalsAdd.DOFade (0f, 0.5f))
						.AppendCallback (() =>
						{
							textCrystalsAddRectTransform.anchoredPosition = _initAddCrystalsTextPosition;
						}).OnKill(()=>
						{
							_mainMenuPanelModel.textCrystalsCount.text = string.Format ("{0}", Utils.SweetMoney(_playerDataModel.crystalsCount));
						});
					break;
				}
		}
	}
		
}
