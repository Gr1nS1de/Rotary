﻿using UnityEngine;
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

					StartItemCountUpdateAnimation (ItemTypes.Coin, giftCoinsCount);
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
		if (_leftStatisticsTabCountingSequence != null && _leftStatisticsTabCountingSequence.IsActive ())
			_leftStatisticsTabCountingSequence.Kill ();

		_leftStatisticsTabCountingSequence = DOTween.Sequence ();

		_leftStatisticsTabCountingSequence
			.Append (_mainMenuPanelModel.textLastScore.DOFade (1f, 0.5f))
			.Join (_mainMenuPanelModel.textLastScore.transform.DOPunchScale (new Vector3 (0.2f, 0.2f, 0f), 0.5f, 1));

		if (gameOverData.ScoreCount > 0)
		{
			_mainMenuPanelModel.textLastScore.text = string.Format("{0}: <size=22>{1}</size>", Localization.CheckKey("TK_SCORE_NAME").ToUpper(), 0);

			_leftStatisticsTabCountingSequence
				.Append(DOVirtual.Float (0f, (float)gameOverData.ScoreCount, Mathf.Clamp(gameOverData.ScoreCount % 10, 1f, 3f), (val ) =>
				{
					int intVal = (int)val;	

					_mainMenuPanelModel.textLastScore.text = string.Format("{0}: <size=22>{1}</size>", Localization.CheckKey("TK_SCORE_NAME").ToUpper(), Utils.SweetMoney(intVal));
				
					if(int.Parse(_mainMenuPanelModel.textRecord.text) < intVal)
					{
						_mainMenuPanelModel.textRecord.text = string.Format("{0}", Utils.SweetMoney(intVal));
					}
				
				}).SetEase(Ease.Linear))
				.Append(_mainMenuPanelModel.textLastScore.GetComponent<RectTransform>().DOShakeAnchorPos(0.3f, new Vector2(10f, 0f), 5, 180f))
				.AppendInterval(0.5f);
		}

		if (gameOverData.CoinsCount > 0)
		{
			StartItemCountUpdateAnimation (ItemTypes.Coin, gameOverData.CoinsCount, _leftStatisticsTabCountingSequence);
		}

		if (gameOverData.CrystalsCount > 0)
		{
			StartItemCountUpdateAnimation (ItemTypes.Crystal, gameOverData.CrystalsCount, _leftStatisticsTabCountingSequence);
		}
	}

	private void StartItemCountUpdateAnimation(ItemTypes itemType, int count, Sequence countingSequence = null)
	{
		if (countingSequence == null)
			countingSequence = DOTween.Sequence ();

		switch (itemType)
		{
			case ItemTypes.Coin:
				{
					RectTransform textCoinsAddRectTransform = _mainMenuPanelModel.textCoinsAdd.GetComponent<RectTransform> ();
					Vector2 initTextPosition = textCoinsAddRectTransform.anchoredPosition;
					Vector2 moveOffset = new Vector2 (0f, 20f);

					_mainMenuPanelModel.textCoinsAdd.text = string.Format("+{0}", Utils.SweetMoney(count));
					textCoinsAddRectTransform.anchoredPosition = initTextPosition + moveOffset;

					countingSequence
						.Append(textCoinsAddRectTransform.DOAnchorPosY(initTextPosition.y, 0.5f))
						.Join(_mainMenuPanelModel.textCoinsAdd.DOFade(1f, 0.5f))
						.Append(DOVirtual.Float ((float)count, 0f, Mathf.Clamp(count % 10, 1f, 2f), (val ) =>
						{
							int intVal = (int)val;

							_mainMenuPanelModel.textCoinsAdd.text = string.Format("+{0}", Utils.SweetMoney(intVal));
							_mainMenuPanelModel.textCoinsCount.text = string.Format("{0}", _playerDataModel.coinsCount - intVal);

							_mainMenuPanelModel.imageCoinIcon.GetComponent<RectTransform>()
								.DOPunchScale(new Vector3(0.1f, 0.1f), 0.01f, 1)
								.OnComplete(()=>
								{
									_mainMenuPanelModel.imageCoinIcon.GetComponent<RectTransform>().localScale = Vector3.one;
								});
						}).SetEase(Ease.Linear))
						.Append(textCoinsAddRectTransform.DOAnchorPosY(initTextPosition.y - moveOffset.y, 0.3f))
						.Join(_mainMenuPanelModel.textCoinsAdd.DOFade(0f, 0.3f))
						.AppendCallback (() =>
						{
							textCoinsAddRectTransform.anchoredPosition = initTextPosition;
						});
					break;
				}

			case ItemTypes.Crystal:
				{
					RectTransform textCrystalsAddRectTransform = _mainMenuPanelModel.textCrystalsAdd.GetComponent<RectTransform> ();
					Vector2 initTextPosition = textCrystalsAddRectTransform.anchoredPosition;
					Vector2 moveOffset = new Vector2 (0f, 20f);

					_mainMenuPanelModel.textCrystalsAdd.text = string.Format("+{0}", Utils.SweetMoney(count));
					textCrystalsAddRectTransform.anchoredPosition = initTextPosition + moveOffset;

					countingSequence
						.Append (textCrystalsAddRectTransform.DOAnchorPosY (initTextPosition.y, 0.5f))
						.Join (_mainMenuPanelModel.textCrystalsAdd.DOFade (1f, 0.5f))
						.Append (DOVirtual.Float ((float)count, 0f, Mathf.Clamp (count % 10, 1f, 2f), (val ) =>
						{
							int intVal = (int)val;

							_mainMenuPanelModel.textCrystalsAdd.text = string.Format ("+{0}", Utils.SweetMoney (intVal));
							_mainMenuPanelModel.textCrystalsCount.text = string.Format ("{0}", _playerDataModel.crystalsCount - intVal);

							_mainMenuPanelModel.imageCrystalIcon.GetComponent<RectTransform> ()
								.DOPunchScale (new Vector3 (0.1f, 0.1f), 0.01f, 1)
								.OnComplete (() =>
								{
									_mainMenuPanelModel.imageCrystalIcon.GetComponent<RectTransform> ().localScale = Vector3.one;
								});
						}).SetEase (Ease.Linear))
						.Append (textCrystalsAddRectTransform.DOAnchorPosY (initTextPosition.y - moveOffset.y, 0.5f))
						.Join (_mainMenuPanelModel.textCrystalsAdd.DOFade (0f, 0.5f))
						.AppendCallback (() =>
						{
							textCrystalsAddRectTransform.anchoredPosition = initTextPosition;
						});
					break;
				}
		}
	}
		
}
