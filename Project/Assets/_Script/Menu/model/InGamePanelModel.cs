﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InGamePanelModel : Model 
{
	public Text 				textCoinsCount			{ get { return _textCoinsCount; } }
	public Text 				textCrystalsCount		{ get { return _textCrystalsCount; } }
	public Text 				textRecord				{ get { return _textRecord; } }
	public Text 				textScore				{ get { return _textScore; } }

	[SerializeField]
	private Text				_textScore;
	[SerializeField]
	private Text				_textRecord;
	[SerializeField]
	private Text				_textCrystalsCount;
	[SerializeField]
	private Text				_textCoinsCount;
}
