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
    public partial class DungeonGenerator : MonoBehaviour
    {
        /// <summary>
        ///     Floor Number
        /// </summary>
        public int floorNumber = 1;

        /// <summary>
        ///     Minimum and maximum enemy count per spawn point.
        /// </summary>
        public Vector2Int enemyCount;

        /// <summary>
        ///     How large the rooms are (width and depth)
        /// </summary>
        public Vector2 roomSize;

        /// <summary>
        ///     Parts used in the dungeon
        /// </summary>
        public DungeonPrefabs parts;

        /// <summary>
        ///     Prefabs used for players
        /// </summary>
        public PlayerDriver[] playerPrefabs;

        /// <summary>
        ///     Prefabs used for enemies
        /// </summary>
        public EnemyDriver[] enemyPrefabs;

        /// <summary>
        ///     Prefabs used for bosses
        /// </summary>
        public EnemyDriver[] bossPrefabs;

        /// <summary>
        ///     Viable design for the dungeon
        /// </summary>
        public DungeonDesign[] designs;

        /// <summary>
        ///     The design of the dungeon
        /// </summary>
        private DungeonDesign design;

        /// <summary> Tag used for anything to be deleted on the next floor </summary>
        private const string DeleteOnNextFloorTag = "DeleteOnNextFloor";

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="DungeonGenerator"/>
        /// </summary>
        private void Start()
        {
            this.GenerateFloor();
        }

        /// <summary>
        ///     Generates a floor.
        /// </summary>
        private void GenerateFloor()
        {
            this.ResetGenerator();
            this.DeleteLastFloor();
            this.CreateParents();

            this.DefineLayout();
            this.SpawnRooms();
            this.ApplyDesign();

            this.SpawnPlayer();
            this.SpawnRecruits();
            this.SpawnEnemies();
            this.SpawnBoss();

            this.InitializeStatic();

            MainManager.CameraController.UpdateAndRestartRangeActivityCheck();
        }

        /// <summary>
        ///     Resets values required to regenerate the values.
        /// </summary>
        private void ResetGenerator()
        {
            this.enemyLevelBase = -1;
            this.totalFloorSize = -1;
        }

        /// <summary>
        ///     Deletes the last floor
        /// </summary>
        private void DeleteLastFloor()
        {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(DungeonGenerator.DeleteOnNextFloorTag))
            {
                MonoBehaviour.Destroy(gameObject);
            }
        }

        /// <summary>
        ///     Creates the parent game objects
        /// </summary>
        private void CreateParents()
        {
            this.entityParent = new GameObject("EntityParent").transform;
            this.roomParent = new GameObject("DungeonParent").transform;

            this.entityParent.tag = DungeonGenerator.DeleteOnNextFloorTag;
            this.roomParent.tag = DungeonGenerator.DeleteOnNextFloorTag;

            this.entityParent.position = Vector3.zero;
            this.roomParent.position = Vector3.zero;
        }

        /// <summary>
        ///     Adds a behaviour to be only active in a limited range
        /// </summary>
        /// <param name="behaviour">The behaviour</param>
        private void AddLimitedRange(Behaviour behaviour)
        {
            behaviour.enabled = false;
            MainManager.CameraController.LimitedRangeBehaviours.Add(behaviour);
        }

        /// <summary>
        ///     Adds a GameObject to be only active in a limited range
        /// </summary>
        /// <param name="gameObject">The GameObject</param>
        private void AddLimitedRange(GameObject gameObject)
        {
            gameObject.SetActive(false);
            MainManager.CameraController.LimitedRangeObjects.Add(gameObject);
        }

        /// <summary>
        ///     Adds behaviours to be only active in a limited range
        /// </summary>
        /// <param name="behaviours">The behaviours</param>
        private void AddLimitedRange(Behaviour[] behaviours)
        {
            foreach (Behaviour behaviour in behaviours)
            {
                this.AddLimitedRange(behaviour);
            }
        }

        /// <summary>
        ///     Adds GameObjects to be only active in a limited range
        /// </summary>
        /// <param name="gameObjects">The GameObjects</param>
        private void AddLimitedRange(GameObject[] gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                this.AddLimitedRange(gameObject);
            }
        }
    }
}