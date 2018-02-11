namespace SAE.RoguePG
{
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     Sets the default values for the <seealso cref="Storage"/> if they haven't been set yet.
    ///     Then destroys itself.
    /// </summary>
    public class DefaultStorageSetter : MonoBehaviour
    {
        /// <summary>
        ///     Default prefab to be used for the player
        /// </summary>
        public PlayerDriver defaultPlayerPrefab;

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="DefaultStorageSetter"/> whether it is active or not
        /// </summary>
        private void Awake()
        {
            if (Storage.SelectedPlayerPrefab == null) Storage.SelectedPlayerPrefab = this.defaultPlayerPrefab;

            MonoBehaviour.Destroy(this);
        }
    }
}
