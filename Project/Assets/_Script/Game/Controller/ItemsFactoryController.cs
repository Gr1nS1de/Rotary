using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemsFactoryController : Controller
{

	public ItemsFactoryModel _itemsFactoryModel	{ get { return game.model.itemsFactoryModel; } } 

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

			case N.OnItemInvisible_:
				{
					ItemView itemView = (ItemView)data [0];

					if(game.model.gameState == GameState.Playing)
						RestoreItem (itemView);
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

	private void CheckItemSpawn(PlatformView platformView)
	{
		int scoreCount = game.model.currentScore;
		float randomItemSpawn = Random.value;

		if (scoreCount >= 3 && scoreCount < 15)
		{
			if (randomItemSpawn <= 0.2f)
			{
				game.controller.objectsPoolController.PoolObject (PoolingObjectType.ITEM, 1, null, GetRandomItem());
			}
		}
		else if (scoreCount >= 15 && scoreCount < 25)
		{
			if (randomItemSpawn <= 0.3f)
			{
				game.controller.objectsPoolController.PoolObject (PoolingObjectType.ITEM, 1, null, GetRandomItem());
			}
		}
		else if (scoreCount >= 25)
		{
			if (randomItemSpawn <= 0.6f)
			{
				game.controller.objectsPoolController.PoolObject (PoolingObjectType.ITEM, 1, null, GetRandomItem());
			}
		}
	}

	private ItemTypes GetRandomItem()
	{
		ItemTypes randomItemType = ItemTypes.Coin;
		string[] itemNames = System.Enum.GetNames (typeof(ItemTypes));
		List<float> itemsChances = new List<float>();
		int scoreCount = game.model.currentScore;

		//Debug.LogFormat ("1. Get random item. Game speed state: {0}", game.model.gameSpeedState);

		for (int i = 0; i < itemNames.Length; i++) 
		{
			switch ((ItemTypes)System.Enum.Parse(typeof(ItemTypes), itemNames[i])) 
			{
				case ItemTypes.Coin:
					{
						int coinChance = GetCoinRandomChance ();

						itemsChances.Add(coinChance);

						//Debug.LogFormat ("2. Add coin chance: {0}", coinChance);
						break;
					}

				case ItemTypes.Crystal:
					{
						int crystalChance = GetCrystalRandomChance ();

						itemsChances.Add(crystalChance);

						//Debug.LogFormat ("3. Add crystal chance: {0}", crystalChance);
						break;
					}

				case ItemTypes.Magnet:
					{
						int magnetChance = GetMagnetRandomChance ();

						itemsChances.Add(magnetChance);

						//Debug.LogFormat ("4. Add magnet chance: {0}", magnetChance);
						break;
					}
			} 
		}

		int choosedItemIndex = ChooseRandomItem (itemsChances);

		randomItemType = (ItemTypes)System.Enum.Parse (typeof(ItemTypes), itemNames [choosedItemIndex]);

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

		return randomPercentChance;

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


	private void RestoreItem(ItemView itemView)
	{
		if (game.controller.objectsPoolController.IsValidPoolingObject (itemView))
			game.controller.objectsPoolController.StoreObjectToPool(PoolingObjectType.ITEM, itemView);
		else
		{
			Destroy (itemView.gameObject);
		}
	}
}

