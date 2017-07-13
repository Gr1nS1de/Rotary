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
	private bool _isPlay = false;

	void Start()
	{
	}

	public void Init()
	{
		if (TiledRenderers.Count > 0)
		{
			foreach (MovingSprite tiledBG in TiledRenderers)
			{
				tiledBG.CurrentSize = new Vector2 (tiledBG.ItemRenderer.sprite.bounds.size.x * 3f, tiledBG.ItemRenderer.sprite.bounds.size.y);
				tiledBG.ItemRenderer.size = tiledBG.CurrentSize;
			}
		}
	}

	public void Play()
	{
		if (TiledRenderers.Count > 0)
		{
			foreach (MovingSprite tiledBG in TiledRenderers)
			{
				tiledBG.CurrentSize = new Vector2 (tiledBG.ItemRenderer.sprite.bounds.size.x * 3f, tiledBG.ItemRenderer.sprite.bounds.size.y);
				tiledBG.ItemRenderer.size = tiledBG.CurrentSize;
			}

			_isPlay = true;
		}
	}

	public void Stop()
	{
		_isPlay = false;

		foreach (MovingSprite tiledBG in TiledRenderers)
		{
			float spriteWidth = tiledBG.ItemRenderer.sprite.bounds.size.x;

			tiledBG.CurrentSize.x = spriteWidth * 3f;

			tiledBG.ItemRenderer.size = tiledBG.CurrentSize;
		}
	}

	void Update()
	{
		if (_isPlay)
		{
			foreach (MovingSprite tiledBG in TiledRenderers)
			{
				float spriteWidth = tiledBG.ItemRenderer.sprite.bounds.size.x;

				tiledBG.CurrentSize.x = Mathf.Clamp(tiledBG.CurrentSize.x + tiledBG.MoveSpeed * game.model.gameSpeed * Time.deltaTime, spriteWidth * 3f, spriteWidth * 9f);

				if (tiledBG.CurrentSize.x >= spriteWidth * 8.99f)
				{
					tiledBG.CurrentSize.x = spriteWidth * 3f;
				}

				tiledBG.ItemRenderer.size = tiledBG.CurrentSize;
			}
		}
	}
		

	void OnDestroy()
	{
		if (_tilingSequence != null && _tilingSequence.IsActive ())
			_tilingSequence.Kill ();
	}
}

