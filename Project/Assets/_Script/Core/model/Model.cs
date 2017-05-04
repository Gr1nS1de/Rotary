using UnityEngine;
using System.Collections;


public abstract class Model : Element
{
	public GameApplication 	game 	{ get { return (GameApplication)m_Game; } }
	public UIApplication 	ui		{ get { return (UIApplication)m_UI; } }
}
