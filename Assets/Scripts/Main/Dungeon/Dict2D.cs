namespace SAE.RoguePG.Main.Dungeon
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SAE.RoguePG.Main.BattleDriver;
    using SAE.RoguePG.Main.Driver;
    using UnityEngine;

    /// <summary>
    ///     A "2D-Dictionary". In short, it has two keys per value.
    /// </summary>
    /// <typeparam name="TKey">The type for keys</typeparam>
    /// <typeparam name="TValue">The type for values</typeparam>
    public class Dict2D<TKey, TValue>
    {
        /// <summary>
        ///     Stores values.
        /// </summary>
        private Dictionary<TKey, Dictionary<TKey, TValue>> storage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dict2D{TKey, TValue}"/> class.
        /// </summary>
        public Dict2D() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dict2D{TKey, TValue}"/> class and
        ///     sets a default return value.
        /// </summary>
        /// <param name="defaultValue">The value to return, if the coordinate attempted to be accessed is not assigned</param>
        public Dict2D(TValue defaultValue)
        {
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        ///     The value to return, if the coordinate attempted to be accessed is not assigned.
        /// </summary>
        public TValue DefaultValue { get; set; }

        /// <summary>
        ///     Gets or sets a value at a given coordinate.
        /// </summary>
        /// <param name="x">The first key</param>
        /// <param name="y">The second key</param>
        /// <returns>The value stored or <seealso cref="DefaultValue"/></returns>
        public TValue this[TKey x, TKey y]
        {
            get
            {
                Dictionary<TKey, TValue> row;

                if (this.storage.TryGetValue(x, out row))
                {
                    TValue value;
                    return row.TryGetValue(y, out value) ? value : this.DefaultValue;
                }

                return this.DefaultValue;
            }

            set
            {
                if (!this.storage.ContainsKey(x))
                {
                    this.storage[x] = new Dictionary<TKey, TValue>();
                }

                this.storage[x][y] = value;
            }
        }

        /// <summary>
        ///     Returns whether there is a value at the given coordinate
        /// </summary>
        /// <param name="x">The first key</param>
        /// <param name="y">The second key</param>
        /// <returns>Whether there was a value stored</returns>
        public bool HasValueAtCoordinate(TKey x, TKey y)
        {
            Dictionary<TKey, TValue> row;

            if (this.storage.TryGetValue(x, out row) && row.ContainsKey(y))
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        ///     Attempts to get a value and returns whether it was successful.
        /// </summary>
        /// <param name="x">The first key</param>
        /// <param name="y">The second key</param>
        /// <param name="value">The value stored or <seealso cref="DefaultValue"/></param>
        /// <returns>Whether there was a value stored</returns>
        public bool TryGetValue(TKey x, TKey y, out TValue value)
        {
            if (this.HasValueAtCoordinate(x, y))
            {
                value = this.storage[x][y];
                return true;
            }
            
            value = this.DefaultValue;
            return false;
        }
    }
}