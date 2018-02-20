//-----------------------------------------------------------------------
// <copyright file="GetBattleAction.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Main.BattleAction
{
    using DPlay.RoguePG.Main.BattleAction.Actions;
    using DPlay.RoguePG.Main.BattleDriver;

    /// <summary>
    ///     Base class for any action that can be taken during a turn.
    /// </summary>
    public abstract partial class BattleAction
    {
        // Additional things related to BattleActions

        /// <summary>
        ///     Pseudo-Pointer to action classes
        /// </summary>
        public enum ActionClass
        {
            AOEHeal,
            Bravery,
            FireMagic,
            MajorHeal,
            MinorHeal,
            Smash,
            QuickSlash,
            NoAction
        }

        /// <summary>
        ///     Returns a new instance of a battle action class based on an enum
        /// </summary>
        /// <param name="action">The action to get</param>
        /// <param name="user">The user to initialize it with</param>
        /// <returns>A new instance of a battle action</returns>
        public static BattleAction GetBattleAction(ActionClass action, BaseBattleDriver user)
        {
            switch (action)
            {
                case ActionClass.AOEHeal:
                    return new AOEHeal(user);
                case ActionClass.Bravery:
                    return new Bravery(user);
                case ActionClass.FireMagic:
                    return new FireMagic(user);
                case ActionClass.MajorHeal:
                    return new MajorHeal(user);
                case ActionClass.MinorHeal:
                    return new MinorHeal(user);
                case ActionClass.Smash:
                    return new Smash(user);
                case ActionClass.QuickSlash:
                    return new QuickSlash(user);
                case ActionClass.NoAction:
                    return new NoAction(user);
                default:
                    // Default to smash
                    return new Smash(user);
            }
        }
    }
}
