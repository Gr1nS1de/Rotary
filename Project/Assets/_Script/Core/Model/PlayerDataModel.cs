using UnityEngine;
using System.Collections;

public class PlayerDataModel : Model
{
	public int							currentScore			{ get { return _currentScore; } 	set { _currentScore = value; } }
	public int	 						playedGamesCount		{ get { return _playedGamesCount; } set { _playedGamesCount = value;}}
	public bool							isDoubleCoin			{ get { return _isDoubleCoin; } 	set { _isDoubleCoin = value;}}
	public int							playerRecord			{ get { return _playerRecord; } 	set { _playerRecord = value;}}
	public int							coinsCount				{ get { return _coinsCount; } 		set { _coinsCount = value;}}
	public int							crystalsCount			{ get { return _crystalsCount; } 	set { _crystalsCount = value;}}

	private int							_crystalsCount;
	private int							_coinsCount;
	private int							_playerRecord;
	private bool						_isDoubleCoin;
	[SerializeField]
	private int							_playedGamesCount;
	[SerializeField]
	private int 						_currentScore;
}

