using UnityEngine;
using System.Collections;

public interface IPoolObject
{
	void Init();
	void OnVisible();
	void OnInvisible();
}

