//-----------------------------------------------------------------------
// <copyright file="DefaultStorageSetter.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG
{
    using DPlay.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Sets the default values for the <seealso cref="Storage"/> if they haven't been set yet.
    ///     Then destroys itself.
    /// </summary>
    public class DefaultStorageSetter : MonoBehaviour
    {
        [Header("Default Values")]

        /// <summary>
        ///     Default prefab to be used for the player
        /// </summary>
        public PlayerDriver selectedPlayerPrefab;

        /// <summary>
        ///     Default 1st stat which gets a bonus on the first player character
        /// </summary>
        public Main.Stat bonusStat1;

        /// <summary>
        ///     Default 2nd stat which gets a bonus on the first player character
        /// </summary>
        public Main.Stat bonusStat2;

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="DefaultStorageSetter"/> whether it is active or not
        /// </summary>
        private void Awake()
        {
            if (Storage.SelectedPlayerPrefab == null)
            {
                Storage.SelectedPlayerPrefab = this.selectedPlayerPrefab;
                Storage.BonusStat1 = this.bonusStat1;
                Storage.BonusStat2 = this.bonusStat2;
            }
            
            // 3 = this component + transform + 1 (to use '<' instead of '<=')
            if (this.GetComponentsInChildren<Component>().Length < 3)
            {
                MonoBehaviour.Destroy(this.gameObject);
            }
            else
            {
                MonoBehaviour.Destroy(this);
            }
        }
    }
}
