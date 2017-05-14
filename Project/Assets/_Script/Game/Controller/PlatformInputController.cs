using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlatformInputController : Controller
{
	private Dictionary<Transform, Vector3> _selectedPlatformsDictionary = new Dictionary<Transform, Vector3>();

	private Vector2 _screenSize;
	private Vector3 _horizontalPlatformSize;

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.OnGameInput___:
				{
					GameObject dragItem = (GameObject)data [0];
					Vector3 inputPoint = (Vector3)data [1];
					FingerMotionPhase gesturePhase = (FingerMotionPhase)data [2];


					OnDragPlatform (dragItem.transform, inputPoint, gesturePhase);
					break;
				}

			case N.GameOver_:
				{
					GameOverData gameOverData = (GameOverData)data [0];

					OnGameOver ();
					break;
				}
		}
	}

	private void OnStart()
	{
		_screenSize = GM.Instance.ScreenSize;
		_horizontalPlatformSize = game.model.gameTheme.GetPlatformRendererSize (PlatformTypes.Horizontal);
	}

	private void OnDragPlatform (Transform selectedPlatform, Vector3 inputPoint, FingerMotionPhase gesturePhase)
	{
		//Debug.Log ("Drag platform = " + selectedPlatform.gameObject.name + " point " + inputPoint);
		PlatformView platformView = selectedPlatform.GetComponentInParent<PlatformView>();

		if (platformView == null)
			return;
		
		Vector3 selectedPoint = new Vector3 (inputPoint.x, inputPoint.y, 0f);
		selectedPlatform = platformView.transform;

		switch (gesturePhase)
		{
			case FingerMotionPhase.Started:
				{
					//Get delta between touch point and gear center position - for proper gear drag
					Vector3 selectedPointDelta = new Vector3(selectedPlatform.position.x, selectedPlatform.position.y, selectedPoint.z) - selectedPoint;

					_selectedPlatformsDictionary.Add (selectedPlatform, selectedPointDelta);

					break;
				}

			case FingerMotionPhase.Updated:
				{
					MovePlatform (selectedPlatform ,inputPoint.y);
					break;
				}

			case FingerMotionPhase.Ended:
				{
					if (_selectedPlatformsDictionary.ContainsKey (selectedPlatform))
						_selectedPlatformsDictionary.Remove (selectedPlatform);
					break;
				}
		}

	}

	private void MovePlatform(Transform selectedPlatform, float inputY)
	{
		float positionY = Mathf.Clamp (inputY + _selectedPlatformsDictionary[selectedPlatform].y, -_screenSize.y / 2f - _horizontalPlatformSize.y / 2f * 0.9f, _screenSize.y / 2f + _horizontalPlatformSize.y / 2f * 0.9f);

		selectedPlatform.transform.DOMoveY (positionY, 0.05f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
	}

	private void OnGameOver()
	{
		if(_selectedPlatformsDictionary != null)
			_selectedPlatformsDictionary.Clear ();
	}
}

