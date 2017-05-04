using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIModel : Model
{
	public UIGameModel		UIGameModel				{ get { return _UIGameModel 		= SearchLocal<UIGameModel>(			_UIGameModel,		typeof(UIGameModel).Name);	} }
	public UIMenuModel		UIMenuModel				{ get { return _UIMenuModel			= SearchLocal<UIMenuModel>(			_UIMenuModel,		typeof(UIMenuModel).Name);	} }

	private UIGameModel		_UIGameModel;
	private UIMenuModel		_UIMenuModel;
}

