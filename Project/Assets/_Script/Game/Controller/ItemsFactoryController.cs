using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemsFactoryController : Controller
{

	public ItemsFactoryModel _itemsFactoryModel	{ get { return game.model.itemsFactoryModel; } } 

	private int 			_itemsNotSpawnedRow = 0;

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
					OnGamePlay ();
					break;
				}

			case N.OnPlatformInvisible_:
				{
					PlatformView platformView = (PlatformView)data [0];

					CheckItemSpawn (platformView);
					break;
				}

		}
	}

	private void OnStart()
	{

	}

	private void OnGamePlay()
	{
	}

	//Check item spawn on platform invisible
	private void CheckItemSpawn(PlatformView platformView)
	{
		int scoreCount = core.playerDataModel.currentScore;
		float randomItemSpawn = Random.value;

		_itemsNotSpawnedRow++;


		if (scoreCount >= 0 && scoreCount <= 3)
		{
			if (Random.Range (0, 4) == 0)
				GoPoolItem (ItemType.Crystal);
			else if( Random.Range(0,11) < 7)
				GoPoolItem( ItemType.Magnet) ;
			else if(Random.Range(0,3) == 0)
				GoPoolItem(GetRandomItem());
			else
			{
				//Spawn nothing :)
			}
		}

		if (scoreCount > 3 && scoreCount < 15)
		{
			if (randomItemSpawn <= 0.2f)
			{
				GoPoolItem(GetRandomItem());
			}
		}
		else if (scoreCount >= 15 && scoreCount < 25)
		{
			if (randomItemSpawn <= 0.3f)
			{
				GoPoolItem(GetRandomItem());
			}
		}
		else if (scoreCount >= 25)
		{
			if (randomItemSpawn <= 0.6f)
			{
				GoPoolItem(GetRandomItem());
			}
		}

		if (_itemsNotSpawnedRow >= Random.Range (3, 5))
		{
			ItemType penaltyItem = GetRandomPenaltyItem ();
			GoPoolItem (penaltyItem);
		}
	}

	private void GoPoolItem(ItemType itemType, int count = 1)
	{
		_itemsNotSpawnedRow = 0;

		game.controller.objectsPoolController.PoolObject (PoolingObjectType.ITEM, count, null, itemType);
	}

	private ItemType GetRandomPenaltyItem()
	{
		ItemType randomPenaltyItemType = ItemType.Coin;
		string[] itemNames = System.Enum.GetNames (typeof(ItemType));
		List<float> itemsChances = new List<float>();
		int scoreCount = core.playerDataModel.currentScore;

		//Debug.LogFormat ("1. Get random item. Game speed state: {0}", game.model.gameSpeedState);

		for (int i = 0; i < itemNames.Length; i++) 
		{
			switch ((ItemType)System.Enum.Parse(typeof(ItemType), itemNames[i])) 
			{
				case ItemType.Coin:
					{
						int coinChance = 5;

						itemsChances.Add(coinChance);

						//Debug.LogFormat ("2. Add coin chance: {0}", coinChance);
						break;
					}

				case ItemType.Crystal:
					{
						int crystalChance = 10;

						itemsChances.Add(crystalChance);

						//Debug.LogFormat ("3. Add crystal chance: {0}", crystalChance);
						break;
					}

				case ItemType.Magnet:
					{
						int magnetChance = 85;

						itemsChances.Add(magnetChance);

						//Debug.LogFormat ("4. Add magnet chance: {0}", magnetChance);
						break;
					}
			} 
		}

		int choosedItemIndex = ChooseRandomItem (itemsChances);

		randomPenaltyItemType = (ItemType)System.Enum.Parse (typeof(ItemType), itemNames [choosedItemIndex]);

		//Debug.LogFormat ("5. Go pool {0}", randomItemType);

		return randomPenaltyItemType;
	}

	private ItemType GetRandomItem()
	{
		ItemType randomItemType = ItemType.Coin;
		string[] itemNames = System.Enum.GetNames (typeof(ItemType));
		List<float> itemsChances = new List<float>();
		int scoreCount = core.playerDataModel.currentScore;

		//Debug.LogFormat ("1. Get random item. Game speed state: {0}", game.model.gameSpeedState);

		for (int i = 0; i < itemNames.Length; i++) 
		{
			switch ((ItemType)System.Enum.Parse(typeof(ItemType), itemNames[i])) 
			{
				case ItemType.Coin:
					{
						int coinChance = GetCoinRandomChance ();

						itemsChances.Add(coinChance);

						//Debug.LogFormat ("2. Add coin chance: {0}", coinChance);
						break;
					}

				case ItemType.Crystal:
					{
						int crystalChance = GetCrystalRandomChance ();

						itemsChances.Add(crystalChance);

						//Debug.LogFormat ("3. Add crystal chance: {0}", crystalChance);
						break;
					}

				case ItemType.Magnet:
					{
						int magnetChance = GetMagnetRandomChance ();

						itemsChances.Add(magnetChance);

						//Debug.LogFormat ("4. Add magnet chance: {0}", magnetChance);
						break;
					}
			} 
		}

		int choosedItemIndex = ChooseRandomItem (itemsChances);

		randomItemType = (ItemType)System.Enum.Parse (typeof(ItemType), itemNames [choosedItemIndex]);

		//Debug.LogFormat ("5. Go pool {0}", randomItemType);

		return randomItemType;
	}

	private int GetCoinRandomChance()
	{
		int randomPercentChance = 0;

		switch (game.model.gameSpeedState)
		{
			case GameSpeedState.Speed_1:
				{
					randomPercentChance = 10;
					break;
				}

			case GameSpeedState.Speed_2:
				{
					randomPercentChance = 20;
					break;
				}

			case GameSpeedState.Speed_3:
				{
					randomPercentChance = 25;
					break;
				}

			case GameSpeedState.Speed_4:
				{
					randomPercentChance = 30;
					break;
				}

			case GameSpeedState.Speed_5:
				{
					randomPercentChance = 40;
					break;
				}
			case GameSpeedState.Speed_6:
				{	
					randomPercentChance = 45;
					break;
				}

			case GameSpeedState.Speed_7:
				{	
					randomPercentChance = 50;
					break;
				}
		}

		return randomPercentChance;
	}

	private int GetCrystalRandomChance()
	{
		int randomPercentChance = 0;

		switch (game.model.gameSpeedState)
		{
			case GameSpeedState.Speed_1:
				{
					randomPercentChance = 80;
					break;
				}

			case GameSpeedState.Speed_2:
				{
					randomPercentChance = 65;
					break;
				}

			case GameSpeedState.Speed_3:
				{
					randomPercentChance = 55;
					break;
				}

			case GameSpeedState.Speed_4:
				{
					randomPercentChance = 50;
					break;
				}

			case GameSpeedState.Speed_5:
				{
					randomPercentChance = 40;
					break;
				}
			case GameSpeedState.Speed_6:
				{	
					randomPercentChance = 45;
					break;
				}

			case GameSpeedState.Speed_7:
				{	
					randomPercentChance = 50;
					break;
				}
		}

		return randomPercentChance;
	}

	private int GetMagnetRandomChance()
	{
		int randomPercentChance = 0;

		//return randomPercentChance;

		switch (game.model.gameSpeedState)
		{
			case GameSpeedState.Speed_1:
				{
					randomPercentChance = 10;
					break;
				}

			case GameSpeedState.Speed_2:
				{
					randomPercentChance = 15;
					break;
				}

			case GameSpeedState.Speed_3:
				{
					randomPercentChance = 20;
					break;
				}

			case GameSpeedState.Speed_4:
				{
					randomPercentChance = 20;
					break;
				}

			case GameSpeedState.Speed_5:
				{
					randomPercentChance = 20;
					break;
				}
			case GameSpeedState.Speed_6:
				{	
					randomPercentChance = 10;
					break;
				}

			case GameSpeedState.Speed_7:
				{	
					randomPercentChance = 0;
					break;
				}
		}

		return randomPercentChance;
	}

	private int ChooseRandomItem (List<float> itemsProbs) 
	{
		float total = 0;

		foreach (float elem in itemsProbs) 
		{
			total += elem;
		}

		float randomPoint = Random.value * total;

		for (int i= 0; i < itemsProbs.Count; i++) 
		{
			if (randomPoint < itemsProbs[i]) 
			{
				return i;
			}
			else 
			{
				randomPoint -= itemsProbs[i];
			}
		}

		return itemsProbs.Count - 1;
	}
}

