using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerSkinView : View
{
	public Image				SkinImage;
	public CanvasGroup 			SkinPricePanel;
	[HideInInspector]
	public string 				SkinId;
	[HideInInspector]
	public Sprite		 		SkinSprite;
	[HideInInspector]
	public int					SkinPrice;
	public bool					IsActive;
	public bool					IsAvailable			= false;

	private Button 				_button;
	private Sequence 			_availableSequence;

	void Start()
	{		
		_button = GetComponent<Button> ();
		_button.onClick.AddListener (OnButtonClick);

		InitAvailableSequence ();
	}

	public void OnInit(int skinPrice, bool isActive)
	{
		SkinImage.sprite = SkinSprite;
		SkinPrice = skinPrice;

		if(isActive)
			SetSkinActive ();
	}

	private void OnButtonClick()
	{
		if (!IsActive && IsAvailable)
		{
			SetSkinActive ();
			Notify (N.OnPlayerBuySkin_, NotifyType.ALL, SkinId);
		}
	}

	private void InitAvailableSequence()
	{
		_availableSequence = DOTween.Sequence ();

		_availableSequence.SetAutoKill (false).SetRecyclable (true);
		_availableSequence
			.Append(transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.5f, 1))
			.SetLoops(-1);

		_availableSequence.Rewind ();
	}

	public void SetAvailable(bool isAvailable)
	{
		IsAvailable = isAvailable;

		if (isAvailable)
		{
			_availableSequence.Play ();

			if(_button != null)
				_button.interactable = true;
		}
		else
		{
			_availableSequence.Rewind ();

			if(_button != null && !IsActive)
				_button.interactable = false;
		}
	}

	public void SetSkinActive()
	{
		IsActive = true;
		SkinPricePanel.alpha = 0;
		SkinImage.DOFade (1f, 0.3f);
	}

}

