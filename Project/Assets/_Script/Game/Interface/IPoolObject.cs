using UnityEngine;
using System.Collections;

public interface IPoolObject
{
	void OnInit();
	void OnVisible();
	void OnInvisible();
	void OnAddToPool();
}

