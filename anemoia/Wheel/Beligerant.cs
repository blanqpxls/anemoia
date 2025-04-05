using Godot;
using Overture.Characters;
using Overture.Leveling;
using Engine.States;

namespace Overture.Characters
{
    public class Player : Belligerent
    {
        // Player-specific leveling modifier.
        public AttunementModifier Attunement { get; private set; }

        // Reference to the state machine controlling movement and combat behavior.
        private States StateMachine;

        public override void _Ready()
        {
            // Call the base initialization.
            base._Ready();

            // Initialize the attunement modifier.
            Attunement = new AttunementModifier(1);
            GD.Print("Player ready. Attunement Level: " + Attunement.Level);

            // Assuming the state machine is a child node called "States"
            StateMachine = GetNode<States>("States");
        }

        public override void _PhysicsProcess(float delta)
        {
            // Update the state machine.
            StateMachine?._PhysicsProcess(delta);
        }

        public void LevelUp()
        {
            Attunement.LevelUp();
            GD.Print("Player leveled up! New Attunement Level: " + Attunement.Level);
        }
    }
}
