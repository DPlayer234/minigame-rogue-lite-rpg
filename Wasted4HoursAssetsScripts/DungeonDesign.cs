namespace SAE.RoguePG.Main.Dungeon
{
    using System;
    using UnityEngine;

    /// <summary>
    ///     Stores information about the design of the dungeon
    /// </summary>
    [Serializable]
    public class DungeonDesign
    {
        /// <summary>
        ///     The material in use by the dungeon
        /// </summary>
        public Material material;

        /// <summary>
        ///     The UV Mapping in use by the dungeon rooms
        /// </summary>
        public UVMapping[] uvMappings;
    }
}
