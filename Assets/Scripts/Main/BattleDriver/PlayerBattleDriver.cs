using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RoguePG.Main.BattleDriver
{
    /// <summary>
    ///     Makes battles work. Do not add from editor!
    /// </summary>
    [RequireComponent(typeof(EntityBattleDriver))]
    [DisallowMultipleComponent]
    public class PlayerBattleDriver : MonoBehaviour
    {
        /// <summary> The <seealso cref="EntityBattleDriver"/> also attached to this <seealso cref="GameObject"/> </summary>
        private EntityBattleDriver battleDriver;

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerBattleDriver"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.battleDriver = this.GetComponent<EntityBattleDriver>();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerBattleDriver"/> when it first becomes active
        /// </summary>
        private void Start()
        {

        }
    }
}
