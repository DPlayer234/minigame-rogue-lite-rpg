//-----------------------------------------------------------------------
// <copyright file="DungeonGeneratorStatic.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

﻿namespace DPlay.RoguePG.Main.Dungeon
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Generates a floor when attached to a GameObject.
    ///     Rooms are generated on a grid.
    /// </summary>
    public partial class DungeonGenerator
    {
        /// <summary>
        ///     Whether a floor is being generated already.
        /// </summary>
        private static bool isGeneratingFloor = false;

        /// <summary>
        ///     The current layout of the floor (<seealso cref="floorLayout"/>)
        /// </summary>
        public static Dictionary<Vector2Int, RoomType> CurrentFloorLayout
        {
            get
            {
                return DungeonGenerator.Instance.floorLayout;
            }
        }

        /// <summary>
        ///     The number of the current floor.
        /// </summary>
        public static int FloorNumber
        {
            get
            {
                return DungeonGenerator.Instance.floorNumber;
            }
        }

        /// <summary>
        ///     The current amount of rooms on the floor (<seealso cref="TotalFloorSize"/>)
        /// </summary>
        public static int CurrentFloorSize
        {
            get
            {
                return DungeonGenerator.Instance.TotalFloorSize;
            }
        }

        /// <summary>
        ///     Parent transform for all entities
        /// </summary>
        public static Transform EntityParent
        {
            get
            {
                return DungeonGenerator.Instance.entityParent;
            }
        }

        /// <summary>
        ///     Parent transform for the dungeon rooms and co.
        /// </summary>
        public static Transform RoomParent
        {
            get
            {
                return DungeonGenerator.Instance.roomParent;
            }
        }

        /// <summary>
        ///     The design of the dungeon
        /// </summary>
        public static DungeonDesign Design
        {
            get
            {
                return DungeonGenerator.Instance.design;
            }
        }

        /// <summary>
        ///     The wall blocking the floor transition (<seealso cref="floorTransitionBlockingWall"/>)
        /// </summary>
        private static GameObject CurrentFloorTransitionBlockingWall
        {
            get
            {
                return DungeonGenerator.Instance.floorTransitionBlockingWall;
            }
        }

        /// <summary>
        ///     Parts currently used by the dungeon generator. 
        /// </summary>
        private static DungeonPrefabs CurrentParts
        {
            get
            {
                return DungeonGenerator.Instance.parts;
            }
        }

        /// <summary>
        ///     Generates the next floor.
        /// </summary>
        public static void GoToNextFloor()
        {
            if (DungeonGenerator.Instance == null)
            {
                throw new RPGException(RPGException.Cause.DungeonNoActiveInstance);
            }

            if (!DungeonGenerator.isGeneratingFloor)
            {
                DungeonGenerator.isGeneratingFloor = true;

                ++DungeonGenerator.Instance.floorNumber;
                DungeonGenerator.Instance.StartCoroutine(DungeonGenerator.Instance.GenerateFloorCo());
            }
        }

        /// <summary>
        ///     Creates the floor transition
        /// </summary>
        public static void CreateFloorTransition()
        {
            if (DungeonGenerator.Instance == null)
            {
                throw new RPGException(RPGException.Cause.DungeonNoActiveInstance);
            }

            if (DungeonGenerator.Instance.floorTransitionBlockingWall == null)
            {
                throw new RPGException(RPGException.Cause.DungeonNoFloorTransition);
            }

            // Spawn floor transition
            GameObject floorTransition = MonoBehaviour.Instantiate(DungeonGenerator.CurrentParts.floorTransition, DungeonGenerator.Instance.roomParent.transform);
            floorTransition.transform.position = DungeonGenerator.CurrentFloorTransitionBlockingWall.transform.position;
            floorTransition.transform.rotation = DungeonGenerator.CurrentFloorTransitionBlockingWall.transform.rotation;

            DungeonGenerator.Instance.ApplyDesign(floorTransition.transform);

            // Delete the wall
            MonoBehaviour.Destroy(DungeonGenerator.CurrentFloorTransitionBlockingWall);
        }

        /// <summary>
        ///     Initializes and updates static fields and properties
        /// </summary>
        private void InitializeStatic()
        {
            this.NewPreferThis();

            MusicManager.PlayMusic(this.design.backgroundMusic);
        }

        /// <summary>
        ///     Generates the floor within a coroutine. (Not immediately.)
        /// </summary>
        /// <returns>An enumator</returns>
        private IEnumerator GenerateFloorCo()
        {
            yield return null;

            this.GenerateFloor();

            DungeonGenerator.isGeneratingFloor = false;
        }
    }
}