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
        ///     The average amount of enemies per room.
        ///     Relevant for enemy level calculations
        /// </summary>
        public const float AverageEnemyCountPerRoom = 1.4f;

        /// <summary> Tag used by Player Spawn Points </summary>
        private const string PlayerSpawnPointTag = "PlayerSpawnPoint";

        /// <summary> Tag used by Recruit Spawn Points </summary>
        private const string RecruitSpawnPointTag = "RecruitSpawnPoint";

        /// <summary> Tag used by Enemy Spawn Points </summary>
        private const string EnemySpawnPointTag = "EnemySpawnPoint";

        /// <summary> Tag used by Boss Spawn Points </summary>
        private const string BossSpawnPointTag = "BossSpawnPoint";

        /// <summary> The multiplier for the level of bosses compared to regular enemies </summary>
        private const float BossLevelMultiplier = 1.15f;

        /// <summary> Parent transform for the entities. </summary>
        private Transform entityParent;

        /// <summary>
        ///     What level are enemies supposed to be?
        /// </summary>
        private int enemyLevelBase = -1;

        /// <summary>
        ///     What level are enemies supposed to be?
        ///     Slight variation.
        /// </summary>
        private int EntityLevel
        {
            get
            {
                if (this.enemyLevelBase < 0)
                {
                    this.enemyLevelBase = Mathf.RoundToInt(
                        VariousCommon.SumFuncRange(
                            DungeonGenerator.GetTotalFloorSize,
                            1,
                            this.floorNumber - 1) * DungeonGenerator.AverageEnemyCountPerRoom);
                }

                return Mathf.Max(1, this.enemyLevelBase + Random.Range(-1, 2));
            }
        }

        /// <summary>
        ///     Spawns the player.
        ///     There should always only be one player spawn point per floor.
        /// </summary>
        private void SpawnPlayer()
        {
            // Get all, so useful errors and warning can be given
            GameObject[] playerSpawnPoints = GameObject.FindGameObjectsWithTag(DungeonGenerator.PlayerSpawnPointTag);

            if (playerSpawnPoints.Length == 0)
            {
                throw new RPGException(RPGException.Cause.DungeonNoPlayerSpawnPoint);
            }
            else if (playerSpawnPoints.Length > 1)
            {
                Debug.LogWarning("There is more than one player spawn point.");
            }

            // Pick the first one
            GameObject playerSpawnPoint = playerSpawnPoints[0];
            
            // Make sure there's a party
            if (PlayerDriver.Party == null || PlayerDriver.Party.Count == 0)
            {
                PlayerDriver.CreateNewParty();

                PlayerDriver player =
                    MainManager.SpawnEntityWithBonus(
                        Storage.SelectedPlayerPrefab,
                        Storage.BonusStat1,
                        Storage.BonusStat2);

                PlayerDriver.Party.Add(player);
            }

            // Move the players
            foreach (PlayerDriver player in PlayerDriver.Party)
            {
                player.transform.position = playerSpawnPoint.transform.position + new Vector3(
                    Random.value * 2.0f - 1.0f,
                    0.0f,
                    Random.value * 2.0f - 1.0f);
            }

            // Move the camera
            MainManager.CameraController.transform.position = playerSpawnPoint.transform.position;

            MonoBehaviour.Destroy(playerSpawnPoint);
        }

        /// <summary>
        ///     Spawns recruits at RecruitSpawnPoints
        /// </summary>
        public void SpawnRecruits()
        {
            GameObject[] recruitSpawnPoints = GameObject.FindGameObjectsWithTag(DungeonGenerator.RecruitSpawnPointTag);

            int recruitIndex = 0;
            foreach (GameObject recruitSpawnPoint in recruitSpawnPoints)
            {
                PlayerDriver recruit = MainManager.SpawnEntityWithBonus(
                    this.playerPrefabs.GetRandomItem(),
                    Stat.Random,
                    Stat.Random);

                recruit.transform.parent = this.entityParent;
                recruit.transform.position = recruitSpawnPoint.transform.position;

                recruit.battleDriver.Level = this.EntityLevel;

                recruit.name = string.Format("Recruit #{0}", recruitIndex);

                MonoBehaviour.Destroy(recruitSpawnPoint);
                ++recruitIndex;
            }
        }

        /// <summary>
        ///     Spawns enemies at all EnemySpawnPoints and deletes those
        /// </summary>
        private void SpawnEnemies()
        {
            GameObject[] enemySpawnPoints = GameObject.FindGameObjectsWithTag(DungeonGenerator.EnemySpawnPointTag);

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

                MonoBehaviour.Destroy(enemySpawnPoint);
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
            EnemyDriver newEnemy = MonoBehaviour.Instantiate(this.enemyPrefabs.GetRandomItem(), this.entityParent);
            newEnemy.transform.position = position;

            newEnemy.battleDriver.Level = this.EntityLevel;
            
            newEnemy.SetLeaderAndFollowing(leaderEnemy, followEnemy);

            if (leaderEnemy == null) leaderEnemy = newEnemy;
            followEnemy = newEnemy;

            this.limitedRangeObjects.Add(newEnemy.gameObject);

            return newEnemy;
        }

        /// <summary>
        ///     Spawns a boss at all a Boss spawn point
        /// </summary>
        private void SpawnBoss()
        {
            GameObject bossSpawnPoint = GameObject.FindGameObjectWithTag(DungeonGenerator.BossSpawnPointTag);

            EnemyDriver boss = MonoBehaviour.Instantiate(this.bossPrefabs.GetRandomItem(), this.entityParent);
            boss.transform.position = bossSpawnPoint.transform.position;

            boss.battleDriver.Level = Mathf.FloorToInt(this.EntityLevel * DungeonGenerator.BossLevelMultiplier);

            this.limitedRangeObjects.Add(boss.gameObject);

            MonoBehaviour.Destroy(bossSpawnPoint);
        }
    }
}