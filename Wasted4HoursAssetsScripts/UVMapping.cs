namespace SAE.RoguePG.Main.Dungeon
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Stores information about how the UV map should be set
    /// </summary>
    [Serializable]
    public class UVMapping
    {
        /// <summary> Which set GameObject name is used as a default </summary>
        public const string AnyGameObjectName = "*";

        /// <summary> The name of the original GameObject. <seealso cref="AnyGameObjectName"/> means any. </summary>
        public string gameObjectName = UVMapping.AnyGameObjectName;

        /// <summary> The UV area </summary>
        public Vector2[] uv;

        /// <summary> The default UV Mapping </summary>
        private static UVMapping defaultMapping;

        /// <summary>
        ///     Initializes the static fields and properites of the <see cref="UVMapping"/> class
        /// </summary>
        static UVMapping()
        {
            UVMapping.defaultMapping = new UVMapping();

            UVMapping.defaultMapping.gameObjectName = UVMapping.AnyGameObjectName;

            UVMapping.defaultMapping.uv = new Vector2[]
            {
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f)
            };
        }

        /// <summary> Get a copy of the default UV Mapping </summary>
        public static UVMapping DefaultMapping
        {
            get
            {
                UVMapping defaultCopy = new UVMapping();

                defaultCopy.gameObjectName = UVMapping.defaultMapping.gameObjectName;

                defaultCopy.uv = new Vector2[UVMapping.defaultMapping.uv.Length];
                for (int i = 0; i < UVMapping.defaultMapping.uv.Length; i++)
                {
                    defaultCopy.uv[i] = UVMapping.defaultMapping.uv[i];
                }

                return defaultCopy;
            }
        }
    }

    /// <summary>
    ///     Contains extension methods for use with the <seealso cref="UVMapping"/> class
    /// </summary>
    public static class UVMappingExtension
    {
        /// <summary>
        ///     Returns the UV mapping for the specified name or <seealso cref="UVMapping.DefaultMapping"/>
        ///     if none is found.
        /// </summary>
        /// <param name="mappings">An enumarable of mappings</param>
        /// <param name="name">The name of the GameObject</param>
        /// <returns>A fitting UVMapping or <seealso cref="UVMapping.DefaultMapping"/></returns>
        public static UVMapping GetMappingFor(this IEnumerable<UVMapping> mappings, string name)
        {
            UVMapping @default = null;

            foreach (UVMapping mapping in mappings)
            {
                if (mapping.gameObjectName == name)
                {
                    return mapping;
                }
                else if (@default == null && mapping.gameObjectName == UVMapping.AnyGameObjectName)
                {
                    @default = mapping;
                }
            }

            return @default != null ? @default : UVMapping.DefaultMapping;
        }
    }
}
