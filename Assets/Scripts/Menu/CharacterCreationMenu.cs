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
            Storage.BonusStat2 = this.GetRandomStat(
                Storage.BonusStat1 = this.GetRandomStat(null));
        }

        /// <summary>
        ///     Select a class based on its index in <seealso cref="characterPrefabs"/>
        /// </summary>
        /// <param name="classIndex">The index of the class</param>
        private void SelectClass(int classIndex)
        {
            Storage.SelectedPlayerPrefab = this.characterPrefabs[classIndex];
        }

        /// <summary>
        ///     Get a random stat
        /// </summary>
        /// <param name="except">A stat which isn't allowed</param>
        /// <returns>A stat</returns>
        private Main.Stat GetRandomStat(Main.Stat? except = null)
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    return this.RFSTUEER(Main.Stat.MaximumHealth, except);
                case 1:
                    return this.RFSTUEER(Main.Stat.PhysicalDamage, except);
                case 2:
                    return this.RFSTUEER(Main.Stat.MagicalDamage, except);
                case 3:
                    return this.RFSTUEER(Main.Stat.Defense, except);
                case 4:
                    return this.RFSTUEER(Main.Stat.TurnSpeed, except);
            }

            // This should never happen
            throw new Exceptions.MenuException("There seems to be no fitting stat found...?");
        }

        /// <summary>
        ///     Return First Stat Unless Equal Else Random.
        /// </summary>
        /// <param name="stat">The stat to possibly return</param>
        /// <param name="except">The stat which to check against</param>
        /// <returns>A stat</returns>
        private Main.Stat RFSTUEER(Main.Stat stat, Main.Stat? except)
        {
            return stat != except ? stat : this.GetRandomStat(except);
        }
    }
}