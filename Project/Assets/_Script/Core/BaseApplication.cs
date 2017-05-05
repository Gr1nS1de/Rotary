using UnityEngine;
using System.Collections;
//using DG.Tweening;

public enum NotifyType
{
	GAME,
	UI,
	ALL
}

public abstract class BaseApplication<G, U> : BaseApplication
	where G : Element
	where U : Element
{
	public G game			{ get { return (G)(object)m_Game; } }
	public U ui				{ get { return (U)(object)m_UI; } }
}

public abstract class BaseApplication : Element
{

	private void Awake()
	{
		InitTweening ();
		Notify( N.RCStartLoad );
	}

	private void Start()
	{		
		
		Notify(N.OnStart);
	}
		
	public void Notify( string alias, Object target, params object[] data )
	{
		
		Traverse( delegate ( Transform it )
		{
			Controller[] list = it.GetComponents<Controller>();

			for ( int i = 0; i < list.Length; i++ )
			{
				list[i].OnNotification( alias, target, data );
			}

			return true;
		} );
	}

	public void NotifyNextFrame( string alias, Object target, params object[] data )
	{

		StartCoroutine(NotifyNextFrameRoutine(alias, target, data));
	}

	private IEnumerator NotifyNextFrameRoutine(string alias, Object target, params object[] data)
	{
		yield return null;

		Traverse( delegate ( Transform it )
		{
			Controller[] list = it.GetComponents<Controller>();

			for ( int i = 0; i < list.Length; i++ )
			{
				list[i].OnNotification( alias, target, data );
			}

			return true;
		} );
	}

	private void InitTweening()
	{
		//DOTween.KillAll();	

		//if (Time.realtimeSinceStartup < 1)
			//DOTween.Init ();
	}
}
