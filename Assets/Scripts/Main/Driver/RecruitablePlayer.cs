namespace SAE.RoguePG.Main.Driver
{
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Sprite3D;
    using SAE.RoguePG.Main.UI;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Allows this player to be recruitable.
    /// </summary>
    [RequireComponent(typeof(PlayerDriver))]
    public class RecruitablePlayer : MonoBehaviour
    {
        /// <summary>
        ///     Base Height for buttons
        /// </summary>
        public const float ButtonHeight = 0.45f;

        /// <summary>
        ///     Base Height for 3D text
        /// </summary>
        public const float TextHeight = 0.9f;

        /// <summary> The minimum distance between a player and this entity to allow recruitment </summary>
        private const float RecruitmentRange = 1.5f;

        /// <summary> The parent object for recruiting things </summary>
        private GameObject recruitUI;

        /// <summary> The <seealso cref="PlayerDriver"/> also attached to this GameObject </summary>
        private PlayerDriver playerDriver;

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="RecruitablePlayer"/> whether it is active or not
        /// </summary>
        private void Awake()
        {
            playerDriver = this.GetComponent<PlayerDriver>();
        }

        /// <summary>
        ///     Gets whether this entity is in recruitment range
        /// </summary>
        private bool IsInRecruitmentRange
        {
            get
            {
                PlayerDriver partyLeader = PlayerDriver.Party.GetLeader();

                return (this.transform.position - partyLeader.transform.position).sqrMagnitude < RecruitablePlayer.RecruitmentRange * RecruitablePlayer.RecruitmentRange;
            }
        }

        /// <summary>
        ///     Creates a new recruitment UI.
        /// </summary>
        private void CreateRecruitmentUI()
        {
            this.DestroyRecruitmentUI();

            this.recruitUI = MonoBehaviour.Instantiate(GenericPrefab.Panel, MainManager.WorldCanvas.transform);

            ButtonController recruitButton = MonoBehaviour.Instantiate(GenericPrefab.WorldButton, this.recruitUI.transform);

            recruitButton.SetText("Recruit");

            recruitButton.SetupButtonController(this.transform, RecruitablePlayer.ButtonHeight);

            recruitButton.OnClick.AddListener(this.Recruit);
        }

        /// <summary>
        ///     Destroys the recruitment UI
        /// </summary>
        private void DestroyRecruitmentUI()
        {
            if (this.recruitUI != null)
            {
                MonoBehaviour.Destroy(recruitUI);
            }
        }

        /// <summary>
        ///     Recruits this entity to the player party
        /// </summary>
        private void Recruit()
        {
            if (PlayerDriver.Party.CapacityFilled)
            {
                Debug.Log("Cannot recruit as the party is already full.");

                Text3DController floatingText = MonoBehaviour.Instantiate(GenericPrefab.Text3D);
                floatingText.Text = "Cannot recruit.\nYour Party is full!";
                floatingText.transform.position = this.transform.position + new Vector3(0.0f, TextHeight, 0.0f);
                return;
            }

            PlayerDriver.Party.Add(this.playerDriver);
            this.transform.parent = null;
        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="RecruitablePlayer"/>
        /// </summary>
        private void FixedUpdate()
        {
            if (this.recruitUI == null && this.IsInRecruitmentRange)
            {
                this.CreateRecruitmentUI();
            }
            else if (this.recruitUI != null && !this.IsInRecruitmentRange)
            {
                this.DestroyRecruitmentUI();
            }
        }

        /// <summary>
        ///     Called by Unity when this behaviour is disabled.
        /// </summary>
        private void OnDisable()
        {
            this.DestroyRecruitmentUI();
        }

        /// <summary>
        ///     Called by Unity when this behaviour is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            this.DestroyRecruitmentUI();
        }
    }
}