namespace SAE.RoguePG.Main
{
    using SAE.RoguePG.Main.BattleDriver;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Generates a floor when attached; then deletes itself.
    /// </summary>
    public class RoomGenerator : MonoBehaviour
    {
        private const string RoomConnectorTag = "RoomConnector";

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
        ///     Prefabs used for enemies
        /// </summary>
        public GameObject[] enemyPrefabs;

        /// <summary>
        ///     Floor Number (First Floor is 1, second is 2 ....)
        /// </summary>
        public int floorNumber = 1;

        /// <summary>
        ///     How many rooms were already generated?
        /// </summary>
        private int generatedRooms;

        /// <summary>
        ///     How many rooms does this floor have?
        /// </summary>
        private int TotalFloorSize
        {
            get
            {
                return Mathf.CeilToInt(floorNumber * 2 + 4 + Random.Range(-1, 1));
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="RoomGenerator"/>
        /// </summary>
        private void Start()
        {
            Instantiate(this.startPrefab).transform.position = Vector3.zero;

            while (this.generatedRooms < this.TotalFloorSize)
            {
                this.SpawnNextRooms();
            }

            Destroy(this);
        }

        private void SpawnNextRooms()
        {
            GameObject[] roomConnections = GameObject.FindGameObjectsWithTag(RoomConnectorTag);

            foreach (GameObject roomConnection in roomConnections)
            {
                ++this.generatedRooms;

                bool tooLarge = this.generatedRooms > this.TotalFloorSize;

                GameObject nextPiece =
                    tooLarge ?
                    Instantiate(wallPrefab) :
                    Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)]);

                nextPiece.transform.position = roomConnection.transform.position;
                nextPiece.transform.rotation = roomConnection.transform.rotation;

                Destroy(roomConnection);
                if (tooLarge) break;
            }

            this.SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            GameObject[] enemySpawnPoints = GameObject.FindGameObjectsWithTag(EnemySpawnPointTag);

            foreach (GameObject enemySpawnPoint in enemySpawnPoints)
            {
                Destroy(enemySpawnPoint);
            }
        }
    }
}