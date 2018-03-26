using System;
using System.Collections;
using System.Collections.Generic;

namespace Tabler
{
    /// <summary>
    /// A text based table of data
    /// <remarks>
    /// Dealing with data as allays strings might not be terribly
    /// efficient, but it's alot easier
    /// </remarks>
    /// </summary>
    class Table : Dictionary<string, List<string>>
    {
        public Table() {
            Rows = new RowEnum(this);
        }

        /// <summary>
        /// Total row count (not including the headers row!)
        /// </summary>
        public int RowCount {
            get {
                try
                {
                    return this[this.Keys[0]].Count;
                }
                catch (ArgumentOutOfRangeException e) {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Iterator for the rows of this table
        /// </summary>
        public RowEnum Rows { get; private set; }

        /**
         * Since OrderedDictionary is not in the .NET Standard,
         * this hacks together a new version of key keeping,
         * to make the key iteration order deterministic
         */
        new public List<string> Keys
        {
            get; private set;
        } = new List<string>();


        new public bool ContainsKey(string key)
        {
            return Keys.Contains(key) && base.ContainsKey(key);
        }

        /// <summary>
        /// Adds an element to a specific column
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="item"></param>
        public void AddToColumn(string colName, string item) {
            if (!this.ContainsKey(colName)) {
                Keys.Add(colName);
                this[colName] = new List<string>();
            }
            this[colName].Add(item);
        }

        /// <summary>
        /// The length of the header including padding chars for each element
        /// </summary>
        public int HeaderLength {
            get {
                var totalLength = 1; //assume padding
                foreach (var key in this.Keys) {
                    var longestElementLength = GetLongestElementOfColumn(key).Length;
                    totalLength += longestElementLength + 1; // assume padding of '|'
                }
                return totalLength;
            }
        }

        /// <summary>
        /// Fetches the longest element in the column
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetLongestElementOfColumn(string key) {
            var elements = new List<string>
                    {
                        key
                    };
            elements.AddRange(this[key]);

            var longestElem = "";
            foreach (var e in elements)
            {
                if (e.Length > longestElem.Length)
                {
                    longestElem = e;
                }
            }
            return longestElem;
        }

        /// <summary>
        /// A private class to enumerate over the rows of the table
        /// Just to add some syntax sugar and make the foreach look nice
        /// </summary>
        public class RowEnum : IEnumerable<List<string>>
        {
            private Table T;
            public RowEnum(Table t) {
                T = t;
            }
            public IEnumerator<List<string>> GetEnumerator()
            {
                yield return T.Keys;
                var i = 0;
                while (i < T.RowCount)
                {
                    var row = new List<string>();
                    foreach (var key in T.Keys) {
                        row.Add(T[key][i]);
                    }
                    i++;
                    yield return row;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
