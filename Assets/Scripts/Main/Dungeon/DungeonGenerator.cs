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
    public class DungeonGenerator : MonoBehaviour
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

        /// <summary>
        ///     Prefabs used for walls
        /// </summary>
        public GameObject[] wallPrefabs;

        /// <summary>
        ///     Prefabs used for the starting rooms (<seealso cref="RoomType.Start"/>)
        /// </summary>
        public GameObject[] startingRoomPrefabs;

        /// <summary>
        ///     Prefabs used for common rooms (<seealso cref="RoomType.Common"/>)
        /// </summary>
        public GameObject[] commonRoomPrefabs;

        /// <summary>
        ///     Prefabs used for boss rooms (<seealso cref="RoomType.Boss"/>)
        /// </summary>
        public GameObject[] bossRoomPrefabs;

        /// <summary>
        ///     Prefabs used for treasure rooms (<seealso cref="RoomType.Treasure"/>)
        /// </summary>
        public GameObject[] treasureRoomPrefabs;

        /// <summary>
        ///     Prefabs used for players
        /// </summary>
        public PlayerDriver[] playerPrefabs;

        /// <summary>
        ///     Prefabs used for enemies
        /// </summary>
        public EnemyDriver[] enemyPrefabs;

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

            this.entityParent = new GameObject("EntityParent").transform;
            this.dungeonParent = new GameObject("DungeonParent").transform;

            this.entityParent.position = Vector3.zero;
            this.dungeonParent.position = Vector3.zero;

            this.DefineLayout();
            this.SpawnRooms();
            this.SpawnPlayer();
            this.SpawnEnemies();

            this.Remove();
        }

        /// <summary>
        ///     Defines the floor layout
        /// </summary>
        private void DefineLayout()
        {
            this.floorLayout = new Dictionary<Vector2Int, RoomType>();

            Vector2Int coordinate = new Vector2Int(0, 0);
            this.floorLayout[coordinate] = RoomType.Start;

            // Common Rooms
            for (int i = 0; i < this.TotalFloorSize; i++)
            {
                // Move room coordinate by 1 in any direction
                coordinate += DungeonGenerator.roomOffsets.GetRandomItem();

                // Skip iteration and repeat if the room was already set.
                if (this.floorLayout.ContainsKey(coordinate))
                {
                    --i;
                    continue;
                }

                this.floorLayout[coordinate] = RoomType.Common;
            }

            this.DefineSpecialRoomLayout();
        }

        /// <summary>
        ///     Defines the position of special rooms
        /// </summary>
        private void DefineSpecialRoomLayout()
        {
            // Gets any valid locations for 'special' room types (aka, empty and exactly one connection to other rooms)
            var validSpecialLocations = new List<Vector2Int>();

            foreach (KeyValuePair<Vector2Int, RoomType> item in this.floorLayout)
            {
                Vector2Int position = item.Key;

                foreach (Vector2Int offset in DungeonGenerator.roomOffsets)
                {
                    Vector2Int checkPosition = position + offset;

                    if (!validSpecialLocations.Contains(checkPosition) && this.IsValidSpecialRoomLocation(checkPosition))
                    {
                        validSpecialLocations.Add(checkPosition);
                    }
                }
            }

            this.AddSpecialRoomToLayout(validSpecialLocations, RoomType.Boss);
        }

        /// <summary>
        ///     Adds a given special room to the layout
        /// </summary>
        /// <param name="validSpecialLocations">A list of valid locations for special rooms</param>
        /// <param name="roomType">The room type</param>
        private void AddSpecialRoomToLayout(List<Vector2Int> validSpecialLocations, RoomType roomType)
        {
            Vector2Int roomPosition = validSpecialLocations.GetRandomItem();
            validSpecialLocations.Remove(roomPosition);

            this.floorLayout[roomPosition] = roomType;
        }

        /// <summary>
        ///     Returns whether the given position is valid for a special room
        /// </summary>
        /// <param name="position">The position on the grid</param>
        /// <returns>Whether the given position is valid for a special room</returns>
        private bool IsValidSpecialRoomLocation(Vector2Int position)
        {
            int adjacent = 0;

            foreach (Vector2Int offset in DungeonGenerator.roomOffsets)
            {
                adjacent += this.floorLayout.ContainsKey(position + offset) ? 1 : 0;
            }

            return adjacent == 1;
        }

        /// <summary>
        ///     Spawns rooms where appropriate
        /// </summary>
        private void SpawnRooms()
        {
            // This is going to make assigning rooms easier.
            Dictionary<RoomType, GameObject[]> typeToPrefabs = new Dictionary<RoomType, GameObject[]>(4);
            typeToPrefabs[RoomType.Start] = this.startingRoomPrefabs;
            typeToPrefabs[RoomType.Common] = this.commonRoomPrefabs;
            typeToPrefabs[RoomType.Boss] = this.bossRoomPrefabs;
            typeToPrefabs[RoomType.Treasure] = this.treasureRoomPrefabs;

            foreach (KeyValuePair<Vector2Int, RoomType> item in this.floorLayout)
            {
                GameObject newRoom = Instantiate(typeToPrefabs[item.Value].GetRandomItem(), this.dungeonParent);

                newRoom.transform.position = new Vector3(
                    item.Key.x * this.roomSize.x,
                    0.0f,
                    item.Key.y * this.roomSize.y);

                this.limitedRangeBehaviours.AddRange(newRoom.GetComponentsInChildren<Light>());
            }
        }

        /// <summary>
        ///     Spawns the player.
        ///     There should always only be one player spawn point per floor.
        /// </summary>
        private void SpawnPlayer()
        {
            GameObject playerSpawnPoint = GameObject.FindGameObjectWithTag(PlayerSpawnPointTag);

            GameObject[] players = GameObject.FindGameObjectsWithTag(BattleManager.PlayerEntityTag);

            if (players.Length < 1)
            {
                players = new GameObject[]
                {
                    MainManager.SpawnEntityWithBonus(
                        Storage.SelectedPlayerPrefab,
                        Storage.BonusStat1,
                        Storage.BonusStat2).gameObject
                };
            }

            foreach (GameObject player in players)
            {
                player.transform.position = playerSpawnPoint.transform.position;
            }

            MonoBehaviour.Destroy(playerSpawnPoint);
        }

        /// <summary>
        ///     Spawns enemies at all EnemySpawnPoints and deletes those
        /// </summary>
        private void SpawnEnemies()
        {
            GameObject[] enemySpawnPoints = GameObject.FindGameObjectsWithTag(EnemySpawnPointTag);

            int enemyLeaderIndex = 0;
            foreach (GameObject enemySpawnPoint in enemySpawnPoints)
            {
                int enemyCount = Random.Range(this.enemyCount.x, this.enemyCount.y + 1);

                if (enemyCount > 0)
                {
                    EnemyDriver leaderEnemy = null, followEnemy = null;

                    for (int index = 0; index < enemyCount; index++)
                    {
                        EnemyDriver enemy = this.SpawnEnemy(
                            enemySpawnPoint.transform.position,
                            ref leaderEnemy,
                            ref followEnemy);

                        enemy.name = string.Format("Enemy #{0}-{1}", enemyLeaderIndex, index);
                    }
                }

                MonoBehaviour.DestroyImmediate(enemySpawnPoint);
                ++enemyLeaderIndex;
            }
        }

        /// <summary>
        ///     Spawns and returns an enemy
        /// </summary>
        /// <param name="position">The position to spawn at</param>
        /// <param name="leaderEnemy">The leader</param>
        /// <param name="followEnemy">The enemy to follow</param>
        /// <returns>The newly spawned enemy</returns>
        private EnemyDriver SpawnEnemy(Vector3 position, ref EnemyDriver leaderEnemy, ref EnemyDriver followEnemy)
        {
            EnemyDriver newEnemy = Instantiate(this.enemyPrefabs.GetRandomItem(), this.entityParent);
            newEnemy.transform.position = position;

            newEnemy.battleDriver.Level = this.EnemyLevel;

            newEnemy.leader = leaderEnemy;
            newEnemy.following = followEnemy;

            if (leaderEnemy == null) leaderEnemy = newEnemy;
            followEnemy = newEnemy;

            this.limitedRangeObjects.Add(newEnemy.gameObject);

            return newEnemy;
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