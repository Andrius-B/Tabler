using System;
using System.Collections.Generic;
using System.Text;

namespace Tabler
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class TableColumn: Attribute
    {
        public bool HeaderSet { get; private set; } = false;
        public string Header { get; private set; } = "";

        public TableColumn(string header) {
            HeaderSet = true;
            Header = header;
        }
    }
}
