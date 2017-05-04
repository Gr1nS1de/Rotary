using UnityEngine;
using System.Collections;

public abstract class UIApplication<M, V, C> : UIApplication
	where M : Element
	where V : Element
	where C : Element
{
	public M model			{ get { return (M)(object)base.model; } }
	public V view			{ get { return (V)(object)base.view; } }
	public C controller		{ get { return (C)(object)base.controller; } }
}

public class UIApplication : BaseApplication
{
	public UIModel			model				{ get { return _model 			= SearchLocal<UIModel>(_model, typeof(UIModel).ToString()); } }
	public UIView			view				{ get { return _view			= SearchLocal<UIView>(_view, typeof(UIView).ToString()); } }
	public UIController		controller			{ get { return _controller		= SearchLocal<UIController>(_controller, typeof(UIController).ToString()); } }

	private UIModel			_model;
	private UIView        	_view;
	private UIController  	_controller;

	private void Awake()
	{

	}

	private void Start()
	{

	}
}

