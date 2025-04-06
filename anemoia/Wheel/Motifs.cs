using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Motifs

{
    
    public class Motif
{
    public int Id { get; set; }
    public string Name { get; set; }
    // Additional properties such as damage, cooldown, etc.

    public Motif(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
}