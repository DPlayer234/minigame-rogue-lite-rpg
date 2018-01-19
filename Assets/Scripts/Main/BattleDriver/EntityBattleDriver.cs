using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RoguePG.Main.BattleDriver
{
    /// <summary>
    ///     Makes battles work. Do not add from editor!
    /// </summary>
    [DisallowMultipleComponent]
    public class EntityBattleDriver : MonoBehaviour
    {
        /// <summary> Whether it's this thing's turn </summary>
        [HideInInspector]
        private bool takingTurn;

        /// <summary> All of its allies, including itself </summary>
        [HideInInspector]
        private GameObject[] allies;

        /// <summary> All of its enemies </summary>
        [HideInInspector]
        private GameObject[] enemies;

        /// <summary> Whether it's this thing's turn </summary>
        public bool TakingTurn
        {
            set
            {
                this.takingTurn = value;
            }

            get
            {
                return this.takingTurn;
            }
        }

        /// <summary> All of its allies, including itself </summary>
        public GameObject[] Allies
        {
            get
            {
                return this.allies;
            }

            set
            {
                this.allies = value;
            }
        }

        /// <summary> All of its enemies </summary>
        public GameObject[] Enemies
        {
            get
            {
                return this.enemies;
            }

            set
            {
                this.enemies = value;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EntityBattleDriver"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            // Reset velocity.
            Rigidbody rigidbody = this.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
            }
        }
    }
}
