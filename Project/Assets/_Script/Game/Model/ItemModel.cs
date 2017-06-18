using UnityEngine;
using System.Collections;

public enum ItemTypes
{
	Coin,
	Crystal,
	Magnet
}

public class ItemModel : Model
{
	public int	crystalFractureCount	{ get { return _crystalFractureCount; } set { _crystalFractureCount = value; }}

	[SerializeField]
	private int	_crystalFractureCount;

	public Vector3 GetItemRendererSize(ItemTypes itemType)
	{
		return game.model.itemsFactoryModel.itemsPrefabsList.Find (item => item.ItemType == itemType).GetMainRendererSize();
	}
}

