using UnityEngine;
using System.Collections;

public abstract class View<T> : View
{
	public GameApplication 	game 	{ get { return (GameApplication)m_Game; } }
	public UIApplication 	ui		{ get { return (UIApplication)m_UI; } }
}


public abstract class View : Element
{
	public GameApplication 	game 	{ get { return (GameApplication)m_Game; } }
	public UIApplication 	ui		{ get { return (UIApplication)m_UI; } }

	public virtual void OnRendererCollisionEnter(Collision2D collision)
	{
	}

	public virtual void OnRendererCollisionExit(Collision2D collision)
	{
	}

	public virtual void OnRendererTriggerEnter(Collider2D otherCollider)
	{
	}

	public virtual void OnRendererTriggerExit(Collider2D otherCollider)
	{
	}

	public virtual void OnVisible()
	{
	}

	public virtual void OnInvisible()
	{
	}
}



