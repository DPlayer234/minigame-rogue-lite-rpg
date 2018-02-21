//-----------------------------------------------------------------------
// <copyright file="Storage.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG
{
    using DPlay.RoguePG.Main;
    using DPlay.RoguePG.Main.Driver;

    /// <summary>
    ///     Stores some general game state, but doesn't really do anything by itself.
    /// </summary>
    public static class Storage
    {
        /// <summary>
        ///     The selected prefab for the first player character.
        /// </summary>
        public static PlayerDriver SelectedPlayerPrefab { get; set; }

        /// <summary>
        ///     The 1st stat which gets a bonus on the first player character.
        /// </summary>
        public static Stat BonusStat1 { get; set; }

        /// <summary>
        ///     The 2nd stat which gets a bonus on the first player character.
        /// </summary>
        public static Stat BonusStat2 { get; set; }
    }
}
