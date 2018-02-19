namespace SAE.RoguePG.Main.UI
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Generates and updates the Mini Map.
    ///     Adding a new MiniMap will delete the old one (including GameObject).
    /// </summary>
    public class MiniMap : MonoBehaviour
    {
        /// <summary>
        ///     The last instance of the mini map
        /// </summary>
        private static MiniMap LastInstance { get; set; }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="MiniMap"/> whether it is active or not
        /// </summary>
        private void Awake()
        {
            if (MiniMap.LastInstance != null)
            {
                // Remove last instance
                MonoBehaviour.Destroy(MiniMap.LastInstance.gameObject);
            }

            MiniMap.LastInstance = this;
        }

        /// <summary>
        ///     Updates the MiniMap based on the layout
        /// </summary>
        /// <param name="dungeonLayout">The current layout of the dungeon</param>
        private void UpdateMap(Dictionary<Vector2Int, Dungeon.RoomType> dungeonLayout)
        {
            ;
        }
    }
}