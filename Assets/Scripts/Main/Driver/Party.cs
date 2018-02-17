namespace SAE.RoguePG.Main.Driver
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Wraps a <seealso cref="List{T}"/> to be a fixed maximum size of <seealso cref="Party{T}.MaximumSize"/>
    ///     for use as an adventure party.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list</typeparam>
    [Serializable]
    public class Party<T> : IList<T> where T : BaseDriver
    {
        /// <summary>
        ///     The maximum size for a party.
        /// </summary>
        public const int MaximumSize = 5;

        /// <summary>
        ///     The list used to store the party
        /// </summary>
        private List<T> list;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Party{T}"/> class
        /// </summary>
        /// <param name="maximumSize">The maximum size of the party</param>
        public Party()
        {
            this.list = new List<T>(Party<T>.MaximumSize);
        }

        /// <summary>
        ///     Gets or sets the item at <paramref name="index"/>
        /// </summary>
        /// <param name="index">The index to check for</param>
        public T this[int index]
        {
            get
            {
                return this.list[index];
            }

            set
            {
                this.list[index] = value;
            }
        }

        /// <summary>
        ///     The current amount of items in the party
        /// </summary>
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        /// <summary>
        ///     Whether this collection is read-only (always false (?))
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return ((IList<T>)this.list).IsReadOnly;
            }
        }

        /// <summary>
        ///     Gets whether the capacity has already been filled
        /// </summary>
        public bool CapacityFilled
        {
            get
            {
                return this.Count >= Party<T>.MaximumSize;
            }
        }

        /// <summary>
        ///     Effectively returns the list used to store the party.
        ///     Just don't mess with it.
        /// </summary>
        /// <param name="party">The party</param>
        public static explicit operator List<T>(Party<T> party)
        {
            return party.list;
        }

        /// <summary>
        ///     Gets the leader of the party (or null if empty).
        /// </summary>
        /// <returns>The leader of the party</returns>
        public T GetLeader()
        {
            if (this.Count > 0)
            {
                return this[0];
            }

            return null;
        }

        /// <summary>
        ///     Moves an item from one index to another by calling
        ///     <seealso cref="RemoveAt(int)"/> and <seealso cref="Insert(int, T)"/>.
        /// </summary>
        /// <param name="fromIndex">The index the item is at the beginning</param>
        /// <param name="toIndex">The index the item should end up at</param>
        public void Move(int fromIndex, int toIndex)
        {
            T item = this[fromIndex];
            this.RemoveAt(fromIndex);
            this.Insert(toIndex, item);
        }

        /// <summary>
        ///     Adds a party member to the party
        /// </summary>
        /// <param name="item">The party member to add</param>
        public void Add(T item)
        {
            this.CheckForMaximumSizeException();

            this.list.Add(item);
        }

        /// <summary>
        ///     Clears the party
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        ///     Returns whether the given character is in the party
        /// </summary>
        /// <param name="item">The party member to check</param>
        /// <returns>Whether the character is in the party</returns>
        public bool Contains(T item)
        {
            return this.list.Contains(item);
        }

        /// <summary>
        ///     Copies this collection to an <paramref name="array"/>, starting at <paramref name="arrayIndex"/>
        /// </summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="arrayIndex">The starting index</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Gets an enumerator for iterating over this collection
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        ///     Returns the index of a given item within the party
        /// </summary>
        /// <param name="item">The item to search for</param>
        /// <returns>The index or -1 if it is not in the party</returns>
        public int IndexOf(T item)
        {
            return this.list.IndexOf(item);
        }

        /// <summary>
        ///     Inserts an item into the party
        /// </summary>
        /// <param name="index">The index to insert it at</param>
        /// <param name="item">The item to add</param>
        public void Insert(int index, T item)
        {
            this.CheckForMaximumSizeException();

            this.list.Insert(index, item);
        }

        /// <summary>
        ///     Removes an item from the collection
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>Whether it was removed</returns>
        public bool Remove(T item)
        {
            return this.list.Remove(item);
        }

        /// <summary>
        ///     Removes an item at a given index from the collection
        /// </summary>
        /// <param name="index">The index to remove it from</param>
        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        /// <summary>
        ///     Gets an enumerator for iterating over this collection
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        ///     Checks whether adding anything to the party would make <seealso cref="Count"/> exceed <seealso cref="MaximumSize"/>
        ///     and throws an exception if this is the case.
        /// </summary>
        private void CheckForMaximumSizeException()
        {
            if (this.CapacityFilled) throw new NotSupportedException("Party may not exceed maximum size.");
        }
    }
}
