using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCollisionDetect : MonoBehaviour 
{
	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.parent == null)
			return;

		View viewObject = collision.transform.parent.GetComponent<View> ();

		if (viewObject == null)
			return;

		transform.parent.GetComponent<View>().OnRendererCollisionEnter (collision);
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.transform.parent == null)
			return;

		View viewObject = collision.transform.parent.GetComponent<View> ();

		if (viewObject == null)
			return;

		transform.parent.GetComponent<View>().OnRendererCollisionExit (collision);
	}
}
