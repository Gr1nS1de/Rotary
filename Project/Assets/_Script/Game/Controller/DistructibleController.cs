using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Destructible2D;

public class DistructibleController : Controller
{
	private DistructibleModel _distructibleModel	{ get { return game.model.distructibleModel; } }

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

		}
	}

	private void OnStart()
	{}

	public void BreakItem(ItemType itemType, D2dDestructible destructible, int fractureCount, Vector2 collisionPoint = default(Vector2))
	{
		// Store explosion point (used in OnEndSplit)
		if (collisionPoint == Vector2.zero || collisionPoint ==  default(Vector2))
			_distructibleModel.entityBreakPoint = destructible.transform.position;
		else
			_distructibleModel.entityBreakPoint = collisionPoint;

		switch(itemType)
		{
			case ItemType.Crystal:
				{
					destructible.gameObject.layer = LayerMask.NameToLayer ("DestroyedItem");

					// Register split event
					destructible.OnEndSplit.AddListener ((piecesList ) =>
					{
						OnEndSplit (destructible, piecesList);
					});

					// Split via fracture
					D2dQuadFracturer.Fracture (destructible, fractureCount, 0.3f);
					break;
				}
		}
	}

	private void OnEndSplit(D2dDestructible destructible, List<D2dDestructible> piecesList)
	{
		//destructible.GetComponent<D2dDestroyer> ().enabled = true;
		//Utils.ActivateTransformChildrens (destructible.transform, false);

		ItemView itemView = destructible.transform.parent.GetComponent<ItemView> ();
		ItemType itemType = itemView.ItemType;
		
		// Go through all clones in the clones list

		for (var i = piecesList.Count - 1; i >= 0; i--)
		{
			var clone = piecesList[i];
			var rigidbody = clone.GetComponent<Rigidbody2D>();
			var d2Destroyer = clone.GetComponent<D2dDestroyer> ();

			switch(itemType)
			{
				case ItemType.Crystal:
					{
						rigidbody.bodyType = RigidbodyType2D.Dynamic;

						d2Destroyer.Life = itemView.CrystalDestroyTime;
						d2Destroyer.FadeDuration = itemView.CrystalDestroyTime / 2f;
						d2Destroyer.enabled = true;

						//Reset all unity colliders on clones
						if (clone.GetComponent<Collider2D> ())
							foreach (Collider2D collider in clone.GetComponents<Collider2D>())
							{
								
								StartCoroutine (ResetCollider(clone.transform, collider));
							}

						// Does this clone have a Rigidbody2D?
						if (rigidbody != null)
						{
							// Get the local point of the explosion that called this split event
							var localPoint = (Vector2)clone.transform.InverseTransformPoint (_distructibleModel.entityBreakPoint);

							// Get the vector between this point and the center of the destructible's current rect
							var vector = clone.AlphaRect.center - localPoint;

							//var force = ( game.model.gameState == GameState.GAMEOVER ? game.model.playerModel.breakForce : game.model.destructibleModel.breakForce );

							// Apply relative force
							rigidbody.AddRelativeForce (vector * _distructibleModel.breakForce, ForceMode2D.Impulse);
						}
						break;
					}
			}
		}
	}

	private System.Collections.IEnumerator ResetCollider(Transform clone, Collider2D collider)
	{
		System.Type colliderType = collider.GetType ();

		Destroy (collider);

		yield return null;

		Collider2D colliderComponent = (Collider2D)clone.gameObject.AddComponent(colliderType);

		colliderComponent.isTrigger = false;
	}

}