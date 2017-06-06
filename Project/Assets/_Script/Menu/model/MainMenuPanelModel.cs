using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuPanelModel : Model
{
	public Text 				textCoinsCount			{ get { return _textCoinsCount; } }
	public Text 				textCrystalsCount		{ get { return _textCrystalsCount; } }
	public Text 				textRecord				{ get { return _textRecord; } }
	public bool 				isGameServicesOpened	{ get { return _isGameServicesOpened; } set { _isGameServicesOpened = value;} }
	public Text 				textDoubleCoin			{ get { return _textDoubleCoin; } }
	public Text 				textCoinsPack_00		{ get { return _textCoinsPack_00; } }
	public Text 				textCoinsPack_01		{ get { return _textCoinsPack_01; } }
	public CanvasGroup			panelRewardVideo		{ get { return _panelRewardVideo; } }

	[SerializeField]
	private CanvasGroup			_panelRewardVideo;
	[SerializeField]
	private Text				_textCoinsPack_01;
	[SerializeField]
	private Text				_textCoinsPack_00;
	[SerializeField]
	private Text				_textDoubleCoin;
	[SerializeField]
	private bool				_isGameServicesOpened;
	[SerializeField]
	private Text				_textRecord;
	[SerializeField]
	private Text				_textCrystalsCount;
	[SerializeField]
	private Text				_textCoinsCount;
}
