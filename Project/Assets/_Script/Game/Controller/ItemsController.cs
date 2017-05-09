using UnityEngine;
using System.Collections;

public class ItemsController : Controller
{
	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}
				/*
			case N.DestructibleBreakEntity___:
				{
					var obstacleDestructible = (D2dDestructible)data [0];
					var fractureCount = (int)data [1];
					var collisionPoint = (Vector2)data [2];

					BreakEntity( obstacleDestructible, fractureCount, collisionPoint);

					break;
				}*/

			case N.PlayerImpactItem__:
				{
					ItemView itemView = (ItemView)data [0];
					Vector3 contactPoint = (Vector3)data [1];

					OnImpactItem (itemView, contactPoint);
					break;
				}

		}
	}

	private void OnStart()
	{

	}

	private void OnImpactItem(ItemView itemView, Vector3 contactPoint)
	{
		ItemTypes itemType = itemView.ItemType;

		switch (itemType)
		{
			case ItemTypes.COIN:
				{
					
					break;
				}

			case ItemTypes.DIMOND:
				{
					//Notify (N.DestructibleBreakEntity___, NotifyType.GAME, destructibleItem, itemView.DistructFractureCount, collision.contacts [0].point);
					game.controller.distructibleController.BreakEntity(itemView.DimondRenderer, itemView.DistructFractureCount, contactPoint);
					break;
				}

			case ItemTypes.MAGNET:
				{
					break;
				}
		}
	}


}

