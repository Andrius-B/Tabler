using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Tabler;

namespace TableTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            /**
             * Data prep
             */
            var d = new DataClass();
            var list = GenerateRandomList();

            /**
             * The table builder requires a stream writter, and 
             * its up to the user to decide whether to print to console, a file,
             * or anything else
             */
            using (var sw = new StreamWriter(Console.OpenStandardOutput()))
                new TextTableBuilder<DataClass>()
                    .AddObject(d) //note : the order of adding items matters!
                    .AddObjects(list)// they appear in the table as are added here
                    .SetOutput(sw)
                    .SetHeaders("Test1", "Test2", "Test3")
                    .SetHeader(f=>f.TestProp2VeryLongName_Is_Still_Not_cool, "ChangedHeader")
                    .PrintToStream();
            s.Stop();
            Console.WriteLine("Test completed in {0}ms!!", s.ElapsedMilliseconds);
            Console.ReadKey();
        }

        /// <summary>
        /// Generates some randomly initialized data objects
        /// </summary>
        /// <returns></returns>
        static List<DataClass> GenerateRandomList() {
            var r = new Random();
            var list = new List<DataClass>();
            var i = 0;
            while (i < 30) {
                list.Add(
                    new DataClass() {
                        TestField = "Generated",
                        TestProp = r.Next(0, 3000000)
                    }
                );
                i++;
            }
            return list;
        }
    }

    /// <summary>
    /// A simple data class, the changes here should reflect in the printed table!
    /// </summary>
    class DataClass
    {

        public string TestField = "Hello world!";

        public float TestProp2VeryLongName_Is_Still_Not_cool { get; private set; } = 5;

        public float TestProp { get; set; } = 10;

    }
}
