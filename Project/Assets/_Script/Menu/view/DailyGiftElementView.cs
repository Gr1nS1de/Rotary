using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class DailyGiftElementView : View
{
	public DailyGiftElementId 		ElementId;

	private Button 					_button;
	private Sequence 				_activeSequence;
	private DailyGiftState				_giftState;

	void Start()
	{
		_button = GetComponent<Button> ();
		_button.onClick.AddListener (OnButtonClick);

		GoToState (_giftState);
	}

	private void InitActiveSequence()
	{
		_activeSequence = DOTween.Sequence ();

		_activeSequence.SetAutoKill (false).SetRecyclable (true);
		_activeSequence
			.Append(transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.5f, 1))
			.SetLoops(-1);

	}


	public void GoToState(DailyGiftState giftState)
	{
		switch (giftState)
		{
			case DailyGiftState.Unavailable:
				{		

					if (transform.localScale != Vector3.one)
						transform.DOScale (Vector3.one, 0.3f);
					
					SetButtonInteractableActive (false);
					break;
				}

			case DailyGiftState.Active:
				{
					if(_activeSequence == null)
						InitActiveSequence ();

					if (transform.localScale != Vector3.one)
						transform.DOScale (Vector3.one, 0.3f);
					
					SetButtonInteractableActive (true);
					break;
				}

			case DailyGiftState.Activated:
				{
					SetButtonInteractableActive (false);
					transform.DOScale(new Vector3(0.9f, 0.9f, 1f), 0.3f);
					break;
				}
		}

		_giftState = giftState;
	}

	private void SetButtonInteractableActive(bool isActive)
	{
		if (isActive)
		{
			if(_activeSequence != null)
				_activeSequence.Play ();

			if(_button != null)
				_button.interactable = true;
		}
		else
		{
			if(_activeSequence != null)
				_activeSequence.Rewind ();

			if(_button != null)
				_button.interactable = false;
		}
	}

	private void OnButtonClick()
	{
		Notify (N.OnPlayerGetDailyGift_, NotifyType.ALL, ElementId);
	}
}

