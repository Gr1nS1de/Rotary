using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerView : View
{
	private PlayerModel _playerModel	{ get { return game.model.playerModel; } }
	private Rigidbody2D _playerRB;

	void Awake()
	{
		_playerRB = GetComponent<Rigidbody2D> ();
	}

	public void OnInit()
	{
	}

	void FixedUpdate()
	{
		if (game.model.gameState != GameState.PLAYING)
			return;

		_playerRB.angularVelocity = _playerModel.angularSpeed;

		//_playerRB.DOMoveX (transform.position.x + _playerModel.moveSpeed, 1f).SetUpdate(UpdateType.Fixed);
		//transform.position += new Vector3( _playerModel.moveSpeed * Time.fixedDeltaTime, 0f, 0f);
	}



}
