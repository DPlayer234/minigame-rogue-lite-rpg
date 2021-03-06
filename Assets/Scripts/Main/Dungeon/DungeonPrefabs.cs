//-----------------------------------------------------------------------
// <copyright file="DungeonPrefabs.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.Dungeon
{
    using System;
    using UnityEngine;

    /// <summary>
    ///     Includes prefabs for dungeons
    /// </summary>
    [Serializable]
    public class DungeonPrefabs
    {
        /// <summary> The Walls to fill gaps where no room goes </summary>
        public GameObject wall;

        /// <summary> Floor transition </summary>
        public GameObject floorTransition;

        /// <summary> Random details </summary>
        public GameObject[] details;

        /// <summary>
        ///     Prefabs used for the starting rooms (<seealso cref="DungeonGenerator.RoomType.Start"/>)
        /// </summary>
        public GameObject[] startingRooms;

        /// <summary>
        ///     Prefabs used for common rooms (<seealso cref="DungeonGenerator.RoomType.Common"/>)
        /// </summary>
        public GameObject[] commonRooms;

        /// <summary>
        ///     Prefabs used for boss rooms (<seealso cref="DungeonGenerator.RoomType.Boss"/>)
        /// </summary>
        public GameObject[] bossRooms;

        /// <summary>
        ///     Prefabs used for treasure rooms (<seealso cref="DungeonGenerator.RoomType.Treasure"/>)
        /// </summary>
        public GameObject[] treasureRooms;
    }
}
