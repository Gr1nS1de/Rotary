using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlatformInputController : Controller
{
	private Transform _currentSelectedPlatform = null;
	private Vector3 _selectedPointDelta;

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

					//Debug.LogError ("Input "+dragItem);
					//If just start drag with new gear
					if (dragItem != null)
					{

						//Get item gear view
						if (dragItem)
							OnDragGear (dragItem.transform, inputPoint, gesturePhase);
						else
							Debug.LogError ("Input gear element == null. ");
					}
					else if (_currentSelectedPlatform)
					{
						OnDragGear (_currentSelectedPlatform, inputPoint, gesturePhase);
					}

					break;
				}
		}
	}

	private void OnStart()
	{

	}

	private void OnDragGear (Transform selectedPlatform, Vector3 inputPoint, FingerMotionPhase gesturePhase)
	{
		//Debug.Log ("Drag gear = " + selectedGear.gameObject.name + " point " + inputPoint);

		Vector3 selectedPoint = new Vector3 (inputPoint.x, inputPoint.y, -2f);// Z = -2 bcs move object forward to camera
		//Vector3 gearPosition = selectedPlatform.transform.position;

		switch (gesturePhase)
		{
			case FingerMotionPhase.Started:
				{
					if (_currentSelectedPlatform != null)
						return;

					_currentSelectedPlatform = selectedPlatform;

					//Get delta between touch point and gear center position - for proper gear drag
					_selectedPointDelta = new Vector3(selectedPlatform.position.x, selectedPlatform.position.y, selectedPoint.z) - selectedPoint;
					

					break;
				}

			case FingerMotionPhase.Updated:
				{
					if (_currentSelectedPlatform != null)
					{
						MoveSelectedPlatform (inputPoint.y);
					}
					break;
				}

			case FingerMotionPhase.Ended:
				{
					_currentSelectedPlatform = null;
					break;
				}
		}

	}

	private void MoveSelectedPlatform(float inputY)
	{
		_currentSelectedPlatform.transform.DOMoveY (inputY + _selectedPointDelta.y, 0.1f);
	}

	void OnDestroy()
	{
		_currentSelectedPlatform = null;
	}

}

