namespace SAE.RoguePG.Main.Dungeon
{
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Generates a floor when attached; then deletes itself.
    ///     Rooms are generated on a grid.
    /// </summary>
    public class DungeonGenerator : MonoBehaviour
    {
        /// <summary> Tag used by Player Spawn Points </summary>
        private const string PlayerSpawnPointTag = "PlayerSpawnPoint";

        /// <summary> Tag used by Enemy Spawn Points </summary>
        private const string EnemySpawnPointTag = "EnemySpawnPoint";

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
        ///     Defines common room types
        /// </summary>
        enum RoomType
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
        /// </summary>
        private int TotalFloorSize
        {
            get
            {
                // Generate value on first demand
                return this.totalFloorSize < 0 ?
                    this.totalFloorSize = Mathf.CeilToInt(floorNumber * 2 + 4 + Random.Range(-1, 1)) :
                    this.totalFloorSize;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="RoomGenerator"/>
        /// </summary>
        private void Start()
        {
            this.DefineLayout();

            this.SpawnRooms();

            this.SpawnPlayer();

            this.SpawnEnemies();

            Destroy(this);
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
        /// <param name="position"></param>
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
                Instantiate(typeToPrefabs[item.Value].GetRandomItem()).transform.position = new Vector3(
                    item.Key.x * this.roomSize.x,
                    0.0f,
                    item.Key.y * this.roomSize.y);
            }
        }

        /// <summary>
        ///     Spawns the player.
        ///     There should always only be one player spawn point per floor.
        /// </summary>
        private void SpawnPlayer()
        {
            GameObject playerSpawnPoint = GameObject.FindGameObjectWithTag(PlayerSpawnPointTag);

            Instantiate(this.playerPrefabs.GetRandomItem()).transform.position = playerSpawnPoint.transform.position;

            Destroy(playerSpawnPoint);
        }

        /// <summary>
        ///     Spawns enemies at all EnemySpawnPoints and deletes those
        /// </summary>
        private void SpawnEnemies()
        {
            GameObject[] enemySpawnPoints = GameObject.FindGameObjectsWithTag(EnemySpawnPointTag);

            int enemyLeaderCount = 0;
            foreach (GameObject enemySpawnPoint in enemySpawnPoints)
            {
                int enemyCount = Random.Range(this.enemyCount.x, this.enemyCount.y + 1);

                if (enemyCount > 0)
                {
                    EnemyDriver leaderEnemy = Instantiate(this.enemyPrefabs.GetRandomItem());
                    leaderEnemy.name = string.Format("LeaderEnemy #{0}", enemyLeaderCount);
                    leaderEnemy.transform.position = enemySpawnPoint.transform.position;

                    EnemyDriver followEnemy = leaderEnemy;

                    // Effectively starts at 1, ends at enemyCount-1
                    for (int i = 0; i < enemyCount; ++i)
                    {
                        EnemyDriver nextEnemy = Instantiate(this.enemyPrefabs.GetRandomItem());
                        nextEnemy.name = string.Format("Enemy #{0}-{1}", enemyLeaderCount, i);
                        nextEnemy.transform.position = enemySpawnPoint.transform.position;

                        nextEnemy.leader = leaderEnemy;
                        nextEnemy.following = followEnemy;

                        followEnemy = nextEnemy;
                    }
                }

                DestroyImmediate(enemySpawnPoint);
                ++enemyLeaderCount;
            }
        }

        /// <summary>
        ///     Finds all GameObjects with <paramref name="tag"/> in the children of <paramref name="gameObject"/>
        /// </summary>
        /// <param name="gameObject">The GameObject whose children to search</param>
        /// <param name="tag">The tag they need to match</param>
        /// <returns>A list of GameObjects</returns>
        private static List<GameObject> FindGameObjectsByTagInChildrenOf(GameObject gameObject, string tag)
        {
            List<GameObject> childrenWithTag = new List<GameObject>();
            Transform transform = gameObject.transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;

                if (child.CompareTag(tag))
                {
                    childrenWithTag.Add(child);
                }
                else
                {
                    childrenWithTag.AddRange(FindGameObjectsByTagInChildrenOf(child, tag));
                }
            }

            return childrenWithTag;
        }
    }
}