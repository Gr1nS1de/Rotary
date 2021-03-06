﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenuPanelModel : Model
{
	public Text 								textCoinsCount						{ get { return _textCoinsCount; } }
	public Text 								textCrystalsCount					{ get { return _textCrystalsCount; } }
	public Text 								textRecord							{ get { return _textRecord; } }
	public bool 								isGameServicesOpened				{ get { return _isGameServicesOpened; } 	set { _isGameServicesOpened = value;} }
	public Text 								textDoubleCoin						{ get { return _textDoubleCoin; } }
	public Text 								textCoinsPack_00					{ get { return _textCoinsPack_00; } }
	public Text 								textCoinsPack_01					{ get { return _textCoinsPack_01; } }
	public CanvasGroup							panelRewardVideo					{ get { return _panelRewardVideo; } }
	public Text 								textHourGiftTimer					{ get { return _textHourGiftTimer; } }
	public CanvasGroup 							panelHourGiftTitle					{ get { return _panelHourGiftTitle; } }
	public List<PlayerSkinView>					playerSkinsList						{ get { return _playerSkinsList; }}
	public PlayerSkinView						playerSkinPrefab					{ get { return _playerSkinPrefab; }}
	public GridLayoutGroup 						playerSkinElementsPanel				{ get { return _playerSkinElementsPanel; } }
	public Image		 						imageCurrentPlayerSkin				{ get { return _imageCurrentPlayerSkin; } }
	public Text		 							textPlayerSkinGeneralGamesPlayed	{ get { return _textPlayerSkinGeneralGamesPlayed; } }
	public Text		 							textPlayerCurrentSkinGamesPlayed	{ get { return _textPlayerCurrentSkinGamesPlayed; } }
	public bool		 							isDailyGiftAvailable				{ get { return _isDailyGiftAvailable; } set {_isDailyGiftAvailable = value;}}
	public Image	 							imageDailyGiftBowTie				{ get { return _imageDailyGiftBowTie; } }
	public Text	 								textLastScore						{ get { return _textLastScore; } }
	public Text	 								textNewRecord						{ get { return _textNewRecord; } }
	public Text	 								textCoinsAdd						{ get { return _textCoinsAdd; } }
	public Text	 								textCrystalsAdd						{ get { return _textCrystalsAdd; } }
	public Image	 							imageCoinIcon						{ get { return _imageCoinIcon; } }
	public Image	 							imageCrystalIcon					{ get { return _imageCrystalIcon; } }
	public CanvasGroup	 						panelIsDoubleCoinBoughtMark			{ get { return _panelIsDoubleCoinBoughtMark; } }

	[SerializeField]
	private CanvasGroup							_panelIsDoubleCoinBoughtMark;
	[SerializeField]
	private Image								_imageCrystalIcon;
	[SerializeField]
	private Image								_imageCoinIcon;
	[SerializeField]
	private Text								_textCrystalsAdd;
	[SerializeField]
	private Text								_textCoinsAdd;
	[SerializeField]
	private Text								_textNewRecord;
	[SerializeField]
	private Text								_textLastScore;
	[SerializeField]
	private Image								_imageDailyGiftBowTie;
	private bool								_isDailyGiftAvailable;
	[SerializeField]
	private Text								_textPlayerCurrentSkinGamesPlayed;
	[SerializeField]
	private Text								_textPlayerSkinGeneralGamesPlayed;
	[SerializeField]
	private Image								_imageCurrentPlayerSkin;
	[SerializeField]
	private GridLayoutGroup						_playerSkinElementsPanel;
	[SerializeField]
	private PlayerSkinView						_playerSkinPrefab;
	[SerializeField]
	private List<PlayerSkinView>				_playerSkinsList 	= new List<PlayerSkinView>();
	[SerializeField]
	private CanvasGroup							_panelHourGiftTitle;
	[SerializeField]
	private Text								_textHourGiftTimer;
	[SerializeField]
	private CanvasGroup							_panelRewardVideo;
	[SerializeField]
	private Text								_textCoinsPack_01;
	[SerializeField]
	private Text								_textCoinsPack_00;
	[SerializeField]
	private Text								_textDoubleCoin;
	[SerializeField]		
	private bool								_isGameServicesOpened;
	[SerializeField]
	private Text								_textRecord;
	[SerializeField]
	private Text								_textCrystalsCount;
	[SerializeField]
	private Text								_textCoinsCount;
}
