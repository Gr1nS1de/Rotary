using UnityEngine;
using System.Collections;

public class UIMainMenuController : Controller
{
	private UIMainMenuModel 		_uiMainMenuModel			{ get { return ui.model.UIMainMenuModel; } }

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}
		}
	}

	private void OnStart()
	{

	}

	#region Public methods
	public void ButtonGamePlay()
	{
		Notify (N.GamePlay);
	}
	#endregion

}
