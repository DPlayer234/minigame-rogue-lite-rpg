using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SAE.RoguePG.Main.BattleDriver;

namespace SAE.RoguePG.Main.BattleActions
{
    /// <summary>
    ///     Base class for any action that can be taken during a turn.
    /// </summary>
    public abstract partial class BattleAction
    {
        // Additional things related to BattleActions

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
                case ActionClass.Smash:
                    return new Smash(user);

                case ActionClass.MinorHeal:
                    return new MinorHeal(user);

                default:
                    // Default to smash
                    return new Smash(user);
            }
        }

        /// <summary>
        ///     Pseudo-Pointer to action classes
        /// </summary>
        public enum ActionClass
        {
            Smash,
            MinorHeal
        }
    }
}
