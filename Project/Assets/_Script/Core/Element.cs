using UnityEngine;
using System.Collections.Generic;

public abstract class Element<T> : Element
{
	public GameApplication 	game 			{ get { return (GameApplication)m_Game; } }
	public UIApplication 	ui				{ get { return (UIApplication)m_UI; } }

}

public abstract class Element : MonoBehaviour
{
	public GameApplication				m_Game			{ get { return _game 	= !_game ? 	SearchGlobal<GameApplication>(	_game, 		typeof(GameApplication).ToString()) : 	_game; } }
	public UIApplication				m_UI			{ get { return _ui 		= !_ui ? 	SearchGlobal<UIApplication> (	_ui, 		typeof(UIApplication).ToString()) : 	_ui;}}
	public Core							m_Core			{ get { return _core 	= !_core ? 	SearchGlobal<Core> (	_core, 		typeof(Core).ToString()) : 	_core;}}
	public Dictionary<string, object>	m_Storage		{ get { return _storage == null ? _storage = new Dictionary<string, object>() : _storage; } }

	private GameApplication				_game;
	private UIApplication 				_ui;
	private Core 						_core;
	private Dictionary<string, object> 	_storage;

	public void Notify( string alias, NotifyType notifyType = NotifyType.ALL, params object[] data ) 
	{ 
		switch(notifyType)
		{
			case NotifyType.CORE:
				{
					m_Core.Notify (alias, this, data);
					break;
				}
			case NotifyType.ALL:
				{
					m_UI.Notify ( alias, this, data ); 
					m_Game.Notify( alias, this , data );
					m_Core.Notify (alias, this, data);
					break;
				}
			case NotifyType.GAME:
				{
					m_Game.Notify( alias, this , data );
					m_Core.Notify (alias, this, data);
					break;
				}
			case NotifyType.UI:
				{
					m_UI.Notify ( alias, this, data );
					m_Core.Notify (alias, this, data);
					break;
				}
		}
	}
	public void NotifyNextFrame( string alias, NotifyType notifyType = NotifyType.ALL, params object[] data ) { m_Game.NotifyNextFrame( alias, this, notifyType, data ); m_UI.NotifyNextFrame ( alias, this, data ); }

	public T SearchGlobal<T> (T obj, string storeKey = "", bool update = false ) where T : Object
	{
		/*
		if (obj)
			Debug.Log ("Start search: " + obj.name + " SK = " + storeKey);
		else
			Debug.Log ("Store key = " + storeKey);
		*/
		
		if (m_Storage.ContainsKey (storeKey) && storeKey != "" && !update)
			if ((T)m_Storage [storeKey] != null)
			{
				return (T)m_Storage [storeKey];
			}
			else
			{
				m_Storage.Remove (storeKey);
			}

		var searchFor = GameObject.FindObjectOfType<T>();

		if ( searchFor && storeKey != "" )
		{
			if ( update && m_Storage.ContainsKey( storeKey ) )
				m_Storage.Remove(storeKey);

			m_Storage.Add( storeKey, searchFor );
		}

		//Debug.Log ("Return " + searchFor);

		return searchFor;
	}

	public T[] SearchGlobal<T>( T[] obj, string storeKey = "", bool update = false ) where T : Object
	{
		if (m_Storage.ContainsKey (storeKey) && storeKey != "" && !update)
			if ((T)m_Storage [storeKey] != null)
			{
				return (T[])m_Storage [storeKey];
			}
			else
			{
				m_Storage.Remove (storeKey);
			}

		var searchFor = GameObject.FindObjectsOfType<T>();

		if ( searchFor.Length > 0 && storeKey != "" )
		{
			if ( update && m_Storage.ContainsKey( storeKey ) )
				m_Storage.Remove( storeKey );

			m_Storage.Add( storeKey, searchFor );
		}

		return searchFor;
	}

	public T SearchLocal<T>( T obj, string storeKey = "", bool update = false ) where T : Object
	{
		//If object with such key exist at sorage & we dont need update
		if (m_Storage.ContainsKey (storeKey) && !string.IsNullOrEmpty(storeKey) && !update)
		{
			//If object exist
			if ((T)m_Storage [storeKey] != null)
			{
				return (T)m_Storage [storeKey];
			}
			else
			{
				m_Storage.Remove (storeKey);
			}
		}

		var searchFor = transform.GetComponent<T>() ? transform.GetComponent<T>() : transform.GetComponentInChildren<T>();

		if ( searchFor && storeKey != "" )
		{
			if ( update && m_Storage.ContainsKey( storeKey ) )
				m_Storage.Remove( storeKey );

			m_Storage.Add( storeKey, searchFor );
		}

		return searchFor;
	}

	public T[] SearchLocal<T>( T[] obj, string storeKey = "", bool update = false ) where T : Object
	{
		if (m_Storage.ContainsKey (storeKey) && storeKey != "" && !update)
		{
			if (m_Storage [storeKey] != null)
			{
				return (T[])m_Storage [storeKey];
			}
			else
			{
				m_Storage.Remove (storeKey);
			}
		}

		List<T> searchFor = new List<T>( transform.GetComponents<T>());
		searchFor.AddRange (transform.GetComponentsInChildren<T> (true));

		if ( searchFor.Count > 0 && storeKey != "" )
		{
			if ( update && m_Storage.ContainsKey( storeKey ) )
				m_Storage.Remove( storeKey );

			m_Storage.Add( storeKey, searchFor.ToArray() );
		}

		return searchFor.ToArray();
	}

	public void Traverse( System.Predicate<Transform> callback, System.Type controller = null )
	{
		OnTraverseStep( transform, callback );
	}

	private void OnTraverseStep( Transform target, System.Predicate<Transform> callback )
	{
		if (target)
			if (!callback (target))
				return;

		for ( int i = 0; i < target.childCount; i++ )
			OnTraverseStep( target.GetChild( i ), callback );
	}

	public T Cast<T>() { return (T)(object)this; }
}
