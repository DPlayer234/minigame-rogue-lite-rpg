namespace SAE.RoguePG
{
    using SAE.RoguePG.Main;
    using SAE.RoguePG.Main.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Stores some general game state, but doesn't really do anything by itself.
    /// </summary>
    public static class GeneralManager
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
