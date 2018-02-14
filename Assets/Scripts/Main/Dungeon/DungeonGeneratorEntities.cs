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
        /// <summary> Tag used by Player Spawn Points </summary>
        private const string PlayerSpawnPointTag = "PlayerSpawnPoint";

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
            EnemyDriver newEnemy = MonoBehaviour.Instantiate(this.enemyPrefabs.GetRandomItem(), this.entityParent);
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
        ///     Spawns a boss at all a Boss spawn point
        /// </summary>
        private void SpawnBoss()
        {
            GameObject bossSpawnPoint = GameObject.FindGameObjectWithTag(DungeonGenerator.BossSpawnPointTag);

            EnemyDriver boss = MonoBehaviour.Instantiate(this.bossPrefabs.GetRandomItem(), this.entityParent);
            boss.transform.position = bossSpawnPoint.transform.position;

            boss.battleDriver.Level = Mathf.FloorToInt(this.EnemyLevel * DungeonGenerator.BossLevelMultiplier);

            this.limitedRangeObjects.Add(boss.gameObject);

            MonoBehaviour.Destroy(bossSpawnPoint);
        }
    }
}