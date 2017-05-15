using UnityEngine;
using System.Collections;

public class ViewVisibleDetect : MonoBehaviour
{
	void OnBecameInvisible()
	{
		transform.parent.GetComponent<View> ().OnInvisible (this);
	}

	void OnBecameVisible()
	{
		transform.parent.GetComponent<View> ().OnVisible (this);
		
	}
}

