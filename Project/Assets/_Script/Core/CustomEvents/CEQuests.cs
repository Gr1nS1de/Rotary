using UnityEngine;
using System.Collections;

public enum QuestTypes
{
    QActionAll,
    QActionDeathmatch,
    QActionSquadmatch,
    QActionKill,
    QActionHeadshot,
    QActionRealBought,
    QActionShare,
    QActionPoints,
    QActionUpgrade,
    QActionCamuflage
}

public class CEQuests : CustomEvent
{
    public QuestTypes QuestType;
}
