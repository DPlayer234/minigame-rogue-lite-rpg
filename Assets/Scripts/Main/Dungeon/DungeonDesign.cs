namespace SAE.RoguePG.Main.Dungeon
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Stores information about the dungeon design
    /// </summary>
    [Serializable]
    public class DungeonDesign
    {
        /// <summary>
        ///     The intensity of point lights
        /// </summary>
        public float pointLightIntensity = 3.0f;

        /// <summary>
        ///     All material-tag pairs
        /// </summary>
        public TagMaterialPair[] materials;

        /// <summary>
        ///     Stores a tag and a material
        /// </summary>
        [Serializable]
        public class TagMaterialPair
        {
            /// <summary>
            ///     The tag
            /// </summary>
            public string tag;

            /// <summary>
            ///     The material
            /// </summary>
            public Material material;
        }
    }
}
