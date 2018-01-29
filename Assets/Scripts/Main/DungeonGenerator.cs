namespace SAE.RoguePG.Main
{
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Generates a floor when attached; then deletes itself.
    /// </summary>
    public class DungeonGenerator : MonoBehaviour
    {
        /// <summary> Tag used by RoomConnectors </summary>
        private const string RoomConnectionTag = "RoomConnection";

        /// <summary> Tag used by Enemy Spawn Points </summary>
        private const string EnemySpawnPointTag = "EnemySpawnPoint";

        /// <summary>
        ///     Prefab used for walls
        /// </summary>
        public GameObject wallPrefab;

        /// <summary>
        ///     Prefab used for the starting room
        /// </summary>
        public GameObject startPrefab;

        /// <summary>
        ///     Prefabs used for rooms
        /// </summary>
        public GameObject[] roomPrefabs;

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
            Instantiate(this.startPrefab).transform.position = Vector3.zero;
            Instantiate(this.playerPrefabs.GetRandomItem()).transform.position = Vector3.zero;

            int i = 0;

            while (GameObject.FindGameObjectWithTag(RoomConnectionTag) != null)
            {
                ++i;
                this.SpawnNextRooms();
                if (i > 10)
                {
                    print("CANCELED");
                    return;
                }
            }

            this.SpawnEnemies();

            Destroy(this);
        }

        /// <summary>
        ///     Spawns rooms where appropriate
        /// </summary>
        private void SpawnNextRooms()
        {
            GameObject[] roomConnections = GameObject.FindGameObjectsWithTag(RoomConnectionTag);

            foreach (GameObject roomConnection in roomConnections)
            {
                bool tooLarge = ++this.generatedRooms > this.TotalFloorSize;

                // Instantiate next room (or wall)
                GameObject nextRoom =
                    tooLarge ?
                    Instantiate(this.wallPrefab) :
                    Instantiate(this.roomPrefabs.GetRandomItem());

                // Find connectors to that room
                GameObject connectTo = FindGameObjectsByTagInChildrenOf(nextRoom, RoomConnectionTag).GetRandomItem();

                // Move the room so they connect.
                // This leaves the related trigonometry to be handled by Unity, not my code!
                connectTo.transform.parent = null;
                nextRoom.transform.parent = connectTo.transform;

                connectTo.transform.position = roomConnection.transform.position;
                connectTo.transform.forward = -roomConnection.transform.forward; 

                // Let's not delete the room accidentally though.
                nextRoom.transform.parent = null;

                // Should be used for bounds; leaving it for later
                Collider collider = nextRoom.GetComponent<Collider>();
                if (collider != null)
                {
                    Destroy(collider);
                }

                DestroyImmediate(roomConnection.gameObject);
                DestroyImmediate(connectTo.gameObject);
            }
        }

        /// <summary>
        ///     Spawns enemies at all EnemySpawnPoints and deletes those
        /// </summary>
        private void SpawnEnemies()
        {
            GameObject[] enemySpawnPoints = GameObject.FindGameObjectsWithTag(EnemySpawnPointTag);

            foreach (GameObject enemySpawnPoint in enemySpawnPoints)
            {
                int enemyCount = Random.Range(this.enemyCount.x, this.enemyCount.y + 1);

                if (enemyCount > 0)
                {
                    EnemyDriver leaderEnemy = Instantiate(this.enemyPrefabs.GetRandomItem());
                    leaderEnemy.transform.position = enemySpawnPoint.transform.position;

                    EnemyDriver followEnemy = leaderEnemy;

                    // Effectively starts at 1, ends at enemyCount-1
                    for (int i = 0; i < enemyCount; ++i)
                    {
                        EnemyDriver nextEnemy = Instantiate(this.enemyPrefabs.GetRandomItem());
                        nextEnemy.transform.position = enemySpawnPoint.transform.position;

                        nextEnemy.leader = leaderEnemy;
                        nextEnemy.following = followEnemy;

                        followEnemy = nextEnemy;
                    }
                }

                DestroyImmediate(enemySpawnPoint);
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