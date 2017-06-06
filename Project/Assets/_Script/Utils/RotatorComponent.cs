using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class RotatorComponent : MonoBehaviour
{
	public float RotateSpeed = 1f;

	void Update()
	{
		transform.Rotate (0f, 0f, RotateSpeed * Time.deltaTime);
	}
}

