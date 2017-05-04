using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIGameModel : Model 
{
	public CanvasGroup 					canvasGroupInGame		{ get { return _canvasGroupInGame; } }
	public Text							scoreText				{ get { return _scoreText; } }
	public Slider						scoreSlider				{ get { return _scoreSlider; } }
	public GameObject					itemSpotPrefab			{ get { return _itemSpotPrefab; } }
	public GameObject					itemSpotsContainer		{ get { return _itemSpotsContainer; } }
	public Dictionary<int, GameObject>	itemsDictionaryByScore	{ get { return _itemsDictionaryByScore; } }
	public Color						itemDeactivatedColor	{ get { return _itemDeactivatedColor; } }

	[SerializeField]
	private CanvasGroup					_canvasGroupInGame;
	[SerializeField]
	private Text						_scoreText;
	[SerializeField]	
	private Slider						_scoreSlider;
	[SerializeField]
	private GameObject					_itemSpotPrefab;
	[SerializeField]
	private GameObject					_itemSpotsContainer;
	private Dictionary<int, GameObject>	_itemsDictionaryByScore = new Dictionary<int, GameObject>();
	private Color						_itemDeactivatedColor 	= new Color(0f, 0f, 0f, 0.1f);			//Grey for deactivated item on score tab.
}
