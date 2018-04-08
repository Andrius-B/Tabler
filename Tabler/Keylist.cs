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
        /// A header dictionary, basically saying that
        /// specific keys are to be called by specific names from this dictionary
        /// </summary>
        /// <remarks>
        /// Layout of this dictionary is <string Key, string Header>
        /// </remarks>
        private Dictionary<string, string> HeadersDict { get; set; }

        /// <summary>
        /// Public access for the internal header dictionary
        /// </summary>
        /// <remarks>
        /// This might contain actual headers, or just the same key names,
        /// depending on what was set, and the intended use is in the table headers
        /// </remarks>
        public List<string> Headers {
            get {
                var list = new List<string>();
                foreach (var key in Keys) {
                    list.Add(GetHeaderForKey(key));
                }
                return list;
            }
        }

        /// <summary>
        /// Iterator for the rows of this table
        /// </summary>
        public RowEnum Rows { get; private set; }

        /// <summary>
        /// Sets a list of headers for the table
        /// </summary>
        /// <param name="headers"></param>
        public void SetHeaders(params string[] headers)
        {
            if (headers.Length != Keys.Count)
                throw new Exception("Mismatch between the number of columns in the table and the number of headers provided");
            else
            {
                HeadersDict.Clear();
                for (int i = 0; i < Keys.Count; i++) {
                    HeadersDict.Add(Keys[i], headers[i]);
                }
            }
        }

        public void SetHeader(string key, string header) {
            HeadersDict[key] = header;
        }

        /// <summary>
        /// Wipes the KeyList to a blank state.
        /// </summary>
        new public void Clear()
        {
            Keys.Clear();
            HeadersDict.Clear();
            Rows = new RowEnum(this);
        }
        /// <summary>
        /// Constructor with no parameters.
        /// </summary>
        public KeyList()
        {
            Keys = new List<string>();
            HeadersDict = new Dictionary<string, string>();
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
        public int TableHeaderLength() {
            var totalLength = 1; //assume padding
                foreach (var key in this.Keys) {
                    totalLength += GetLongestElementOfColumn(key).Length + 1; // assume padding of '|'
                }
                return totalLength;
        }
        /// <summary>
        /// Fetches the longest element in the column.
        /// (Used for setting the approrpiate format padding)
        /// </summary>
        public string GetLongestElementOfColumn(string columnName)
        {
            string longestElem = GetHeaderForKey(columnName);
            foreach (string element in this[columnName])
            {
                string currentElem = element.ToString();
                if (currentElem.Length > longestElem.Length)
                    longestElem = currentElem;
            }
            return longestElem;
        }

        public string GetHeaderForKey(string columnKey) {
            if (HeadersDict.ContainsKey(columnKey))
            {
                return HeadersDict[columnKey];
            }
            else {
                return columnKey;
            }
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
                yield return T.Headers;
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
