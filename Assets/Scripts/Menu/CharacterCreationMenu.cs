namespace SAE.RoguePG.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SAE.RoguePG.Main.Driver;
    using SAE.RoguePG.Main.Sprite3D;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary>
    ///     Any functions used within the character creation menu.
    /// </summary>
    public class CharacterCreationMenu : AnyMenu
    {
        /// <summary> The prefabs used for classes </summary>
        public Main.Driver.PlayerDriver[] characterPrefabs;

        /// <summary> The index of the wizard </summary>
        public int wizardIndex = 0;

        /// <summary> The index of the paladin </summary>
        public int paladinIndex = 1;

        /// <summary> The index of the assassin </summary>
        public int assassinIndex = 2;

        public Text bonusStat1Text;

        public Text bonusStat2Text;

        public Transform playerPreviewPosition;

        private PlayerDriver playerPreview;

        /// <summary>
        ///     Select the wizard class
        /// </summary>
        public void SelectWizard()
        {
            this.SelectClass(this.wizardIndex);
        }

        /// <summary>
        ///     Select the paladin class
        /// </summary>
        public void SelectPaladin()
        {
            this.SelectClass(this.paladinIndex);
        }

        /// <summary>
        ///     Select the assassin class
        /// </summary>
        public void SelectAssassin()
        {
            this.SelectClass(this.assassinIndex);
        }

        /// <summary>
        ///     Randomizes the character bonus stats
        /// </summary>
        public void RandomizeBoni()
        {
            // Set stat boni
            Storage.BonusStat2 = Main.MainGeneral.GetRandomStat(
                Storage.BonusStat1 = Main.MainGeneral.GetRandomStat());
            
            this.SetBonusText(this.bonusStat1Text, Storage.BonusStat1);
            this.SetBonusText(this.bonusStat2Text, Storage.BonusStat2);
        }

        /// <summary>
        ///     Starts the game and loads the main scene.
        /// </summary>
        public void StartGame()
        {
            SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
        }

        /// <summary>
        ///     Select a class based on its index in <seealso cref="characterPrefabs"/>
        /// </summary>
        /// <param name="classIndex">The index of the class</param>
        private void SelectClass(int classIndex)
        {
            Storage.SelectedPlayerPrefab = this.characterPrefabs[classIndex];

            if (this.playerPreview != null)
            {
                MonoBehaviour.Destroy(this.playerPreview.gameObject);
            }

            this.playerPreview = MonoBehaviour.Instantiate(Storage.SelectedPlayerPrefab, this.playerPreviewPosition);
            
            this.DisableBehaviour<PlayerDriver>(this.playerPreview);
            this.DisableBehaviour<SpriteManager>(this.playerPreview);
            this.DisableBehaviour<SpriteAnimator>(this.playerPreview);

            Rigidbody rigidbody = this.playerPreview.GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            rigidbody.useGravity = false;

            this.playerPreview.transform.localPosition = Vector3.zero;
            this.playerPreview.transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="CharacterCreationMenu"/> when it is first enabled.
        /// </summary>
        private void Start()
        {
            this.SelectClass(Random.Range(0, this.characterPrefabs.Length));
            this.RandomizeBoni();
        }

        /// <summary>
        ///     Sets the bonus text
        /// </summary>
        /// <param name="text">The UI Text</param>
        /// <param name="stat">The stat</param>
        private void SetBonusText(Text text, Main.Stat stat)
        {
            text.text = "+" + Regex.Replace(stat.ToString(), @"[A-Z]", this.AddSpace);
        }

        /// <summary>
        ///     Adds a space to a match and returns it.
        /// </summary>
        /// <param name="match">The match</param>
        /// <returns>The matched string with a space prefixed</returns>
        private string AddSpace(Match match)
        {
            return " " + match.Value;
        }

        /// <summary>
        ///     Disables behaviour on the given game object if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the behaviour</typeparam>
        /// <param name="component">Any component of the GameObject</param>
        private void DisableBehaviour<T>(Component component) where T : Behaviour
        {
            T behaviour = component.GetComponent<T>();

            if (behaviour != null)
            {
                behaviour.enabled = false;
            }
        }
    }
}