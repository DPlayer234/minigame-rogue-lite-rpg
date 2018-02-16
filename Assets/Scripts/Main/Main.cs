namespace SAE.RoguePG.Main
{
    using UnityEngine;

    /// <summary>
    ///     An enum for character statistics
    /// </summary>
    public enum Stat
    {
        Random = -1,
        MaximumHealth,
        PhysicalDamage,
        MagicalDamage,
        Defense,
        TurnSpeed
    }

    /// <summary>
    ///     General functions
    /// </summary>
    public static class MainGeneral
    {
        /// <summary>
        ///     Get a random stat
        /// </summary>
        /// <param name="except">A stat which isn't allowed</param>
        /// <returns>A stat</returns>
        public static Stat GetRandomStat(Stat? except = null)
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    return MainGeneral.RFSTUEER(Stat.MaximumHealth, except);
                case 1:
                    return MainGeneral.RFSTUEER(Stat.PhysicalDamage, except);
                case 2:
                    return MainGeneral.RFSTUEER(Stat.MagicalDamage, except);
                case 3:
                    return MainGeneral.RFSTUEER(Stat.Defense, except);
                case 4:
                    return MainGeneral.RFSTUEER(Stat.TurnSpeed, except);
            }

            // This should never happen
            throw new System.Exception("There seems to be no fitting stat found...?");
        }

        /// <summary>
        ///     Return First Stat Unless Equal Else Random.
        /// </summary>
        /// <param name="stat">The stat to possibly return</param>
        /// <param name="except">The stat which to check against</param>
        /// <returns>A stat</returns>
        private static Stat RFSTUEER(Stat stat, Stat? except = null)
        {
            return stat != except ? stat : MainGeneral.GetRandomStat(except);
        }
    }
}
