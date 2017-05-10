using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMainMenuModel : Model
{
	public CanvasGroup 	canvasGroup				{ get { return _canvasGroupStart; } }
	public Text 		bestScoreText			{ get { return _bestScoreText; } }
	public Text 		lastScoreText			{ get { return _lastScoreText; } }
	public Button 		buttonPlay				{ get { return _buttonPlay; } }

	[SerializeField]
	private Button		_buttonPlay;
	[SerializeField]
	private CanvasGroup	_canvasGroupStart;
	[SerializeField]
	private Text		_bestScoreText;
	[SerializeField]
	private Text		_lastScoreText;
}
