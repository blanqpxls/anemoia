using Godot;
using System;
using A

public class Player : Belligerent
{
    // The player's attunement modifier for leveling and scaling stats.
    public AttunementModifier Attunement { get; private set; }

    // Reference to the state machine component.
    private Engine.States.States stateMachine;

    public override void _Ready()
    {
        // Initialize the base class logic.
        base._Ready();

        // Create and initialize the attunement modifier.
        Attunement = new AttunementModifier(initialLevel: 1);

        // Option 1: If the state machine is a child node of the player:
        stateMachine = GetNode<Engine.States.States>("States");

        // Option 2: Alternatively, if the state machine is a separate node in the scene tree,
        // you might need to use a different path, e.g.:
        // stateMachine = GetNode<Engine.States.States>("../States");

        GD.Print("Player is ready. Attunement Level: " + Attunement.Level);
    }

    public override void _PhysicsProcess(float delta)
    {
        // Update the state machine for handling movement, attacks, etc.
        stateMachine?._PhysicsProcess(delta);

        // Additional player-specific physics logic can be placed here.
        // For example, you might integrate attunement modifiers into combat calculations:
        // float modifiedDamage = Attunement.ApplyDamageModifier(AttackDamage);
    }

    // A method to level up the player (and update attunement values)
    public void LevelUp()
    {
        Attunement.LevelUp();
        GD.Print("Player leveled up! New Attunement Level: " + Attunement.Level);
    }
}
