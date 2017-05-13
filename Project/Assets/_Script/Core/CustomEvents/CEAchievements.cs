using UnityEngine;
using System.Collections;

public enum AchievementsType
{
    ActionSvdKill,
    ActionAkKill,
    ActionSteyrKill,
    ActionFinishBattle,
    ActionKill,
    ActionWin,
    ActionOneShot,
    ActionHeadShot,
    ActionHeal,
    ActionFullWin,
    ActionSerialKill,
    ActionSerialKillLen,
    ActionRevenge,
    ActionBuyRifle,
    ActionBuyGun,
    ActionBuyArmor
}

public class CEAchievements : CustomEvent
{
    public AchievementsType AchievementsType;
}
