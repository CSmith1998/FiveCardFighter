using System;
using UnityEngine;

public static class GameDetails {
    #region Round Details
    [HideInInspector] public static bool HeroTurnSkipped = false;
    [HideInInspector] public static bool HeroSelectedAction = false;

    [HideInInspector] public static bool WaitingForNewRound = false;
    #endregion

    #region Round Timer
    [Header("Default Value")]
    public static float SelectionTimer;
    [HideInInspector] public static float TimeRemaining;
    #endregion
}