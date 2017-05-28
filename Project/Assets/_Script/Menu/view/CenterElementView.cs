using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class CenterElementView : ThemeElementView
{
	public CenterElementId ElementId;

	void Start()
	{
		RegisterEvents ();
	}

	void OnDestroy()
	{
		UnregisterEvents ();
	}

	private void OnUIThemeChanged(UITheme menuTheme)
	{
		if (GraphicIcons == null || GraphicIcons.Length <= 0)
			return;

		foreach (Graphic element in GraphicIcons)
		{
			element.DOColor(menuTheme.IconsColor, 0.3f);
		}
	}

	private void OnButtonClicked()
	{
		Notify (N.CenterButtonPressed_, NotifyType.UI, ElementId);
	}

	private void RegisterEvents()
	{
		if (ButtonBackground != null)
		{
			ButtonBackground.onClick.AddListener( OnButtonClicked);
		}

		ui.controller.RightButtonsController.ActionUIThemeChanged += OnUIThemeChanged;
	}

	private void UnregisterEvents()
	{
		if (ButtonBackground != null)
		{
			ButtonBackground.onClick.RemoveListener(OnButtonClicked);
		}

		if(ui.controller.RightButtonsController != null)
			ui.controller.RightButtonsController.ActionUIThemeChanged -= OnUIThemeChanged;
	}
}

