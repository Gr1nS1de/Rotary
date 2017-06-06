using UnityEngine;
using System.Collections;

public class UIResourcesController : Controller
{

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.RCAwakeLoad:
				{
					break;
				}

			case N.RCLoadGameTheme_:
				{
					GameThemeType gameThemeType = (GameThemeType)data [0];

					break;
				}
		}
	}

}

