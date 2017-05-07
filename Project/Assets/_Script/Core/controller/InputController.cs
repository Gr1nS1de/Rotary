using UnityEngine;
using System.Collections;

public class InputController : Controller
{
	/*
	void OnFingerMove( FingerMotionEvent e ) 
	{
		//if(e.Selection != null)
		//	Notify (N.OnGameInput___, NotifyType.GAME, e.Selection, Camera.main.ScreenToWorldPoint(e.Position), e.Phase);
	}

	void OnFingerStationary( FingerMotionEvent e ) 
	{

		//Notify (N.OnGameInput___, NotifyType.GAME, e.Selection, Camera.main.ScreenToWorldPoint(e.Position), e.Phase);
		
	}

	void OnFingerUp( FingerUpEvent e ) 
	{

		//Notify (N.OnGameInput___, NotifyType.GAME, null, Camera.main.ScreenToWorldPoint(e.Position), FingerMotionPhase.Ended);
	}*/

	void OnDrag(DragGesture e)
	{
		if(e.StartSelection != null)
			Notify (N.OnGameInput___, NotifyType.GAME, e.StartSelection, Camera.main.ScreenToWorldPoint(e.Position), e.Phase);
	}
}