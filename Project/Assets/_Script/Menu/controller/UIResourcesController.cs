using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIResourcesController : Controller
{

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.RCAwakeLoad:
				{
					LoadPlayerSkins ();
					break;
				}

			/*case N.RCLoadGameTheme_:
				{
					GameThemeType gameThemeType = (GameThemeType)data [0];

					break;
				}*/
		}
	}

	private void LoadPlayerSkins()
	{
		PlayerSkinView playerSkinPrefab = ui.model.mainMenuPanelModel.playerSkinPrefab;
		//Get player skins sprites
		List<Sprite> playerSkinsSpritesList = new List<Sprite>( Resources.LoadAll<Sprite> (string.Format ("PlayerSkinSprites")));
		List<PlayerSkinView> playerSkinsViewList = new List<PlayerSkinView> ();;


		//Load this views to ui model for PlayerSkinController
		for (int i = 0; i < playerSkinsSpritesList.Count; i++)
		{
			PlayerSkinView playerSkin = Instantiate (playerSkinPrefab, ui.model.mainMenuPanelModel.playerSkinElementsPanel.transform) as PlayerSkinView;
			Sprite skinSprite = playerSkinsSpritesList [i];

			playerSkin.name = string.Format ("PlayerSkin_{0:00}", i);
			playerSkin.SkinId = i;
			playerSkin.SkinSprite = skinSprite;

			ui.model.mainMenuPanelModel.playerSkinsList.Add (playerSkin);

			playerSkin.gameObject.SetActive (true);

			playerSkinsViewList.Add (playerSkin);
		}

		ui.controller.PlayerSkinController.InitPlayerSkins (playerSkinsViewList);
	}

}

