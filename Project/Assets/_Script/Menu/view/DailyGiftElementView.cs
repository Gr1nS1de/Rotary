using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DailyGiftElementView : View
{
	public DailyGiftElementId 		ElementId;
	public bool						IsActive;

	private Button 					_button;

	void Start()
	{
		_button = GetComponent<Button> ();
		_button.onClick.AddListener (OnButtonClick);
	}

	public void SetActive(bool isActive)
	{
		IsActive = isActive;
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

