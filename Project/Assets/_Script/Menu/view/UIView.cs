using UnityEngine;
using System.Collections;

public class UIView : View
{
	public RightElementView[]		rightElementsArray		{ get { return _rightElementsArray == null ? 	_rightElementsArray			= SearchLocal<RightElementView>(		_rightElementsArray,		typeof(RightElementView).Name) : _rightElementsArray; 	} }
	public DailyGiftElementView[]	dailyGiftElementsArray	{ get { return _dailyGiftElementsArray == null ? _dailyGiftElementsArray	= SearchLocal<DailyGiftElementView>(	_dailyGiftElementsArray,	typeof(DailyGiftElementView).Name) : _dailyGiftElementsArray; 	} }

	private DailyGiftElementView[]	_dailyGiftElementsArray = null;
	private RightElementView[]		_rightElementsArray = null;

	public RightElementView GetRightElement(RightElementId rightElementId)
	{
		RightElementView rightElementView = null;

		foreach (RightElementView rightElement in rightElementsArray)
		{
			if (rightElement.ElementId == rightElementId)
			{
				rightElementView = rightElement;
				break;
			}
		}

		return rightElementView;
	}

	public DailyGiftElementView GetDailyGiftElement(DailyGiftElementId rightElementId)
	{
		DailyGiftElementView dailyElementView = null;

		foreach (DailyGiftElementView dailyElement in dailyGiftElementsArray)
		{
			if (dailyElement.ElementId == rightElementId)
			{
				dailyElementView = dailyElement;
				break;
			}
		}

		return dailyElementView;
	}
	/*
	public MainMenuVIew				mainMenuView			{ get { return _mainMenuView		= SearchLocal<MainMenuVIew>(		_mainMenuView,			typeof(MainMenuVIew).Name );}}
	public SettingsView				settingsView			{ get { return _settingsView		= SearchLocal<SettingsView>(		_settingsView,			typeof(SettingsView).Name );}}
	public PlayerSkinView			playerSkinView			{ get { return _playerSkinView		= SearchLocal<PlayerSkinView>(		_playerSkinView,		typeof(PlayerSkinView).Name );}}
	public StoreView				storeView				{ get { return _storeView			= SearchLocal<StoreView>(			_storeView,				typeof(StoreView).Name );}}
	public LikeView					likeView				{ get { return _likeView			= SearchLocal<LikeView>(			_likeView,				typeof(LikeView).Name );}}
	public AchievementsView			achievementsView		{ get { return _achievementsView	= SearchLocal<AchievementsView>(	_achievementsView,		typeof(AchievementsView).Name );}}

	private AchievementsView		_achievementsView;
	private LikeView				_likeView;
	private StoreView				_storeView;
	private PlayerSkinView			_playerSkinView;
	private SettingsView			_settingsView;
	private MainMenuVIew			_mainMenuView;
	*/
}

