using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Tabler
{
    /// <summary>
    /// Class made generic to have compile time assurance, that all objects are of the
    /// same class (otherwise the table might not make sense)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TextTableBuilder<T> where T: class
    {
        private StreamWriter Output { get; set; }
        private Table Table { get; } = new Table();

        /// <summary>
        /// Sets the printing output stream
        /// </summary
        /// <returns></returns>
        public TextTableBuilder<T> SetOutput(StreamWriter sw) {
            Output = sw;
            return this;
        }

        /// <summary>
        /// Adds an object to the internal data table,
        /// by parsing all the visible fields and properties
        /// In the order of declaration.
        /// </summary>
        /// <returns></returns>
        public TextTableBuilder<T> AddObject(T o) {
            Type type = o.GetType();

            foreach (var member in type.GetMembers()) {
                switch (member.MemberType) {
                    case MemberTypes.Property:
                        var prop = member as PropertyInfo;
                        Table.AddToColumn(prop.Name, prop.GetValue(o).ToString());
                        break;
                    case MemberTypes.Field:
                        var field = member as FieldInfo;
                        Table.AddToColumn(field.Name, field.GetValue(o).ToString());
                        break;
                }
            }
            return this;
        }


        public TextTableBuilder<T> AddObjects(IEnumerable<T> objects) {
            foreach (var o in objects) {
                this.AddObject(o);
            }
            return this;
        }

        /// <summary>
        /// Prints a table to output stream writter.
        /// </summary>
        public void Print() {
            var columnFormats = new List<string>();
            foreach (var key in Table.Keys) {
                var len = Table.GetLongestElementOfColumn(key).Length;
                
                // format my format, baby!
                columnFormats.Add(String.Format("{{0,{0}}}|", len));
            }
            var header = new String('-', Table.HeaderLength);
            Output.WriteLine(header);
            foreach (var row in Table.Rows) {
                var index = 0;
                Output.Write('|');
                foreach (var element in row) {
                    Output.Write(String.Format(columnFormats[index], element));
                    index++;
                }
                Output.WriteLine();
                Output.WriteLine(header);
            }
        }
    }
}
