using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraView : View<GameActivity>
{
	private CameraModel 	_cameraModel	{ get { return game.model.cameraModel; } }
	private Camera 			_camera			{ get { return GetComponent<Camera> (); } }
	private PlayerView		_playerView		{ get { return game.view.playerView; } }
	private float			_moveSpeed		{ get { return _cameraModel.moveSpeed; } }

	public void OnInit()
	{
	}

	void Update()
	{
		if (game.model.gameState != GameState.Playing)
			return;

		Vector3 cameraPosition = _camera.transform.position;

		cameraPosition.x += _moveSpeed * game.model.gameSpeed;

		_camera.transform.position = Vector3.Lerp(_camera.transform.position, cameraPosition, Time.deltaTime);
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
