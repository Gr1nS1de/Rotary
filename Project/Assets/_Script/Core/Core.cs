using UnityEngine;
using System.Collections;

public class Core : BaseApplication<GameApplication, UIApplication>
{
	#region controllers
	public PurchaseController		purchaseController				{ get { return _purchaseController			= SearchLocal<PurchaseController>(			_purchaseController,			typeof(PurchaseController).Name );}}
	public PlayerDataController		playerDataController			{ get { return _playerDataController		= SearchLocal<PlayerDataController>(		_playerDataController,			typeof(PlayerDataController).Name );}}

	private PlayerDataController	_playerDataController;
	private PurchaseController		_purchaseController;
	#endregion

	#region models
	public PlayerDataModel			playerDataModel			{ get { return _playerDataModel		= SearchLocal<PlayerDataModel>(		_playerDataModel,			typeof(PlayerDataModel).Name );}}

	private PlayerDataModel			_playerDataModel;
	#endregion
}


