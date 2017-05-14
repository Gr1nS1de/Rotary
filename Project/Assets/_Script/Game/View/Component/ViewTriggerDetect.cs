using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTriggerDetect : MonoBehaviour
{
	public LayerMask ignoredMasks;
	void OnTriggerEnter2D(Collider2D otherCollider)
	{		
		if(ignoredMasks == (ignoredMasks | (1 << otherCollider.gameObject.layer)))
			return;

		if (otherCollider.transform.parent == null)
			return;

		View viewObject = otherCollider.transform.parent.GetComponent<View> ();

		if (viewObject == null)
			return;

		transform.parent.GetComponent<View>().OnRendererTriggerEnter (otherCollider);
	}

	void OnTriggerExit2D(Collider2D otherCollider)
	{
		if(ignoredMasks == (ignoredMasks | (1 << otherCollider.gameObject.layer)))
			return;
		
		if (otherCollider.transform.parent == null)
			return;

		View viewObject = otherCollider.transform.parent.GetComponent<View> ();

		if (viewObject == null)
			return;

		transform.parent.GetComponent<View>().OnRendererTriggerExit (otherCollider);
	}
}
