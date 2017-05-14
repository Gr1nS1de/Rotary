using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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
					Vector2 contactPoint = (Vector2)data [1];

					OnImpactItem (itemView, contactPoint);
					break;
				}

		}
	}

	private void OnStart()
	{

	}

	private void OnImpactItem(ItemView itemView, Vector2 contactPoint)
	{
		ItemTypes itemType = itemView.ItemType;

		itemView.OnPlayerImpact ();

		switch (itemType)
		{
			case ItemTypes.Coin:
				{
					
					break;
				}

			case ItemTypes.Crystal:
				{
					//Notify (N.DestructibleBreakEntity___, NotifyType.GAME, destructibleItem, itemView.DistructFractureCount, collision.contacts [0].point);
					game.controller.distructibleController.BreakEntity(itemView.DimondRenderer, itemView.DistructFractureCount, contactPoint);
					break;
				}

			case ItemTypes.Magnet:
				{
					break;
				}
		}
	}


}

