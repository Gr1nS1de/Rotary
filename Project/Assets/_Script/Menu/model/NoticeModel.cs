using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NoticeModel : Model
{
	public CanvasGroup					panelNotice					{ get { return _panelNotice; } }
	public Text							textNoticeTitle				{ get { return _textNoticeTitle; } }
	public Text							textNoticeDescription		{ get { return _textNoticeDescription; } }
	public Image						imageNoticeIcon				{ get { return _imageNoticeIcon; } }

	[SerializeField]
	private Image						_imageNoticeIcon;
	[SerializeField]
	private Text						_textNoticeDescription;
	[SerializeField]
	private Text						_textNoticeTitle;
	[SerializeField]
	private CanvasGroup					_panelNotice;
}

