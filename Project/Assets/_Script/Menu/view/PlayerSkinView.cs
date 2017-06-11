using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerSkinView : View
{
	public CanvasGroup 			SkinPricePanel;
	[HideInInspector]
	public string 				SkinId;
	[HideInInspector]
	public Sprite		 		SkinSprite;
	[HideInInspector]
	public int					SkinPrice;
	public Text					SkinPriceText;
	public bool					IsActive;
	public bool					IsAvailable			= false;

	private Button 				_button;
	private Sequence 			_availableSequence;

	public void OnInit(int skinPrice, bool isActive)
	{
		_button = GetComponent<Button> ();
		_button.onClick.AddListener (OnButtonClick);

		InitAvailableSequence ();

		SkinPrice = skinPrice;
		SkinPriceText.text = string.Format ("{0}", skinPrice);

		if(isActive)
			SetSkinActive ();
	}

	private void OnButtonClick()
	{
		if (!IsActive && IsAvailable)
		{
			Notify (N.OnPlayerBuySkin_, NotifyType.ALL, SkinId);
		}
		else if (IsActive)
		{
			Notify (N.OnPlayerSelectSkin_, NotifyType.ALL, SkinId);
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
		if (IsActive)
			return;
		
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

			if (_button != null)
			{
				_button.interactable = false;
			}
		}
	}

	public void SetSkinActive()
	{
		IsActive = true;
		IsAvailable = false;
		SkinPricePanel.alpha = 0;

		if(_availableSequence != null)
			_availableSequence.Rewind ();
	}

}

