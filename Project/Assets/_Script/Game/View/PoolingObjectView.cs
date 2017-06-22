using UnityEngine;
using System.Collections;

public abstract class PoolingObjectView : View
{
	public PoolingObjectType PoolingType;
	public PoolingObjectState ObjectVisibleState;

	public virtual void OnAddToPool()
	{

	}

	public void GoToVisibleState(PoolingObjectState visibleState)
	{
		ObjectVisibleState = visibleState;
	}
}

