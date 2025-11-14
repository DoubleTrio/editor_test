using System.Collections.Generic;

namespace AvaloniaTest.Models;

public class ModHeader
{
    public string Name { get; set; }
    public string Key { get; set; }


    public ModHeader(string name, string key)
    {
        Name = name;
        Key = key;
    }
}