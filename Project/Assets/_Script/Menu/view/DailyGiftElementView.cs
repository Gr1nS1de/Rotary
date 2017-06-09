using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class DailyGiftElementView : View
{
	public DailyGiftElementId 		ElementId;
	public bool						IsActive;

	private Button 					_button;
	private Sequence 				_activeSequence;

	void Start()
	{
		_button = GetComponent<Button> ();
		_button.onClick.AddListener (OnButtonClick);

		InitActiveSequence ();
	}

	private void InitActiveSequence()
	{
		_activeSequence = DOTween.Sequence ();

		_activeSequence.SetAutoKill (false).SetRecyclable (true);
		_activeSequence
			.Append(transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.5f, 1))
			.SetLoops(-1);

		_activeSequence.Rewind ();
	}


	public void SetActive(bool isActive)
	{
		IsActive = isActive;

		if (isActive)
		{
			_activeSequence.Play ();
		}
		else
		{
			_activeSequence.Rewind ();
		}
	}

	private void OnButtonClick()
	{
		if (IsActive)
		{
			SetActive (false);
			Notify (N.OnClickDailtGiftElement_, NotifyType.ALL, ElementId);
		}
	}
}

