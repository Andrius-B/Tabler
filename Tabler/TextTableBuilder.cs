using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace Tabler
{
    /// <summary>
    /// Class made generic to have compile time assurance, that all objects are of the
    /// same class (otherwise the table might not make sense)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TextTableBuilder<T> where T: class
    {
        private KeyList Table { get; } = new KeyList();
        private Dictionary<string, MemberInfo> ReflectionCache = new Dictionary<string, MemberInfo>();
        private Type DataType;
        private bool IsCached { get; set; } = false;
        private StreamWriter OutputStream { get; set; }

        /// <summary>
        /// Sets the printing output stream
        /// </summary
        /// <returns></returns>
        public TextTableBuilder<T> SetOutput(StreamWriter sw) {
            OutputStream = sw;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public TextTableBuilder<T> Clear()
        {
            Table.Clear();
            ReflectionCache.Clear();
            DataType = null;
            IsCached = false;
            OutputStream = null;
            return this;
        }

        public TextTableBuilder<T> SetHeaders(params string[] headers)
        {
            Table.SetHeaders(headers);
            return this;
        }

        /// <summary>
        /// Sets a header by a property selection,
        /// to have some help while typing the field names
        /// and not rely on ordering
        /// </summary>
        public TextTableBuilder<T> SetHeader<TKey>(Expression<Func<T,  TKey>> keySelector, string header) {
            //this works by parsing the lambda expression argument
            var key = ((MemberExpression)keySelector.Body).Member.Name;
            Table.SetHeader(key, header);
            return this;
        }

        /// <summary>
        /// Adds an object to the internal data table,
        /// by parsing all the visible fields and properties
        /// In the order of declaration.
        /// </summary>
        /// <returns></returns>
        public TextTableBuilder<T> AddObject(T o) {
            if (!IsCached)
                CacheBy(o);

            if (this.IsCached && DataType == o.GetType())
            {
                for(int i = 0; i < Table.Keys.Count; i++)
                {
                    MemberInfo currentInfo = ReflectionCache[Table.Keys[i]];
                    var foundKey = "";
                    switch (currentInfo.MemberType)
                    {
                        case MemberTypes.Property:
                            var prop = currentInfo as PropertyInfo;
                            Table.AddToColumn(prop.Name, prop.GetValue(o).ToString());
                            foundKey = prop.Name;
                            break;
                        case MemberTypes.Field:
                            var field = currentInfo as FieldInfo;
                            Table.AddToColumn(field.Name, field.GetValue(o).ToString());
                            foundKey = field.Name;
                            break;
                    }

                    var attribute = (TableColumn)currentInfo.GetCustomAttribute(typeof(TableColumn), false);
                    if (attribute != null && attribute.HeaderSet) {
                        Table.SetHeader(foundKey, attribute.Header);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Identifies the relevant (public and instanced) fields and properties, sorts them by order of declaration.
        /// Caches the reflection data for future use.
        /// </summary>
        /// <param name="o"></param>
        private void CacheBy(T o)
        {
            this.DataType = o.GetType();
            // GetMembers does not necessarily acquire fields and properties in the way they are declared.
            // Although officially somewhat discouraged, sorting by MetadataToken is a fairly reliable way of getting the order correct.
            foreach (MemberInfo member in DataType.GetMembers().Where(x => x.MemberType == MemberTypes.Property || x.MemberType == MemberTypes.Field).OrderBy(x => x.MetadataToken))
            {
                Table.Add(member.Name, new List<string>());
                ReflectionCache.Add(member.Name, member);
            }
            this.IsCached = true;
        }

        public TextTableBuilder<T> AddObjects(IEnumerable<T> objects) {
            foreach (var o in objects)
                this.AddObject(o);
            return this;
        }

        /// <summary>
        /// Prints a table to output stream writter.
        /// </summary>
        public void PrintToStream() {
            List<string> columnFormats = new List<string>();
            foreach (string key in Table.Keys) {
                int len = Table.GetLongestElementOfColumn(key).Length;
                // format my format, baby!
                columnFormats.Add(String.Format("{{0,{0}}}|", len));
            }
            string header = new String('-', Table.TableHeaderLength());
            OutputStream.WriteLine(header);
            foreach (var row in Table.Rows) {
                var index = 0;
                OutputStream.Write('|');
                foreach (var element in row) {
                    OutputStream.Write(String.Format(columnFormats[index], element));
                    index++;
                }
                OutputStream.WriteLine();
                OutputStream.WriteLine(header);
            }
        }
    }
}
