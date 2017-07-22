using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCollisionDetect : MonoBehaviour 
{
	public LayerMask ignoredMasks;

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(ignoredMasks == (ignoredMasks | (1 << collision.gameObject.layer)))
			return;
		
		if (collision.transform.parent == null)
			return;

		View viewObject = collision.transform.GetComponentInParent<View> ();

		if (viewObject == null)
			return;

		transform.parent.GetComponent<View>().OnRendererCollisionEnter (this, collision);
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		if(ignoredMasks == (ignoredMasks | (1 << collision.gameObject.layer)))
			return;

		if (collision.transform.parent == null)
			return;

		View viewObject = collision.transform.GetComponentInParent<View> ();

		if (viewObject == null)
			return;

		transform.parent.GetComponent<View>().OnRendererCollisionExit (this, collision);
	}
}
