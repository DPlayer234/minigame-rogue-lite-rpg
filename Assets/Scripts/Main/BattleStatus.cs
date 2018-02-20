//-----------------------------------------------------------------------
// <copyright file="BattleStatus.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DPlay.RoguePG.Main.BattleDriver;
    using DPlay.RoguePG.Main.Driver;
    using UnityEngine;
    
    /// <summary>
    ///     Class storing the battle status.
    ///     Instantiating it will load all relevant fighters.
    /// </summary>
    [Serializable]
    public class BattleStatus
    {
        /// <summary> The buffered highest turn speed. </summary>
        private float highestTurnSpeed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BattleStatus"/> class.
        ///     Assigns all entities participating in the fight based on their leaders.
        /// </summary>
        /// <param name="leaderPlayer">The leader player</param>
        /// <param name="leaderEnemy">The leader enemy</param>
        public BattleStatus(PlayerBattleDriver leaderPlayer, EnemyBattleDriver leaderEnemy)
        {
            this.DeactivatableGameObjects = new List<GameObject>();

            this.FindFightingPlayers(leaderPlayer);
            this.FindFightingEnemies(leaderEnemy);
            this.AssignFighters();

            this.highestTurnSpeed = this.GetHightestTurnSpeed();
        }

        /// <summary> The Entity whose turn it currently is. </summary>
        public BaseBattleDriver CurrentTurnOf { get; set; }

        /// <summary> All Entities taking part in the fight </summary>
        public List<BaseBattleDriver> AllFightingEntities { get; private set; }

        /// <summary> All Entities which are still fighting </summary>
        public List<BaseBattleDriver> StillFightingEntities { get; private set; }

        /// <summary> All Players currently fighting </summary>
        public List<BaseBattleDriver> FightingPlayers { get; private set; }

        /// <summary> All Enemies currently fighting </summary>
        public List<BaseBattleDriver> FightingEnemies { get; private set; }

        /// <summary> All GameObjects that can be deactivated to make room for the fighters </summary>
        public List<GameObject> DeactivatableGameObjects { get; private set; }

        /// <summary>
        ///     Gets the highest turn speed within all fighting entities.
        /// </summary>
        public float HighestTurnSpeed { get { return this.highestTurnSpeed; } }

        /// <summary>
        ///     Gets all player fighters and deactivates the ones not participating.
        ///     Assigns <seealso cref="fightingPlayers"/>.
        /// </summary>
        /// <param name="leaderPlayer">The leader player</param>
        private void FindFightingPlayers(PlayerBattleDriver leaderPlayer)
        {
            this.FightingPlayers = this.FindFighters(leaderPlayer);
        }

        /// <summary>
        ///     Gets all enemy fighters and deactivates the ones not participating.
        ///     Assigns <seealso cref="fightingEnemies"/>.
        /// </summary>
        /// <param name="leaderEnemy">The leader enemy</param>
        private void FindFightingEnemies(EnemyBattleDriver leaderEnemy)
        {
            this.FightingEnemies = this.FindFighters(leaderEnemy);
        }

        /// <summary>
        ///     Finds all fighters with the same tag as <paramref name="leaderBattleDriver"/>
        ///     and adds the ones not participating to <seealso cref="DeactivatableGameObjects"/>
        /// </summary>
        /// <param name="leaderBattleDriver">The leader of the bunch</param>
        /// <returns>A list of <seealso cref="BaseBattleDriver"/>s</returns>
        private List<BaseBattleDriver> FindFighters(BaseBattleDriver leaderBattleDriver)
        {
            // Find all GameObjects with the same tag as the leader
            GameObject[] allFighters = GameObject.FindGameObjectsWithTag(leaderBattleDriver.tag);

            List<BaseBattleDriver> listOfFighters = new List<BaseBattleDriver>();

            BaseDriver leaderDriver = leaderBattleDriver.GetComponent<BaseDriver>();

            // Check each of them
            foreach (GameObject fighter in allFighters)
            {
                BaseDriver driver = fighter.GetComponent<BaseDriver>();

                if (driver != null && (driver.Leader == leaderDriver || driver == leaderDriver))
                {
                    // Required component; should never be missing
                    listOfFighters.Add(fighter.GetComponent<BaseBattleDriver>());
                }
                else
                {
                    fighter.SetActive(false);
                    this.DeactivatableGameObjects.Add(fighter);
                }
            }

            // Sort them
            List<BaseBattleDriver> listOfFightersSorted = new List<BaseBattleDriver>(listOfFighters.Count);

            for (int i = 0; i < listOfFighters.Count; i++)
            {
                foreach (BaseBattleDriver battleDriver in listOfFighters)
                {
                    BaseDriver driver = battleDriver.entityDriver;
                    int number = 0;
                    while (driver.Following != null)
                    {
                        driver = driver.Following;
                        ++number;

                        if (number > listOfFighters.Count) throw new RPGException(RPGException.Cause.DriverLoopingFollowing);
                    }

                    if (number == i)
                    {
                        listOfFightersSorted.Add(battleDriver);
                    }
                }
            }

            return listOfFightersSorted;
        }

        /// <summary>
        ///     Assigns <seealso cref="allFightingEntities"/> using <seealso cref="fightingPlayers"/> and <seealso cref="fightingEnemies"/>
        /// </summary>
        private void AssignFighters()
        {
            this.AllFightingEntities = new List<BaseBattleDriver>();
            this.StillFightingEntities = new List<BaseBattleDriver>();

            // Setup for battle
            // For some reason, conversion of 'compatible' lists won't work, but it works with arrays.
            this.AllFightingEntities.AddRange(this.FightingPlayers);
            this.AllFightingEntities.AddRange(this.FightingEnemies);

            // Copy. May diverge during battle
            this.StillFightingEntities.AddRange(this.AllFightingEntities);
        }

        /// <summary>
        ///     Calculates and returns the highest turn speed.
        /// </summary>
        /// <returns>The highest turn speed</returns>
        private float GetHightestTurnSpeed()
        {
            float turnSpeed = 0;
            foreach (BaseBattleDriver battleDriver in this.AllFightingEntities)
            {
                turnSpeed = Mathf.Max(battleDriver.TurnSpeed, turnSpeed);
            }

            return turnSpeed;
        }
    }
}
