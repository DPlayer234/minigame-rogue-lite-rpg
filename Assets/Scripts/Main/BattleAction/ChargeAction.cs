//-----------------------------------------------------------------------
// <copyright file="ChargeAction.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Main.BattleAction
{
    using DPlay.RoguePG.Main.BattleDriver;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Base class for any action that charges at the enemy.
    /// </summary>
    public abstract class ChargeAction : BattleAction
    {
        /// <summary> Multiplier for the attack velocity </summary>
        protected float velocityMultiplier = 10.0f;

        /// <summary> Amount of Velocity added in Y-direction for attack </summary>
        protected float yVelocity = 2.0f;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChargeAction"/> class.
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public ChargeAction(BaseBattleDriver user) : base(user) { }

        /// <summary>
        ///     The method to run when this action is being used.
        /// </summary>
        /// <param name="target">The target battle driver</param>
        protected override void Use(BaseBattleDriver target)
        {
            this.DealDamage(target);

            this.User.StartCoroutine(this.DoCharge(target));
        }

        /// <summary>
        ///     Charges through the target.
        /// </summary>
        /// <param name="target">The target</param>
        /// <returns>An enumator</returns>
        protected IEnumerator DoCharge(BaseBattleDriver target)
        {
            this.User.IsWaitingOnAnimation = true;

            Rigidbody rigidbody = this.User.GetComponent<Rigidbody>();

            Vector3 lookAt = (target.transform.position - this.User.transform.position);
            lookAt.Normalize();
            lookAt *= this.velocityMultiplier;

            rigidbody.velocity = lookAt + new Vector3(0.0f, this.yVelocity, 0.0f);

            Func<bool> wait = delegate { return rigidbody.velocity.y > 0.0f; };

            yield return new WaitUntil(wait);
            yield return new WaitWhile(wait);

            rigidbody.velocity = Vector3.zero;

            this.User.IsWaitingOnAnimation = false;
        }
    }
}
