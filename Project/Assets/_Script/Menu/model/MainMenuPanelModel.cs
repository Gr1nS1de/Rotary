using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuPanelModel : Model
{
	public Text 				textCoinsCount			{ get { return _textCoinsCount; } }
	public Text 				textCrystalsCount		{ get { return _textCrystalsCount; } }
	public Text 				textRecord				{ get { return _textRecord; } }
	public bool 				isGameServicesOpened	{ get { return _isGameServicesOpened; } set { _isGameServicesOpened = value;} }

	[SerializeField]
	private bool				_isGameServicesOpened;
	[SerializeField]
	private Text				_textRecord;
	[SerializeField]
	private Text				_textCrystalsCount;
	[SerializeField]
	private Text				_textCoinsCount;
}
