using UnityEngine;
using System.Collections;

public class GameSoundModel : Model
{
	public AudioClip	gameBackground				{ get { return _gameBackground; } }
	public AudioClip[]	hardObstaclesBreak			{ get { return _hardObstacleBreak; } 			set { _hardObstacleBreak = value; } }
	public AudioClip[]	destructibleObstacleBreak	{ get { return _destructibleObstacleBreak; }  	set { _destructibleObstacleBreak = value; } }

	[SerializeField]
	public AudioClip	_gameBackground				{ get { return _gameBackground; } }
	[SerializeField]
	private AudioClip[] _hardObstacleBreak;
	[SerializeField]
	private AudioClip[]	_destructibleObstacleBreak;

}
