namespace SAE.RoguePG.Main.Dungeon
{
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     The script used by a floor transition
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class FloorTransition : MonoBehaviour
    {
        /// <summary>
        ///     Does its thing and proceeds to the next floor if the given GameObject is a leading player
        /// </summary>
        private void ProceedIfPlayer(GameObject gameObject)
        {
            PlayerDriver playerDriver = gameObject.GetComponent<PlayerDriver>();

            if (playerDriver != null && playerDriver.IsLeader)
            {
                DungeonGenerator.GoToNextFloor();
            }
        }

        /// <summary>
        ///     Called by Unity when a trigger enters.
        /// </summary>
        /// <param name="other">The other collider</param>
        private void OnTriggerEnter(Collider other)
        {
            this.ProceedIfPlayer(other.gameObject);
        }

        /// <summary>
        ///     Called by Unity when a collision occurs.
        /// </summary>
        /// <param name="collision">The collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            this.ProceedIfPlayer(collision.gameObject);
        }
    }
}