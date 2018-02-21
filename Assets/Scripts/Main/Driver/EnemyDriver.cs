//-----------------------------------------------------------------------
// <copyright file="EnemyDriver.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.Driver
{
    using System.Collections;
    using DPlay.RoguePG.Main.BattleDriver;
    using UnityEngine;

    /// <summary>
    ///     Makes Enemies work.
    /// </summary>
    [RequireComponent(typeof(EnemyBattleDriver))]
    [DisallowMultipleComponent]
    public class EnemyDriver : BaseDriver
    {
        /// <summary> How far away it will detect a player </summary>
        public float targetRange = 4.0f;

        /// <summary> Whether this enemy was defeated </summary>
        [HideInInspector]
        public bool defeated;

        /// <summary> The delay in seconds between checks for the target </summary>
        private const float TargetUpdateDelay = 0.2f;

        /// <summary> The maximum distance between a player and this entity to trigger a battle </summary>
        private const float BattleTriggerRange = 0.5f;

        /// <summary> The player this enemy is currently chasing. If null, just walks around aimlessly. </summary>
        private PlayerDriver targetPlayer;

        /// <summary> The coroutine which is running <seealso cref="UpdateTarget"/> </summary>
        private Coroutine targetUpdater;

        /// <summary>
        ///     Sets the leader and the thing it's following
        /// </summary>
        /// <param name="leader">The Leader</param>
        /// <param name="following">The Following Driver</param>
        public void SetLeaderAndFollowing(EnemyDriver leader, EnemyDriver following)
        {
            this.leader = leader;
            this.following = following;
        }

        /// <summary>
        ///     Calculates and returns the top-down movement vector.
        ///     The axes are mapped X: X, Y: Z.
        /// </summary>
        /// <returns>Target Top-Down Movement Vector</returns>
        protected override Vector2 GetLeaderMovement()
        {
            return
                // Has target
                this.targetPlayer != null ?
                new Vector2(
                    this.targetPlayer.transform.position.x - this.transform.position.x,
                    this.targetPlayer.transform.position.z - this.transform.position.z) :
                
                // Idling
                Vector2.zero;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyDriver"/> whether it is or is not active.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            this.defeated = false;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyDriver"/> when it first becomes active
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="EnemyDriver"/>
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            // Start a battle if close enough and not alread game overed
            if (!GameOverHandler.IsGameOver && this.targetPlayer != null && (this.targetPlayer.transform.position - this.transform.position).sqrMagnitude < EnemyDriver.BattleTriggerRange * EnemyDriver.BattleTriggerRange)
            {
                BattleManager.StartNewBattle(
                    this.targetPlayer.battleDriver as PlayerBattleDriver,
                    (this.IsLeader ? this.battleDriver : this.Leader.battleDriver) as EnemyBattleDriver);
            }
        }

        /// <summary>
        ///     Finds an appropriate target
        /// </summary>
        private void LookForTarget()
        {
            PlayerDriver player = PlayerDriver.Party.GetLeader();

            if (player != null && this.CanSee(player.gameObject))
            {
                this.targetPlayer = player;
            }
        }

        /// <summary>
        ///     Returns whether this enemy can see the specified <seealso cref="GameObject"/>
        /// </summary>
        /// <param name="gameObject">The <seealso cref="GameObject"/> to check</param>
        /// <returns>Whether it's visible.</returns>
        private bool CanSee(GameObject gameObject)
        {
            Vector3 positionDifference = gameObject.transform.position - this.transform.position;

            if (positionDifference.sqrMagnitude < this.targetRange * this.targetRange)
            {
                RaycastHit raycastHit;

                return Physics.Raycast(this.transform.position, positionDifference, out raycastHit, this.targetRange) && raycastHit.transform.gameObject == gameObject;
            }

            return false;
        }

        /// <summary>
        ///     Started as a coroutine to update the target
        /// </summary>
        /// <returns>An iterator</returns>
        private IEnumerator UpdateTarget()
        {
            do
            {
                if (this.targetPlayer == null)
                {
                    if (Random.value < 0.2f)
                    {
                        this.spriteManager.FlipToDirection(!this.spriteManager.IsFacingRight);
                    }

                    this.LookForTarget();
                }
                else if (!this.CanSee(this.targetPlayer.gameObject))
                {
                    this.targetPlayer = null;
                }

                yield return new WaitForSeconds(TargetUpdateDelay);
            }
            while (!this.defeated);
        }

        /// <summary>
        ///     Called by Unity when this Behaviour is enabled.
        /// </summary>
        private void OnEnable()
        {
            this.targetUpdater = this.StartCoroutine(this.UpdateTarget());
        }

        /// <summary>
        ///     Called by Unity when this Behaviour is disabled.
        /// </summary>
        private void OnDisable()
        {
            if (this.targetUpdater != null) this.StopCoroutine(this.targetUpdater);
        }
    }
}
