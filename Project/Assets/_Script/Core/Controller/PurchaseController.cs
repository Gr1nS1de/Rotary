using UnityEngine;
using System.Collections;

public class PurchaseController : Controller
{
	//replace with your consumable item
	public const string COINS_PACK_00 	= "coins_pack_00";
	public const string COINS_PACK_01 	= "coins_pack_01";

	//replace with your non-consumable item
	public const string DOUBLE_COIN 	= "double_coins";

	private bool _isInited 	= false;

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch (alias)
		{
			case N.RCAwakeLoad:
				{
					break;
				}

			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.PurchaseDoubleCoin:
				{
					purchase (DOUBLE_COIN);
					break;
				}

			case N.PurchaseCoinsPack_00:
				{
					purchase (COINS_PACK_00);
					break;
				}

			case N.PurchaseCoinsPack_01:
				{
					purchase (COINS_PACK_01);
					break;
				}

		}
	}

	private void OnStart()
	{
		InitProducts ();
		ConnectAndroidPurchaser ();
	}

	private void InitProducts()
	{
		GoogleProductTemplate doubleCoin = new GoogleProductTemplate()
		{
			ProductType = AN_InAppType.NonConsumable,
			SKU = "double_coin",
			Title = Localization.CheckKey("TK_DOUBLE_COIN_NAME"),

		};

		GoogleProductTemplate coinsPack_00 = new GoogleProductTemplate()
		{
			ProductType = AN_InAppType.Consumable,
			SKU = "coinsPack_00",
			Title = Localization.CheckKey("TK_COINS_PACK_00_NAME"),

		};

		GoogleProductTemplate coinsPack_01 = new GoogleProductTemplate()
		{
			ProductType = AN_InAppType.Consumable,
			SKU = "coinsPack_01",
			Title = Localization.CheckKey("TK_COINS_PACK_01_NAME"),

		};

		AndroidInAppPurchaseManager.Client.AddProduct (doubleCoin);
		AndroidInAppPurchaseManager.Client.AddProduct (coinsPack_00);
		AndroidInAppPurchaseManager.Client.AddProduct (coinsPack_01);
	}

	private void ConnectAndroidPurchaser()
	{
		AndroidInAppPurchaseManager.ActionProductPurchased += OnProductPurchased;
		AndroidInAppPurchaseManager.ActionProductConsumed += OnProductConsumed;
		AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;

		//you may use loadStore function without parametr if you have filled base64EncodedPublicKey in plugin settings
		AndroidInAppPurchaseManager.Client.Connect();
	}

	public  void purchase(string SKU)
	{
		AndroidInAppPurchaseManager.Client.Purchase (SKU);
	}

	public void consume(string SKU) 
	{
		AndroidInAppPurchaseManager.Client.Consume (SKU);
	}

	private  void OnProcessingPurchasedProduct(GooglePurchaseTemplate purchase)
	{
		//some stuff for processing product purchse. Add coins, unlock track, etc

		switch(purchase.SKU) 
		{
			case COINS_PACK_00:
				{
					consume (COINS_PACK_00);
				
					break;
				}

			case COINS_PACK_01:
				{
					consume (COINS_PACK_01);
					break;
				}

			case DOUBLE_COIN:
				{
					Notify (N.OnPurchasedDoubleCoin);
					break;
				}
		}
	}

	private void OnProcessingConsumeProduct(GooglePurchaseTemplate purchase) 
	{
		switch(purchase.SKU) 
		{
			case COINS_PACK_00:
				{
					core.playerDataController.UpdatePlayerItemCount (ItemTypes.Coin, 3000);
					Notify (N.OnPurchasedCoinsPack_00);
					break;
				}

			case COINS_PACK_01:
				{
					core.playerDataController.UpdatePlayerItemCount (ItemTypes.Coin, 15000);
					Notify (N.OnPurchasedCoinsPack_01);
					break;
				}
		}
	}

	private void OnProductPurchased(BillingResult result) 
	{

		//this flag will tell you if purchase is available
		//result.isSuccess


		//infomation about purchase stored here
		//result.purchase

		//here is how for example you can get product SKU
		//result.purchase.SKU


		if(result.IsSuccess) 
		{
			OnProcessingPurchasedProduct (result.Purchase);
		} else {
			AndroidMessage.Create("Product Purchase Failed", result.Response.ToString() + " " + result.Message);
		}

		Debug.Log ("Purchased Responce: " + result.Response.ToString() + " " + result.Message);
	}


	private void OnProductConsumed(BillingResult result)
	{

		if(result.IsSuccess) 
		{
			OnProcessingConsumeProduct (result.Purchase);
		} else {
			AndroidMessage.Create("Product Cousume Failed", result.Response.ToString() + " " + result.Message);
		}

		Debug.Log ("Cousume Responce: " + result.Response.ToString() + " " + result.Message);
	}


	private void OnBillingConnected(BillingResult result)
	{

		AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;

		if(result.IsSuccess) 
		{
			//Store connection is Successful. Next we loading product and customer purchasing details
			AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
			AndroidInAppPurchaseManager.Client.RetrieveProducDetails();

		} 

		//AndroidMessage.Create("Connection Responce", result.Response.ToString() + " " + result.Message);
		Debug.Log ("Connection Responce: " + result.Response.ToString() + " " + result.Message);
	}

	private void OnRetrieveProductsFinised(BillingResult result)
	{
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;
		if(result.IsSuccess) 
		{
			UpdateStoreData();
			_isInited = true;
		} else {
			//AndroidMessage.Create("Connection Responce", result.Response.ToString() + " " + result.Message);
		}

		Notify (N.PurchaseProductsLoaded_, NotifyType.ALL, result.IsSuccess);
	}

	private void UpdateStoreData() 
	{

		foreach(GoogleProductTemplate p in AndroidInAppPurchaseManager.Client.Inventory.Products) 
		{
			Debug.Log("Loaded product: " + p.Title);
		}

		//chisking if we already own some consuamble product but forget to consume those

		if(AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased(COINS_PACK_00)) 
		{
			consume(COINS_PACK_00);
		}

		if(AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased(COINS_PACK_01)) 
		{
			consume(COINS_PACK_01);
		}

		//Check if non-consumable rpduct was purchased, but we do not have local data for it.
		//It can heppens if game was reinstalled or download on oher device
		//This is replacment for restore purchase fnunctionality on IOS


		if(AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased(DOUBLE_COIN))
		{
			Notify (N.OnPurchasedDoubleCoin);
		}


	}
}

