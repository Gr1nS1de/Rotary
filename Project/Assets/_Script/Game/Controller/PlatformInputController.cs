using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlatformInputController : Controller
{
	private Dictionary<Transform, Vector3> _selectedPlatformsDictionary = new Dictionary<Transform, Vector3>();

	private Vector2 _screenSize;
	private Vector3 _horizontalPlatformSize;
	private float 	_smoothStartAccumulative = 0;
	private Tweener	_smoothStartTween;

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.OnPlatformInput___:
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

					if(!_selectedPlatformsDictionary.ContainsKey(selectedPlatform))
						_selectedPlatformsDictionary.Add (selectedPlatform, selectedPointDelta);

					if (_selectedPlatformsDictionary.Count == 1)
						_smoothStartAccumulative = 0.5f;

					if (_smoothStartTween != null && _smoothStartTween.IsActive ())
						_smoothStartTween.Kill ();

					_smoothStartTween = 
						DOVirtual.Float (_smoothStartAccumulative, 0f, 0.1f, (val ) =>
						{
							_smoothStartAccumulative = val;
						}).SetUpdate(UpdateType.Fixed);
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

					if (_selectedPlatformsDictionary.Count == 0)
					{
						if (_smoothStartTween.IsActive ())
							_smoothStartTween.Kill ();
						
						_smoothStartTween = 
							DOVirtual.Float (_smoothStartAccumulative, 0.1f, 0.05f, (val ) =>
							{
								_smoothStartAccumulative = val;
							});
					}
					break;
				}
		}

	}

	private void MovePlatform(Transform selectedPlatform, float inputY)
	{
		if (selectedPlatform == null || !_selectedPlatformsDictionary.ContainsKey(selectedPlatform))
			return;
		
		float positionY = Mathf.Clamp (inputY + _selectedPlatformsDictionary[selectedPlatform].y, -_screenSize.y / 2f - _horizontalPlatformSize.y / 2f * 0.9f, _screenSize.y / 2f + _horizontalPlatformSize.y / 2f * 0.9f);

		selectedPlatform
			.DOMoveY (positionY, game.model.platformModel.horizontalPlatformInputSpeed * (1 + _smoothStartAccumulative))
			.SetEase(Ease.Linear)
			.SetUpdate(UpdateType.Fixed);
	}

	private void OnGameOver()
	{
		if(_selectedPlatformsDictionary != null)
			_selectedPlatformsDictionary.Clear ();
	}
}

