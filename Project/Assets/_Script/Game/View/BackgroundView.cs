using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BackgroundView : View
{
	public ParticleSystem	ParticlesDust;
	public ParticleSystem	ParticleWorryDust;
	public List<SpriteRenderer> TiledRenderers;

	private Sequence _tilingSequence = null;

	void Start()
	{
		if (_tilingSequence == null)
		{
			_tilingSequence = DOTween.Sequence ();

			foreach(SpriteRenderer tiledBG in TiledRenderers)
			{
			_tilingSequence
					.Append(DOVirtual.Float(GM.Instance.ScreenSize.x, GM.Instance.ScreenSize.x*3.6f, 3f, (val)=>
					{
						tiledBG.size = new Vector2(val, tiledBG.size.y);
					}).SetEase(Ease.Linear))
					.SetLoops(-1);

			}
		}
	}
}

