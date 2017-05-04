using UnityEngine;
using System.Collections;

public abstract class Controller : Element
{
	virtual public void OnNotification( string alias, Object target, params object[] data ) { }

	public GameApplication game 	{ get { return (GameApplication)m_Game; } }
	public UIApplication ui			{ get { return (UIApplication)m_UI; } }
}
