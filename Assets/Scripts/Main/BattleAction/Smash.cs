namespace SAE.RoguePG.Main.BattleActions
{
    using SAE.RoguePG.Main.BattleDriver;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Simple physical attack
    /// </summary>
    public class Smash : BattleAction
    {
        /// <summary> Action Name </summary>
        public const string ActionName = "Smash";

        /// <summary> Multiplier for the attack velocity </summary>
        public const float VelocityMultiplier = 10.0f;

        /// <summary> Amount of Velocity added in Y-direction for attack </summary>
        public const float YVelocity = 2.0f;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Smash"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public Smash(BaseBattleDriver user) : base(user)
        {
            this.name = ActionName;

            this.attackPointCost = 4.0f;
            this.attackPower = 10.0f;
            this.category = ActionCategory.PhysicalAttack;
            this.targetOption = ActionTargetOption.OneOpponent;
        }

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
        private IEnumerator DoCharge(BaseBattleDriver target)
        {
            this.User.IsWaitingOnAnimation = true;

            Rigidbody rigidbody = this.User.GetComponent<Rigidbody>();

            Vector3 lookAt = (target.transform.position - this.User.transform.position) * Smash.VelocityMultiplier;

            rigidbody.velocity = lookAt + new Vector3(0.0f, Smash.YVelocity, 0.0f);

            Func<bool> wait = delegate { return rigidbody.velocity.y > 0.0f; };

            yield return new WaitUntil(wait);
            yield return new WaitWhile(wait);

            rigidbody.velocity = Vector3.zero;

            this.User.IsWaitingOnAnimation = false;
        }
    }
}
