using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemsFactoryModel : Model
{
	public List<ItemView>		itemsPrefabsList	{ get { return _itemsPrefabsList; } }

	[SerializeField]
	private List<ItemView>		_itemsPrefabsList	= new List<ItemView>();
}

