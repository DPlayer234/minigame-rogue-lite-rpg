//-----------------------------------------------------------------------
// <copyright file="MinorHeal.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Main.BattleAction.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DPlay.RoguePG.Main.BattleDriver;
    using UnityEngine;

    /// <summary>
    ///     Healing action
    /// </summary>
    public class MinorHeal : HealAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MinorHeal"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public MinorHeal(BaseBattleDriver user) : base(user)
        {
            this.name = "Minor Heal";
            this.description = "Heals an ally for 25% of their Maximum Health.";

            // Storing heal-potential (fraction) in attack power.
            this.attackPower = 0.25f;

            this.attackPointCost = 6.0f;
            this.category = ActionCategory.Support;
            this.targetOption = ActionTargetOption.OneAlly;
        }
    }
}
