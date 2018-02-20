//-----------------------------------------------------------------------
// <copyright file="BaseBattleDriverLeader.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Main.BattleDriver
{
    using System.Collections;
    using System.Collections.Generic;
    using DPlay.RoguePG.Dev;
    using DPlay.RoguePG.Main.BattleAction;
    using DPlay.RoguePG.Main.Driver;
    using DPlay.RoguePG.Main.Sprite3D;
    using DPlay.RoguePG.Main.UI;
    using UnityEngine;

    /// <summary>
    ///     Makes battles work.
    /// </summary>
    public abstract partial class BaseBattleDriver
    {
        /// <summary>
        ///     Is this the "leader" of the party?
        /// </summary>
        public bool IsLeader
        {
            get
            {
                return this.Allies[0] == this;
            }
        }

        /// <summary>
        ///     Creates status bars for its entire party.
        /// </summary>
        /// <param name="parent">The transform to parent them to</param>
        public void CreateStatusBars(Transform parent, StatusDisplayController prefab)
        {
            this.ThrowExceptionIfNotLeader();

            int index = 0;

            foreach (BaseBattleDriver battleDriver in this.Allies)
            {
                StatusDisplayController statusDisplay = MonoBehaviour.Instantiate(prefab, parent);

                statusDisplay.battleDriver = battleDriver;

                RectTransform rectTransform = statusDisplay.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition3D = new Vector3(
                        0.0f,
                        -StatusDisplayController.Height * index++,
                        0.0f);
                }
            }
        }

        /// <summary>
        ///     Sets the name index where needed
        /// </summary>
        public void DeduplicateBattleNamesInAllies()
        {
            this.ThrowExceptionIfNotLeader();

            // Count every occurence
            Dictionary<string, int> names = new Dictionary<string, int>();

            foreach (BaseBattleDriver ally in this.Allies)
            {
                if (names.ContainsKey(ally.battleName))
                {
                    names[ally.battleName] += 1;
                    continue;
                }

                names.Add(ally.battleName, 1);
            }

            // Add #N where needed
            Dictionary<string, int> namesYet = new Dictionary<string, int>();

            foreach (BaseBattleDriver ally in this.Allies)
            {
                if (names[ally.battleName] > 1)
                {
                    if (!namesYet.ContainsKey(ally.battleName))
                    {
                        namesYet.Add(ally.battleName, 0);
                    }

                    ally.battleNameIndex = ++namesYet[ally.battleName];
                }
            }
        }

        /// <summary>
        ///     Throws an exception if this is not the leader.
        /// </summary>
        private void ThrowExceptionIfNotLeader()
        {
            if (!this.IsLeader) throw new RPGException(RPGException.Cause.BattleDriverNotLeader);
        }
    }
}