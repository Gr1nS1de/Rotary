using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerSkinView : View
{
	public Image 				SkinImage;
	public CanvasGroup 			SkinPricePanel;
	public RuntimeAnimatorController	SkinAnimationController;
	[HideInInspector]
	public int	 				SkinId;
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
		SkinImage.sprite = SkinSprite;

		if(isActive)
			SetSkinActive ();
	}

	private void OnButtonClick()
	{
		Notify (N.OnPlayerSelectSkin___, NotifyType.ALL, SkinId, IsActive, IsAvailable);
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
		}
		else
		{
			_availableSequence.Rewind ();
		}
	}

	public void SetSkinActive()
	{
		SetAvailable (false);
		IsActive = true;
		SkinPricePanel.DOFade(0f, 0.3f);
		SkinPricePanel.transform.DOPunchScale (new Vector3 (0.3f, 0.3f, 0f), 0.3f);

		if(_availableSequence != null)
			_availableSequence.Rewind ();
	}

}

