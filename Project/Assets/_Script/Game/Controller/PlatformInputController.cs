using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlatformInputController : Controller
{
	private Dictionary<Transform, Vector3> _selectedPlatformsDictionary = new Dictionary<Transform, Vector3>();

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
		}
	}

	private void OnStart()
	{

	}

	private void OnDragPlatform (Transform selectedPlatform, Vector3 inputPoint, FingerMotionPhase gesturePhase)
	{
		//Debug.Log ("Drag gear = " + selectedGear.gameObject.name + " point " + inputPoint);

		Vector3 selectedPoint = new Vector3 (inputPoint.x, inputPoint.y, -2f);// Z = -2 bcs move object forward to camera
		//Vector3 gearPosition = selectedPlatform.transform.position;

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
		selectedPlatform.transform.DOMoveY (inputY + _selectedPlatformsDictionary[selectedPlatform].y, 0.1f);
	}

}

