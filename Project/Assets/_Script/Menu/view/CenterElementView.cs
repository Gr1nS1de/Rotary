using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class CenterElementView : ThemeElementView
{
	public CenterElementId ElementId;

	void Awake()
	{
		RegisterEvents ();
	}

	void OnEnable()
	{
		ChangeUITheme (ui.model.menuTheme, true);
	}

	void OnDestroy()
	{
		UnregisterEvents ();
	}

	private void OnUIThemeChanged(UITheme menuTheme)
	{
		ChangeUITheme (menuTheme);
	}

	private void ChangeUITheme(UITheme menuTheme, bool isImmediate = false)
	{
		if (GraphicIcons == null || GraphicIcons.Length <= 0)
			return;

		foreach (Graphic element in GraphicIcons)
		{
			if (element == null)
				continue;
			
			if (isImmediate)
			{
				element.color = menuTheme.IconsColor;
			}
			else
			{
				element.DOColor (menuTheme.IconsColor, 0.3f);
			}
		}
	}

	private void OnButtonClicked()
	{
		Notify (N.OnCenterButtonPressed_, NotifyType.UI, ElementId);
	}

	private void RegisterEvents()
	{
		if (ButtonBackground != null)
		{
			ButtonBackground.onClick.AddListener( OnButtonClicked);
		}

		ui.controller.CenterElementsController.ActionUIThemeChanged += OnUIThemeChanged;
	}

	private void UnregisterEvents()
	{
		if (ButtonBackground != null)
		{
			ButtonBackground.onClick.RemoveListener(OnButtonClicked);
		}

		if(ui != null && ui.controller != null && ui.controller.CenterElementsController != null)
			ui.controller.CenterElementsController.ActionUIThemeChanged -= OnUIThemeChanged;
	}
}

