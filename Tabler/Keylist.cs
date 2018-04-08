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
    class KeyList : Dictionary<string, List<string>>
    {
        /// <summary>
        /// 
        /// </summary>
        new public List<string> Keys { get; private set; }
        /// <summary>
        /// A string list for storing custom headers for the flags and properties.
        /// </summary>
        public List<string> Headers { get; private set; }
        /// <summary>
        /// A flag for marking if fancier headers were provided.
        /// </summary>
        public bool HeadersSet { get; private set; }
        /// <summary>
        /// Iterator for the rows of this table
        /// </summary>
        public RowEnum Rows { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        public void SetHeaders(params string[] headers)
        {
            if (headers.Length != Keys.Count)
                throw new Exception("Mismatch between the number of columns in the table and the number of headers provided");
            else
            {
                Headers.Clear();
                Headers.AddRange(headers);
            }
            HeadersSet = true;
        }
        /// <summary>
        /// Wipes the KeyList to a blank state.
        /// </summary>
        new public void Clear()
        {
            Keys = new List<string>();
            Headers = new List<string>();
            HeadersSet = false;
            Rows = new RowEnum(this);
        }
        /// <summary>
        /// Constructor with no parameters.
        /// </summary>
        public KeyList()
        {
            Keys = new List<string>();
            Headers = new List<string>();
            HeadersSet = false;
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
                catch (ArgumentOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        /**
         * Since OrderedDictionary is not in the .NET Standard,
         * this hacks together a new version of key keeping,
         * to make the key iteration order deterministic
         */
        new public bool ContainsKey(string key)
        {
            return Keys.Contains(key) && base.ContainsKey(key);
        }
        /// <summary>
        /// Similar functionality to the Add from Dictionary, created to be used wtih the enumerable keys in mind.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        new public void Add(string key, List<string> value)
        {
            if (this.Keys.Contains(key))
                return;
            else
            {
                Keys.Add(key);
                this[key] = value;
            }
        }
        /// <summary>
        /// Adds an element to a specific column
        /// </summary>
        public void AddToColumn(string colName, string item)
        {
            if (!this.ContainsKey(colName))
            {
                Keys.Add(colName);
                this[colName] = new List<string>();
            }
            this[colName].Add(item);
        }
        /// <summary>
        /// The length of the header, including padding chars for each element.
        /// </summary>
        public int HeaderLength() {
            var totalLength = 1; //assume padding
                foreach (var key in this.Keys) {
                    totalLength += GetLongestElementOfColumn(key).Length + 1; // assume padding of '|'
                }
                return totalLength;
        }
        /// <summary>
        /// Fetches the longest element in the column.
        /// </summary>
        public string GetLongestElementOfColumn(string columnName)
        {
            string longestElem = "";
            if (HeadersSet)
                longestElem = Headers[Keys.IndexOf(columnName)];
            else
                longestElem = columnName;
            foreach (string element in this[columnName])
            {
                string currentElem = element.ToString();
                if (currentElem.Length > longestElem.Length)
                    longestElem = currentElem;
            }
            return longestElem;
        }
        /// <summary>
        /// A private class to enumerate over the rows of the table
        /// Just to add some syntax sugar and make the foreach look nice
        /// </summary>
        public class RowEnum : IEnumerable<List<string>>
        {
            private KeyList T;
            public RowEnum(KeyList t) {
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
