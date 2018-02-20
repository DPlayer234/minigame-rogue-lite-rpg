namespace DPlay.RoguePG.Main.Dungeon
{
    using System.Collections;
    using System.Collections.Generic;
    using DPlay.RoguePG.Main.BattleDriver;
    using DPlay.RoguePG.Main.Camera;
    using DPlay.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Generates a floor when attached to a GameObject.
    ///     Rooms are generated on a grid.
    /// </summary>
    public partial class DungeonGenerator : Singleton<DungeonGenerator>
    {
        /// <summary>
        ///     Floor Number
        /// </summary>
        [SerializeField]
        private int floorNumber = 1;

        /// <summary>
        ///     Minimum and maximum enemy count per spawn point.
        /// </summary>
        [SerializeField]
        private Vector2Int enemyCount;

        /// <summary>
        ///     Minimum and maximum recruit count per spawn point.
        /// </summary>
        [SerializeField]
        private Vector2Int recruitCount;

        /// <summary>
        ///     How large the rooms are (width and depth)
        /// </summary>
        [SerializeField]
        private Vector2 roomSize;

        /// <summary>
        ///     Parts used in the dungeon
        /// </summary>
        [SerializeField]
        private DungeonPrefabs parts;

        /// <summary>
        ///     Prefabs used for players
        /// </summary>
        [SerializeField]
        private PlayerDriver[] playerPrefabs;

        /// <summary>
        ///     Prefabs used for enemies
        /// </summary>
        [SerializeField]
        private EnemyDriver[] enemyPrefabs;

        /// <summary>
        ///     Prefabs used for bosses
        /// </summary>
        [SerializeField]
        private EnemyDriver[] bossPrefabs;

        /// <summary>
        ///     Viable design for the dungeon
        /// </summary>
        [SerializeField]
        private DungeonDesign[] designs;

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

            ActivityHandler.UpdateAndRestart();
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
    }
}