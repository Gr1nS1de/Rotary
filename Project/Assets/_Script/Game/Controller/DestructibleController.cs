using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Destructible2D;

public class DestructibleController : Controller
{
	private DestructibleModel _destructibleModel	{ get { return game.model.destructibleModel; } }

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.DestructibleBreakEntity___:
				{
					var obstacleDestructible = (D2dDestructible)data [0];
					var fractureCount = (int)data [1];
					var collisionPoint = (Vector2)data [2];

					BreakEntity( obstacleDestructible, fractureCount, collisionPoint);

					break;
				}

		}
	}

	private void OnStart()
	{}

	private void BreakEntity( D2dDestructible destructible, int fractureCount, Vector2 collisionPoint = default(Vector2))
	{
		// Store explosion point (used in OnEndSplit)
		if (collisionPoint == Vector2.zero)
			_destructibleModel.entityBreakPoint = destructible.transform.position;
		else
			_destructibleModel.entityBreakPoint = collisionPoint;

		destructible.gameObject.layer = LayerMask.NameToLayer ("DestroyedItem");

		// Register split event
		destructible.OnEndSplit.AddListener((piecesList)=>
		{
			OnEndSplit(destructible, piecesList);
		});

		// Split via fracture
		D2dQuadFracturer.Fracture(destructible, fractureCount, 0.3f);
	}

	private void OnEndSplit(D2dDestructible destructible, List<D2dDestructible> piecesList)
	{
		//destructible.GetComponent<D2dDestroyer> ().enabled = true;
		//Utils.ActivateTransformChildrens (destructible.transform, false);
		
		// Go through all clones in the clones list
		for (var i = piecesList.Count - 1; i >= 0; i--)
		{
			var clone = piecesList[i];
			var rigidbody = clone.GetComponent<Rigidbody2D>();

			rigidbody.bodyType = RigidbodyType2D.Dynamic;
			clone.GetComponent<D2dDestroyer> ().enabled = true;

			// Does this clone have a Rigidbody2D?
			if (rigidbody != null)
			{
				// Get the local point of the explosion that called this split event
				var localPoint = (Vector2)clone.transform.InverseTransformPoint(_destructibleModel.entityBreakPoint);

				// Get the vector between this point and the center of the destructible's current rect
				var vector = clone.AlphaRect.center - localPoint;

				//var force = ( game.model.gameState == GameState.GAMEOVER ? game.model.playerModel.breakForce : game.model.destructibleModel.breakForce );

				// Apply relative force
				rigidbody.AddRelativeForce(vector * _destructibleModel.breakForce, ForceMode2D.Impulse);
			}
		}
	}

}