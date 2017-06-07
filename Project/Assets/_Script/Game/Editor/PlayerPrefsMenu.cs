using UnityEditor;
using UnityEngine;


public class PlayerPrefsMenu 
{
	[MenuItem("PlayerPrefs/Clear")]
	static void ClearCache()
	{
		PlayerPrefs.DeleteAll();
	}
}
