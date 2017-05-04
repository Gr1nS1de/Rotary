using UnityEngine;
using System.Collections;

public abstract class GameApplication<M, V, C> : GameApplication
	where M : Element
	where V : Element
	where C : Element
{
	public M model			{ get { return (M)(object)base.model; } }
	public V view			{ get { return (V)(object)base.view; } }
	public C controller		{ get { return (C)(object)base.controller; } }
}

public class GameApplication : BaseApplication
{
	public GameModel		model				{ get { return _model 			= SearchLocal<GameModel>(_model, typeof(GameModel).ToString()); } }
	public GameView			view				{ get { return _view			= SearchLocal<GameView>(_view, typeof(GameView).ToString()); } }
	public GameController	controller			{ get { return _controller		= SearchLocal<GameController>(_controller, typeof(GameController).ToString()); } }

	private GameModel		_model;
	private GameView        _view;
	private GameController  _controller;

	private void Awake()
	{

	}

	private void Start()
	{

	}
}

