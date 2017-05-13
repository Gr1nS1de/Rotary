using UnityEngine;
using System.Collections;

public class DontDestroy : MonoBehaviour {

	void Awake()
	{
        //если такой объект уже есть - уничтожить
	    var goThis = GameObject.Find(transform.name);
	    if (goThis != gameObject)
	    {
	        Destroy(gameObject);
	    }
	    else
	    {
            DontDestroyOnLoad(transform.gameObject);    
	    }
		
	}
}
