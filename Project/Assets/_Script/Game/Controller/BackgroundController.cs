using UnityEngine;
using System.Collections;

[System.Serializable]
public class MovingSprite
{
	public  float LoopMoveTime;
	public SpriteRenderer ItemRenderer;
	public float CurrentSizeX;
}

public class BackgroundController : Controller
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

			case N.GameThemeChanged_:
				{
					OnGameThemeChanged ();
					break;
				}
		}
	}

	private void OnStart()
	{
		
	}

	private void OnGameThemeChanged()
	{
		if (game.view.backgroundView != null)
			Destroy (game.view.backgroundView.gameObject);

		BackgroundView backgroundView = (BackgroundView)Instantiate (game.model.gameTheme.BackgroundView, game.view.cameraView.transform);//.cameraView.transform);
	
		if (!backgroundView.gameObject.activeInHierarchy)
			backgroundView.gameObject.SetActive (true);

		//backgroundView.Init ();
	}
}

