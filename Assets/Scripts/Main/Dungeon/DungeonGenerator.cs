namespace SAE.RoguePG.Main.Dungeon
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Generates a floor when attached; then deletes itself.
    ///     Rooms are generated on a grid.
    /// </summary>
    public partial class DungeonGenerator : MonoBehaviour
    {
        /// <summary>
        ///     The average amount of enemies per room.
        ///     Relevant for enemy level calculations
        /// </summary>
        public const int AverageEnemyCountPerRoom = 2;

        /// <summary>
        ///     Whether or not to also remove the GameObject
        /// </summary>
        public bool removeGameObject = true;

        /// <summary>
        ///     How large the rooms are (width and depth)
        /// </summary>
        public Vector2 roomSize;

        /// <summary> Parts used in the dungeon </summary>
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
        ///     Minimum and maximum enemy count per spawn point.
        /// </summary>
        public Vector2Int enemyCount;

        /// <summary>
        ///     Floor Number (First Floor is 1, second is 2 etc.)
        /// </summary>
        public int floorNumber = 1;

        /// <summary> Tag used by Player Spawn Points </summary>
        private const string PlayerSpawnPointTag = "PlayerSpawnPoint";

        /// <summary> Tag used by Enemy Spawn Points </summary>
        private const string EnemySpawnPointTag = "EnemySpawnPoint";

        /// <summary> Tag used by Boss Spawn Points </summary>
        private const string BossSpawnPointTag = "BossSpawnPoint";

        /// <summary> Tag used for anything to be deleted on the next floor </summary>
        private const string DeleteOnNextFloorTag = "DeleteOnNextFloor";

        /// <summary> The multiplier for the level of bosses compared to regular enemies </summary>
        private const float BossLevelMultiplier = 1.15f;

        /// <summary>
        ///     Lists possible offsets from one room to the next
        /// </summary>
        private static List<Vector2Int> roomOffsets = new List<Vector2Int>()
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        /// <summary>
        ///     How many rooms were already generated?
        /// </summary>
        private int generatedRooms;

        /// <summary>
        ///     How many rooms does this floor have?
        /// </summary>
        private int totalFloorSize = -1;

        /// <summary>
        ///     How the floor is layed out.
        /// </summary>
        private Dictionary<Vector2Int, RoomType> floorLayout;

        /// <summary> Parent transform for the entities. </summary>
        private Transform entityParent;

        /// <summary> Parent transform for the dungeon rooms and co. </summary>
        private Transform dungeonParent;

        /// <summary> A referrence copy of <seealso cref="CameraController.LimitedRangeObjects"/> </summary>
        private List<GameObject> limitedRangeObjects;

        /// <summary> A referrence copy of <seealso cref="CameraController.LimitedRangeBehaviours"/> </summary>
        private List<Behaviour> limitedRangeBehaviours;

        /// <summary>
        ///     Defines common room types
        /// </summary>
        private enum RoomType
        {
            /// <summary> There is no room </summary>
            None = -1,

            /// <summary> The starting room </summary>
            Start,

            /// <summary> A room without any special features </summary>
            Common,
            
            /// <summary> The boss room, containing the boss and the path to the next floor </summary>
            Boss,

            /// <summary> A room with 'treasure' of some kind </summary>
            Treasure
        }

        /// <summary>
        ///     How many rooms does this floor have?
        ///     Not necessarily equal to <seealso cref="DungeonGenerator.GetTotalFloorSize(int)"/>.
        /// </summary>
        private int TotalFloorSize
        {
            get
            {
                // Generate value on first demand
                return this.totalFloorSize < 0 ?
                    this.totalFloorSize = DungeonGenerator.GetTotalFloorSize(this.floorNumber) + Random.Range(-1, 1) :
                    this.totalFloorSize;
            }
        }

        /// <summary>
        ///     What level are enemies supposed to be?
        ///     Slightly variation.
        /// </summary>
        private int EnemyLevel
        {
            get
            {
                int enemyLevel = Mathf.RoundToInt(
                    VariousCommon.SumFuncRange(
                        DungeonGenerator.GetTotalFloorSize,
                        1,
                        this.floorNumber - 1) * DungeonGenerator.AverageEnemyCountPerRoom + Random.Range(-1, 2));

                return Mathf.Max(1, enemyLevel);
            }
        }

        /// <summary>
        ///     Gets the size of a floor in <paramref name="floorNumber"/> without variation
        /// </summary>
        /// <param name="floorNumber">The floor number</param>
        /// <returns>The floor size</returns>
        private static int GetTotalFloorSize(int floorNumber)
        {
            return Mathf.CeilToInt(floorNumber * 2 + 4);
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="DungeonGenerator"/>
        /// </summary>
        private void Start()
        {
            this.limitedRangeObjects = MainManager.CameraController.LimitedRangeObjects;
            this.limitedRangeBehaviours = MainManager.CameraController.LimitedRangeBehaviours;

            this.DeleteLastFloor();
            this.CreateParents();
            this.DefineLayout();
            this.SpawnRooms();
            this.SpawnPlayer();
            this.SpawnEnemies();
            this.SpawnBoss();

            this.Remove();
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
            this.dungeonParent = new GameObject("DungeonParent").transform;

            this.entityParent.tag = DungeonGenerator.DeleteOnNextFloorTag;
            this.dungeonParent.tag = DungeonGenerator.DeleteOnNextFloorTag;

            this.entityParent.position = Vector3.zero;
            this.dungeonParent.position = Vector3.zero;
        }

        /// <summary>
        ///     Removes this thing
        /// </summary>
        private void Remove()
        {
            if (this.removeGameObject)
            {
                MonoBehaviour.Destroy(this.gameObject);
            }
            else
            {
                MonoBehaviour.Destroy(this);
            }
        }
    }
}