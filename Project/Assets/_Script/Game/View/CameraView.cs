using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class CameraView : View<GameActivity>
{
	public Camera 			Camera;

	private CameraModel 	_cameraModel	{ get { return game.model.cameraModel; } }
	private PlayerView		_playerView		{ get { return game.view.playerView; } }
	private float			_moveSpeed		{ get { return _cameraModel.moveSpeed; } }

	void Awake()
	{
		Camera = GetComponent<Camera> ();
	}

	void Start()
	{

	}

	public void OnInit()
	{
		Vector3 cameraPosition = Camera.transform.position;

		cameraPosition.x += GM.Instance.ScreenSize.x;

		transform.DOMove (cameraPosition, 0.5f).OnComplete (() =>
		{
			Notify(N.GamePlay);
		}).SetEase(Ease.Linear);
		
	}

	void Update()
	{
		if (game.model.gameState != GameState.Playing)
			return;

		Vector3 cameraPosition = Camera.transform.position;

		cameraPosition.x += _moveSpeed * game.model.gameSpeed;

		Camera.transform.position = Vector3.Lerp(Camera.transform.position, cameraPosition, Time.deltaTime);
	}

	//void Update()
	//{
		//cam.backgroundColor = GM.instance.currentBackgroundColor;
	//}

	public void DOStart(System.Action callback)
	{/*
		cam.DOOrthoSize(playZoomSize, 0.7f)
			.SetEase(Ease.OutBack)
			.OnComplete(() => {
				DOVirtual.DelayedCall(0.2f, () => {
					if(callback != null)
						callback();
				});
			});*/
	}

	/*
	public void DOShake()
	{
		if(DOTween.IsTweening(cam))
			return;

		cam.DOOrthoSize(playZoomSize * 1.03f, 0.002f).OnComplete(() => {
			cam.DOOrthoSize(playZoomSize * 0.97f, 0.004f).OnComplete(() => {
				cam.DOOrthoSize(playZoomSize, 0.002f).OnComplete(() => {
					cam.orthographicSize = playZoomSize;
				});
				//				cam.DOOrthoSize(orthoSize * 1.03f, 0.04f).OnComplete(() => {
				//					cam.DOOrthoSize(orthoSize, 0.02f).OnComplete(() => {
				//						cam.orthographicSize = orthoSize;
				//					});
				////					cam.DOOrthoSize(orthoSize * 0.95f, 0.05f).OnComplete(() => {
				////						
				////					});
				//				});

			});
		});

	}
*/
}
