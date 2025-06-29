using Godot;
using System;

namespace ParadisisNostalga.Wheel
{
    // Belligerant wraps a Character and provides integration for state/input systems
    public partial class Belligerant : Character
    {
        public string ScriptKey { get; private set; }
        public Player PlayerRef { get; private set; }
        public Composition.CompositionData Composition { get; private set; }

        public Belligerant(string scriptKey, Player player, Composition.CompositionData composition)
        {
            ScriptKey = scriptKey;
            PlayerRef = player;
            Composition = composition;
            this.iKey = scriptKey;
            // Optionally, copy stats from composition
            this.Health = composition.BaseHealth;
            this.AttackDamage = composition.BaseDamage;
            // ...other stat assignments as needed
        }
        // Add any additional logic for Belligerant here
    }
}
