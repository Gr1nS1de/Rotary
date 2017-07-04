using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BackgroundView : View
{
	public ParticleSystem	ParticlesDust;
	public ParticleSystem	ParticleWorryDust;
	public List<MovingSprite> TiledRenderers;

	private Sequence _tilingSequence = null;
	private float _renderersSizeX;

	void Start()
	{
	}

	public void Play(float moveSpeed)
	{
		foreach (MovingSprite tiledBG in TiledRenderers)
		{
			tiledBG.CurrentSizeX = tiledBG.ItemRenderer.sprite.bounds.size.x * 3f;
			tiledBG.ItemRenderer.size = new Vector2 (tiledBG.ItemRenderer.sprite.bounds.size.x * 3f, tiledBG.ItemRenderer.sprite.bounds.size.y);
		}
	}

	public void Stop()
	{

	}

	void Update()
	{

	}
	/*
	public void Init()
	{

	}*/

	void OnDestroy()
	{
		if (_tilingSequence != null && _tilingSequence.IsActive ())
			_tilingSequence.Kill ();
	}
}

