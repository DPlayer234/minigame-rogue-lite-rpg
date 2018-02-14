namespace SAE.RoguePG.Main.Dungeon
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
        ///     The current <see cref="DungeonGenerator"/> instance
        /// </summary>
        private static DungeonGenerator Instance { get; set; }

        /// <summary>
        ///     The current layout of the floor (<seealso cref="floorLayout"/>)
        /// </summary>
        private static Dictionary<Vector2Int, RoomType> CurrentFloorLayout
        {
            get
            {
                return DungeonGenerator.Instance.floorLayout;
            }
        }

        /// <summary>
        ///     The current amount of rooms on the floor (<seealso cref="TotalFloorSize"/>)
        /// </summary>
        private static int CurrentFloorSize
        {
            get
            {
                return DungeonGenerator.Instance.TotalFloorSize;
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
                throw new Exceptions.DungeonGeneratorException("There is no active instance of the dungeon generator.");
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

        /// <summary>
        ///     Initializes and updates static fields and properties
        /// </summary>
        private void InitializeStatic()
        {
            DungeonGenerator.Instance = this;
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