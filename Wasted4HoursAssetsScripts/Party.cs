namespace SAE.RoguePG.Main.Driver
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Represents a party of a fixed maximum size
    /// </summary>
    /// <typeparam name="T">The type of the party</typeparam>
    public class Party<T> : IEnumerable<T>, IList<T> where T : BaseDriver
    {
        /// <summary>
        ///     The party array
        /// </summary>
        private T[] party;

        /// <summary>
        ///     The count
        /// </summary>
        private int count;

        /// <summary>
        ///     The maximum size of the party
        /// </summary>
        private int maximumSize;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Party"/> class.
        /// </summary>
        /// <param name="maximumSize">The maximum party size</param>
        public Party(int maximumSize)
        {
            this.maximumSize = maximumSize;

            this.Clear();
        }

        /// <summary>
        ///     Gets or sets the element at <paramref name="index"/>
        /// </summary>
        /// <param name="index">The index of the item</param>
        public T this[int index]
        {
            get
            {
                return this.party[index];
            }

            set
            {
                this.party[index] = value;

                if (index >= this.Count)
                {
                    this.Count = index + 1;
                }
            }
        }

        /// <summary>
        ///     The maximum size of the party
        /// </summary>
        public int MaximumSize { get { return this.maximumSize; } }

        /// <summary>
        ///     The current amount of party members
        /// </summary>
        public int Count { get { return this.count; } private set { this.count = value; } }

        /// <summary>
        ///     Whether this Collection is read only (always false)
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        ///     Adds an item to the end of the list
        /// </summary>
        /// <param name="item">The item</param>
        public void Add(T item)
        {
            if (this.Count >= this.MaximumSize) throw new NotSupportedException("Cannot add items to Party<T> when it is at maximum capacity.");
            
            this[++this.Count] = item;
        }

        /// <summary>
        ///     Clears the collection
        /// </summary>
        public void Clear()
        {
            this.party = new T[this.MaximumSize];
            this.Count = 0;
        }

        /// <summary>
        ///     Returns whether this collection contains the given item
        /// </summary>
        /// <param name="item">The item to search for</param>
        /// <returns>Whether it is in this collection</returns>
        public bool Contains(T item)
        {
            return this.IndexOf(item) >= 0;
        }

        /// <summary>
        ///     Copies the contents of the collection to the given <paramref name="array"/>, starting at <paramref name="arrayIndex"/>
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="arrayIndex">The starting index in the array</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Length < arrayIndex + this.Count) throw new ArgumentException("Array cannot store all elements of this collection.", "array");

            int index = 0;
            foreach (T item in this)
            {
                array[arrayIndex + index] = item;
                ++index;
            }
        }

        /// <summary>
        ///     Returns the index of the item within the list or -1 if it is not contained
        /// </summary>
        /// <param name="item">The item to search for</param>
        /// <returns>The index in the list</returns>
        public int IndexOf(T item)
        {
            int index = 0;
            foreach (T itemInList in this)
            {
                if (itemInList == item)
                {
                    return index;
                }
                ++index;
            }

            return -1;
        }

        /// <summary>
        ///     Inserts an <paramref name="item"/> into the list at the given <paramref name="index"/>
        /// </summary>
        /// <param name="index">The index to insert it at</param>
        /// <param name="item">The item to insert</param>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > this.Count + 1) throw new ArgumentOutOfRangeException("index");
            if (this.Count >= this.MaximumSize) throw new NotSupportedException("Cannot add items to Party<T> when it is at maximum capacity.");

            for (int i = this.Count; i > index; i--)
            {
                this[i] = this[i - 1];
            }

            this[index] = item;
            ++this.Count;
        }

        /// <summary>
        ///     Removes the first occurence of the given item from the list
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>Whether it was successful</returns>
        public bool Remove(T item)
        {
            int index = this.IndexOf(item);

            if (index < 0)
            {
                return false;
            }
            else
            {
                this.RemoveAt(index);
                return true;
            }
        }

        /// <summary>
        ///     Removes an item at the given index
        /// </summary>
        /// <param name="index">The index to remove an item from</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException("index");

            for (int i = index; i < this.Count - 1; i++)
            {
                this[i] = this[i + 1];
            }

            this[--this.Count] = null;
        }

        /// <summary>
        ///     Returns a generic iterator for type <typeparamref name="T"/>
        /// </summary>
        /// <returns>An iterator</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (T item in this.party)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        ///     Returns an iterator for type <typeparamref name="T"/>
        /// </summary>
        /// <returns>An iterator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T item in this.party)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }
    }
}