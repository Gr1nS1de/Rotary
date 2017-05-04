using UnityEngine;
using System.Collections;

public class UIMenuController : Controller
{
	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.GameOnStart:
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
	public void ButtonGenerateLevel()
	{
		Notify (N.StartGame);
	}
	#endregion

}
