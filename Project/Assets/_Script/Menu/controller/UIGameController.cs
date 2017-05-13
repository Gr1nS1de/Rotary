using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using DG.Tweening;

public class UIGameController : Controller
{
	private UIGameModel UIGameModel				{ get { return ui.model.UIGameModel; } }
	//private RoadModel 	currentRoadModel		{ get { return game.model.currentGearView; } }

	public override void OnNotification ( string alias, Object target, params object[] data )
	{
		switch (alias)
		{
			case N.GameStartPlay:
				{
					//InitScoreBarItems ();
					//InitScoreBarSlider ();

					break;
				}

			/*case N.GameAddScore:
				{
					int score = (int) data[0];

					OnAddScore (score);
					CheckGetItem ();

					break;
				}*/
		}
	}
	/*
	private void InitScoreBarItems ()
	{
		int currentRoadScore = Utils.GetRoadScore (game.model.currentRoad);

		UIGameModel.itemsDictionaryByScore.Clear ();
		
		foreach (ItemsScoreData itemScore in currentRoadModel.itemsScoreData)
		{
			int itemScoreCount = itemScore.scoreCount;

			//If score for reach item is = 0  - it's empty slot.  
			if (itemScoreCount == 0)
				continue;

			GameObject itemGO = Instantiate( UIGameModel.itemSpotPrefab);
			RectTransform itemRectTranform = itemGO.GetComponent<RectTransform> ();
			Image itemImage = itemGO.transform.Find ("item").GetComponent<Image> ();
			Image itemLineImage = itemGO.transform.Find ("line").GetComponent<Image> ();

			itemGO.transform.SetParent (UIGameModel.itemSpotsContainer.transform);

			SetScoreItemPositionByScoreCount (itemRectTranform, itemScoreCount);
				
			itemImage.sprite = itemScore.sprite;

			//If player alrdy got the item - deactivate it on score tab.
			if (currentRoadScore > itemScoreCount)
			{
				DeactivateScoreItem (itemGO);
				continue;
			}

			UIGameModel.itemsDictionaryByScore.Add (itemScoreCount, itemGO);
		}
	}

	private void SetScoreItemPositionByScoreCount (RectTransform item, int itemScoreCount )
	{
		int scoreToFinish = currentRoadModel.scoreToFinish;
		float itemsContainerWidth = UIGameModel.itemSpotsContainer.GetComponent<RectTransform> ().rect.width;
		Vector2 itemPosition = item.anchoredPosition;

		itemPosition.x = (itemScoreCount/ (float)scoreToFinish ) * itemsContainerWidth;

		if (itemPosition.x < itemsContainerWidth / 2f)
			itemPosition.x = -(itemsContainerWidth / 2f - itemPosition.x);
		else
			itemPosition.x -= itemsContainerWidth / 2f;

		itemPosition.y = 30f;

		item.anchoredPosition = itemPosition;
	}

	private void InitScoreBarSlider()
	{
		Slider scoreSlider = UIGameModel.scoreSlider;
		int scoreToFinish = currentRoadModel.scoreToFinish;

		scoreSlider.maxValue = scoreToFinish;

		scoreSlider.value = Utils.GetRoadScore (currentRoadModel.alias);
	}

	private void OnAddScore(int score)
	{
		UIGameModel.scoreText.text = game.model.currentScore.ToString();
		UIGameModel.scoreSlider.value += score;

		Utils.AddRoadScore (currentRoadModel.alias, score);
	}

	private void CheckGetItem()
	{
		int sliderValue = (int)UIGameModel.scoreSlider.value;

		foreach (ItemsScoreData itemScore in currentRoadModel.itemsScoreData)
		{
			int itemScoreCount = itemScore.scoreCount;

			//If score for reach item is = 0  - it's empty slot.  
			if (itemScoreCount == 0)
				continue;

			//If item not in dictionary - it's already received.
			if (!UIGameModel.itemsDictionaryByScore.ContainsKey (itemScoreCount))
				return;

			//If player achieved score for receive item.
			if (sliderValue >= itemScoreCount)
			{
				GameObject itemGO = UIGameModel.itemsDictionaryByScore [itemScoreCount];
				DeactivateScoreItem (itemGO);
				UIGameModel.itemsDictionaryByScore.Remove (itemScoreCount);
				break;
			}
		}
	}

	private void DeactivateScoreItem(GameObject item)
	{
		Image itemImage = item.transform.Find ("item").GetComponent<Image> ();
		Image itemLineImage = item.transform.Find ("line").GetComponent<Image> ();

		itemImage.color = UIGameModel.itemDeactivatedColor;
		itemLineImage.enabled = false;
	}*/

}
