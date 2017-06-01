using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class RightElementView : ThemeElementView
{
	public RightElementId ElementId;

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
		Notify (N.RightButtonPressed_, NotifyType.UI, ElementId);
	}

	private void RegisterEvents()
	{
		if (ButtonBackground != null)
		{
			ButtonBackground.onClick.AddListener(OnButtonClicked);
		}

		ui.controller.RightElementsController.ActionUIThemeChanged += OnUIThemeChanged;
	}

	private void UnregisterEvents()
	{
		if (ButtonBackground != null)
		{
			ButtonBackground.onClick.RemoveListener( OnButtonClicked);
		}

		if(ui.controller.RightElementsController != null)
			ui.controller.RightElementsController.ActionUIThemeChanged -= OnUIThemeChanged;
	}
}

