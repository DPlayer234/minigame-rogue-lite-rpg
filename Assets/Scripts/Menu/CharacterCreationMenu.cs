namespace SAE.RoguePG.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

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
        public int paladinIndex = 0;

        /// <summary> The index of the assassin </summary>
        public int assassinIndex = 0;

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
        }

        /// <summary>
        ///     Select a class based on its index in <seealso cref="characterPrefabs"/>
        /// </summary>
        /// <param name="classIndex">The index of the class</param>
        private void SelectClass(int classIndex)
        {
            Storage.SelectedPlayerPrefab = this.characterPrefabs[classIndex];
        }
    }
}