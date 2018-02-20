//-----------------------------------------------------------------------
// <copyright file="NoAction.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Main.BattleAction.Actions
{
    using DPlay.RoguePG.Main.BattleDriver;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Template for any action
    /// </summary>
    public class NoAction : BattleAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NoAction"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public NoAction(BaseBattleDriver user) : base(user)
        {
            this.name = "End Turn";
            this.description = "Ends the current turn.";

            this.attackPointCost = 1.0f;
            this.attackPower = 0.0f;
            this.category = ActionCategory.Undefined;
            this.targetOption = ActionTargetOption.Self;
        }

        /// <summary>
        ///     The method to run when this action is being used.
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected override void Use(BaseBattleDriver target)
        {
            if (target.TakingTurn)
            {
                target.TakingTurn = false;
            }
        }
    }
}
