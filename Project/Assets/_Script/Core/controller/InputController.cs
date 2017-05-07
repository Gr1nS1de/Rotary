using UnityEngine;
using System.Collections;

public class InputController : Controller
{
	private bool _isStationaryStarted = false;
	private bool _isMovedStarted = false;

	void OnFingerMove( FingerMotionEvent e ) 
	{

		//Debug.LogError ("Moved "+e.Selection);
		if ((!_isStationaryStarted && !_isMovedStarted) && e.Phase == FingerMotionPhase.Started && e.Selection != null)// && e.Raycast.Hit2D.rigidbody != null)
		{
			_isMovedStarted = true;

			Notify (N.OnGameInput___, NotifyType.GAME, e.Selection, Camera.main.ScreenToWorldPoint(e.Position), e.Phase);
		}

		if ((_isMovedStarted || _isStationaryStarted) && e.Phase == FingerMotionPhase.Updated)
		{
			//Null selected sended bcs it is currentGearView
			Notify (N.OnGameInput___, NotifyType.GAME, null, Camera.main.ScreenToWorldPoint(e.Position), e.Phase);
		}
			

	}

	void OnFingerStationary( FingerMotionEvent e ) 
	{

		if ((!_isStationaryStarted && !_isMovedStarted) && e.Phase == FingerMotionPhase.Started && e.Selection != null)// && e.Raycast.Hit2D.rigidbody != null)
		{
			_isStationaryStarted = true;

			Notify (N.OnGameInput___, NotifyType.GAME, e.Selection, Camera.main.ScreenToWorldPoint(e.Position), e.Phase);
		}
		
	}

	void OnFingerUp( FingerUpEvent e ) 
	{
		if (_isMovedStarted || _isStationaryStarted)
		{
			_isMovedStarted = false;
			_isStationaryStarted = false;
			Notify (N.OnGameInput___, NotifyType.GAME, null, Camera.main.ScreenToWorldPoint(e.Position), FingerMotionPhase.Ended);
		}
	}

}