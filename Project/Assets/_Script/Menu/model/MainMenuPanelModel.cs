using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenuPanelModel : Model
{
	public Text 								textCoinsCount			{ get { return _textCoinsCount; } }
	public Text 								textCrystalsCount		{ get { return _textCrystalsCount; } }
	public Text 								textRecord				{ get { return _textRecord; } }
	public bool 								isGameServicesOpened	{ get { return _isGameServicesOpened; } 	set { _isGameServicesOpened = value;} }
	public Text 								textDoubleCoin			{ get { return _textDoubleCoin; } }
	public Text 								textCoinsPack_00		{ get { return _textCoinsPack_00; } }
	public Text 								textCoinsPack_01		{ get { return _textCoinsPack_01; } }
	public CanvasGroup							panelRewardVideo		{ get { return _panelRewardVideo; } }
	public Text 								textHourGiftTimer		{ get { return _textHourGiftTimer; } }
	public CanvasGroup 							panelHourGiftTitle		{ get { return _panelHourGiftTitle; } }
	public Dictionary<string, PlayerSkinView>	playerSkinsDictionary	{ get { return _playerSkinsDictionary; }}
	public PlayerSkinView						playerSkinPrefab		{ get { return _playerSkinPrefab; }}
	public GridLayoutGroup 						playerSkinElementsPanel	{ get { return _playerSkinElementsPanel; } }

	[SerializeField]
	private GridLayoutGroup						_playerSkinElementsPanel;
	[SerializeField]
	private PlayerSkinView						_playerSkinPrefab;
	[SerializeField]
	private Dictionary<string, PlayerSkinView>	_playerSkinsDictionary 	= new Dictionary<string, PlayerSkinView>();
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
