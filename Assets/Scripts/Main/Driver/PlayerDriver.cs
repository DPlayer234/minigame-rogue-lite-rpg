namespace DPlay.RoguePG.Main.Driver
{
    using System.Collections;
    using System.Collections.Generic;
    using DPlay.RoguePG.Main.Sprite3D;
    using DPlay.RoguePG.Main.BattleDriver;
    using UnityEngine;

    /// <summary>
    ///     Makes Players work.
    /// </summary>
    [RequireComponent(typeof(PlayerBattleDriver))]
    [DisallowMultipleComponent]
    public class PlayerDriver : BaseDriver
    {
        /// <summary>
        ///     The current player party
        /// </summary>
        public static Party<PlayerDriver> Party { get; private set; }

        /// <summary>
        ///     The recruitable component attached to this GameObject
        /// </summary>
        private RecruitablePlayer recruitableComponent;

        /// <summary>
        ///     Whether this player driver is recruitable (and therefore not in the party)
        /// </summary>
        public bool IsRecruitable
        {
            get
            {
                return !PlayerDriver.Party.Contains(this);
            }
        }

        /// <summary>
        ///     The <seealso cref="PlayerDriver"/> leading the group.
        /// </summary>
        public override BaseDriver Leader
        {
            get
            {
                return !this.IsRecruitable ? PlayerDriver.Party.GetLeader() : null;
            }
        }

        /// <summary>
        ///     The <seealso cref="PlayerDriver"/> this one is directly following.
        /// </summary>
        public override BaseDriver Following
        {
            get
            {
                int myIndex = PlayerDriver.Party.IndexOf(this);
                return (!this.IsRecruitable && myIndex > 0) ? PlayerDriver.Party[myIndex - 1] : null;
            }
        }

        /// <summary> Returns whether this <see cref="PlayerDriver"/> is the leader </summary>
        public override bool IsLeader { get { return this.Leader == this; } }

        /// <summary>
        ///     Assigns an new <seealso cref="Party{T}"/> to <seealso cref="Party"/>
        /// </summary>
        public static void CreateNewParty()
        {
            PlayerDriver.Party = new Party<PlayerDriver>();

#if UNITY_EDITOR
            // Set this variable for debugging
            MainManager.Instance.playerPartyDebug = (List<PlayerDriver>)PlayerDriver.Party;
#endif
        }

        /// <summary>
        ///     Makes this player leave the party
        /// </summary>
        public void LeaveParty()
        {
            PlayerDriver.Party.Remove(this);
            this.transform.parent = Dungeon.DungeonGenerator.EntityParent;
        }

        /// <summary>
        ///     Called on game over.
        /// </summary>
        public void OnGameOver()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        ///     Calculates and returns the top-down movement vector.
        ///     The axes are mapped X: X, Y: Z.
        /// </summary>
        /// <returns>Target Top-Down Movement Vector</returns>
        protected override Vector2 GetLeaderMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 rawMovement =
                MainManager.CameraController.transform.forward * vertical +
                MainManager.CameraController.transform.right * horizontal;
            return new Vector2(rawMovement.x, rawMovement.z).normalized;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerDriver"/> whether it is or is not active.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="PlayerDriver"/> when it first becomes active
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="BaseDriver"/>
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.IsLeader && MainManager.CameraController.following != this.spriteManager.rootTransform)
            {
                MainManager.CameraController.following = this.spriteManager.rootTransform;
            }

            // Make sure the recruitable component is attached while needed
            if (this.IsRecruitable)
            {
                this.FindRecruitableComponent();
            }
            else if (this.recruitableComponent != null)
            {
                MonoBehaviour.Destroy(this.recruitableComponent);
            }
        }

        /// <summary>
        ///     Finds or attaches a <seealso cref="RecruitablePlayer"/> to the GameObject and assigns it to <seealso cref="recruitableComponent"/>
        /// </summary>
        private void FindRecruitableComponent()
        {
            if (this.recruitableComponent == null)
            {
                this.recruitableComponent = this.GetComponent<RecruitablePlayer>();

                if (this.recruitableComponent == null)
                {
                    this.recruitableComponent = this.gameObject.AddComponent<RecruitablePlayer>();
                }
            }
        }
    }
}
