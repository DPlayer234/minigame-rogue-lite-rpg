namespace SAE.RoguePG.Main.Dungeon
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Generates a floor when attached to a GameObject.
    ///     Rooms are generated on a grid.
    /// </summary>
    public partial class DungeonGenerator
    {
        /// <summary>
        ///     The current <see cref="DungeonGenerator"/> instance
        /// </summary>
        private static DungeonGenerator Instance { get; set; }

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
        ///     The wall blocking the floor transition (<seealso cref="floorTransitionBlockingWall"/>)
        /// </summary>
        public static GameObject CurrentFloorTransitionBlockingWall
        {
            get
            {
                return DungeonGenerator.Instance.floorTransitionBlockingWall;
            }
        }

        /// <summary>
        ///     Parts currently used by the dungeon generator. 
        /// </summary>
        public static DungeonPrefabs CurrentParts
        {
            get
            {
                return DungeonGenerator.Instance.parts;
            }
        }

        /// <summary>
        ///     Initializes and updates static fields and properties
        /// </summary>
        private void InitializeStatic()
        {
            DungeonGenerator.Instance = this;
        }

        /// <summary>
        ///     Creates the floor transition
        /// </summary>
        private void CreateFloorTransition()
        {
            if (DungeonGenerator.Instance == null)
            {
                throw new Exceptions.DungeonGeneratorException("There is no active instance of the dungeon generator.");
            }

            if (DungeonGenerator.Instance.floorTransitionBlockingWall == null)
            {
                throw new Exceptions.DungeonGeneratorException("There is no position for the floor transition set!");
            }

            // Spawn floor transition
            GameObject floorTransition = MonoBehaviour.Instantiate(DungeonGenerator.CurrentParts.floorTransition, DungeonGenerator.Instance.roomParent.transform);
            floorTransition.transform.position = DungeonGenerator.CurrentFloorTransitionBlockingWall.transform.position;
            floorTransition.transform.rotation = DungeonGenerator.CurrentFloorTransitionBlockingWall.transform.rotation;

            // Delete the wall
            MonoBehaviour.Destroy(DungeonGenerator.CurrentFloorTransitionBlockingWall);
        }
    }
}