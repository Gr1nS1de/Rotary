using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public enum NoticeType
{
	BuySkin,
	BuyTheme,
	PurchaseStore,
}

public class NoticeItem
{
	public NoticeType 	NoticeType;
	public Sprite		NoticeIcon;
	public string		NoticeTitle;
	public string		NoticeDescription;
}

public class NoticeController : Controller
{
	private NoticeModel			_noticeModel				{ get {return ui.model.noticeModel;} }
	private Queue<NoticeItem> 	_noticeQueue 				= new Queue<NoticeItem> ();
	private Sequence 			_noticePanelMoveSequence	= null;
	private bool				_isNoticeShow				= false;

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.OnPlayerBuySkin_:
				{
					//int skinId = (int)data [0];

					AddNoticeToQueue (new NoticeItem 
					{
						NoticeType = NoticeType.BuySkin,
						NoticeIcon = null,
						NoticeTitle = string.Format("{0}", Localization.CheckKey("TK_NOTICE_BUY_TITLE").ToUpper()),
						NoticeDescription = string.Format("{0}", Localization.CheckKey("TK_NOTICE_BUY_SKIN_DESCRIPTION").ToUpper())
					});
					break;
				}

			case N.OnPurchasedDoubleCoin:
				{
					AddNoticeToQueue (new NoticeItem 
					{
						NoticeType = NoticeType.PurchaseStore,
						NoticeIcon = null,
						NoticeTitle = string.Format("{0}", Localization.CheckKey("TK_NOTICE_PURCHASE_TITLE").ToUpper()),
						NoticeDescription = string.Format("{0}", Localization.CheckKey("TK_NOTICE_PURCHASE_DOUBLE_COIN_DESCRIPTION").ToUpper())
					});
					break;
				}

			case N.OnPurchasedCoinsPack_00:
				{
					AddNoticeToQueue (new NoticeItem 
					{
						NoticeType = NoticeType.PurchaseStore,
						NoticeIcon = null,
						NoticeTitle = string.Format("{0}", Localization.CheckKey("TK_NOTICE_PURCHASE_TITLE").ToUpper()),
						NoticeDescription = string.Format("{0}", Localization.CheckKey("TK_NOTICE_PURCHASE_COINS_PACK_00_DESCRIPTION").ToUpper())
					});
					break;
				}

			case N.OnPurchasedCoinsPack_01:
				{
					AddNoticeToQueue (new NoticeItem 
					{
						NoticeType = NoticeType.PurchaseStore,
						NoticeIcon = null,
						NoticeTitle = string.Format("{0}", Localization.CheckKey("TK_NOTICE_PURCHASE_TITLE").ToUpper()),
						NoticeDescription = string.Format("{0}", Localization.CheckKey("TK_NOTICE_PURCHASE_COINS_PACK_01_DESCRIPTION").ToUpper())
					});
					break;
				}
		}

	}

	private void OnStart()
	{

	}

	private void AddNoticeToQueue(NoticeItem noticeItem)
	{
		_noticeQueue.Enqueue (noticeItem);

		if (!_isNoticeShow)
		{
			ShowNotice (_noticeQueue.Dequeue());
		}
	}

	private void ShowNotice(NoticeItem noticeItem)
	{
		_noticeModel.textNoticeTitle.text = noticeItem.NoticeTitle;
		_noticeModel.textNoticeDescription.text = noticeItem.NoticeDescription;

		if (noticeItem.NoticeIcon != null)
			_noticeModel.imageNoticeIcon.sprite = noticeItem.NoticeIcon;

		if (_noticePanelMoveSequence == null)
		{
			RectTransform noticePanelRect = _noticeModel.panelNotice.GetComponent<RectTransform> ();
			float noticePanelHeight = noticePanelRect.rect.height;

			_noticePanelMoveSequence = DOTween.Sequence ();

			_noticePanelMoveSequence
				.Append (noticePanelRect.DOAnchorPosY (-noticePanelHeight, 0.5f))
				.AppendInterval (3f)
				.Append (noticePanelRect.DOAnchorPosY (0f, 0.5f))
				.SetAutoKill (false)
				.SetRecyclable (true)
				.OnComplete (() =>
				{
						OnEndShowNotice();		
				});
		}

		_isNoticeShow = true;
		_noticePanelMoveSequence.Restart ();
	}

	private void OnEndShowNotice()
	{
		_isNoticeShow = false;
		CheckNoticeQueue ();
	}

	private void CheckNoticeQueue()
	{
		if (_noticeQueue.Count > 0)
		{
			ShowNotice (_noticeQueue.Dequeue());
		}
	}

}

