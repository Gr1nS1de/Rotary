using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using DG.Tweening;

public class InGamePanelController : Controller
{
	private InGamePanelModel _inGamePanelModel		{ get { return ui.model.inGamePanelModel; } }
	//private RoadModel 	currentRoadModel		{ get { return game.model.currentGearView; } }

	public override void OnNotification ( string alias, Object target, params object[] data )
	{
		switch (alias)
		{
			case N.GameStart:
				{
					//InitScoreBarItems ();
					//InitScoreBarSlider ();
					InitScoreBar();
					break;
				}

			case N.GameAddScore:
				{
					_inGamePanelModel.textScore.text = string.Format("{0}", game.model.currentScore);

					break;
				}
		}
	}

	private void InitScoreBar()
	{
		_inGamePanelModel.textScore.text = string.Format("0");
	}
}
