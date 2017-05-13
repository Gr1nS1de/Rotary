using UnityEngine;
using System.Collections;

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

			case N.GameStartPlay:
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
		float randomNum = Random.value;

		if (scoreCount > 5)
		{
			if (randomNum > 0.8f)
			{
				game.controller.objectsPoolController.PoolObject (PoolingObjectType.ITEM, 1, null, ItemTypes.Crystal);
			}
		}
		else if (scoreCount > 15)
		{
			if (randomNum > 0.7f)
			{
				game.controller.objectsPoolController.PoolObject (PoolingObjectType.ITEM, 1, null, ItemTypes.Crystal);
			}
		}
		else if (scoreCount > 25)
		{
			if (randomNum > 0.6f)
			{
				game.controller.objectsPoolController.PoolObject (PoolingObjectType.ITEM, 1, null, ItemTypes.Crystal);
			}
		}
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

