using UnityEngine;
using System.Collections;
//using DG.Tweening;

public class CameraController : Controller
{
	private CameraModel 	_cameraModel		{ get { return game.model.cameraModel; } }
	private CameraView	 	_cameraView			{ get { return game.view.cameraView; } }

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.GamePlay:
				{
					InitCamera ();
					break;
				}

		}
	 }

	private void OnStart()
	{
		//game.view.cameraView.OnStart ();
	}

	private void InitCamera()
	{
		_cameraView.OnInit ();
	}

	/*
	private void ShakeCamera()
	{
		if(DOTween.IsTweening(Camera.main))
			return;
		
		game.view.cameraView.DOShake ();
		//_gameManager.Add1Point();
		//FindObjectOfType<CameraManager>().DOShake();
	}
*/
}
