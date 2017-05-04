using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMenuModel : Model
{
	public CanvasGroup 	canvasGroupStart		{ get { return _canvasGroupStart; } }
	public Text 		bestScoreText			{ get { return _bestScoreText; } }
	public Text 		lastScoreText			{ get { return _lastScoreText; } }

	[SerializeField]
	private CanvasGroup	_canvasGroupStart;
	[SerializeField]
	private Text		_bestScoreText;
	[SerializeField]
	private Text		_lastScoreText;
}
