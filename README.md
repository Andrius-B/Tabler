# Tabler
Makes text tables out of data, used for uni
In our university most of the tasks require that you print out both the input data and the results in tables, 
Tabler is a utility to help out with that chore

# Example
Consider a data class:
```C#
class DataClass
    {
        public string Field = "Hello world!";
        public float Prop2 { get; private set; } = 5;
        public float Prop { get; set; } = 10;
    }
```
To print it in a file or the console:
```C#
var dataObject = new DataClass();

using (var sw = new StreamWriter(Console.OpenStandardOutput()))
    new TextTableBuilder<DataClass>()
        .AddObject(dataObject)
        .SetOutput(sw)
        .Print();
```
and the resulting output:

```
-------------------------
|Prop2|Prop|       Field|
-------------------------
|    5|  10|Hello world!|
-------------------------
```

Lists are also accepted to the table.
