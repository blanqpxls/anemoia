namespace ParadisisNostalga.Wheel
{
    using Godot;
    using System;
    using Engine.States; // Add this at the top to use eStates and States

    public partial class Character : CharacterBody2D
    {
        [Export] public float Speed = 300.0f;
        [Export] public float JumpVelocity = 600.0f;
        [Export] public float AttackDamage = 10f;
        [Export] public float AttCooldown = 0.5f;
        [Export] public float Health = 500.0f;
        [Export] public int SymphonProfundity = 25;
        [Export] public string iKey = ""; // The id of the belligerant that can accept input

        private float _attackTimer = 0f;
        private States stateManager;

        public override void _Ready()
        {
            base._Ready();
            stateManager = GetNode<States>("States"); // Ensure the States node is properly set up
            // Assign ScriptKey to this actor and its States node
            if (string.IsNullOrEmpty(iKey))
                iKey = Guid.NewGuid().ToString();
            if (stateManager != null)
                stateManager.ScriptKey = iKey;
        }

        public void SimpleAIMove(string currentControlId)
        {
            // Only accept input if this belligerant's TargetId matches the current control id
            if (iKey != currentControlId)
                return;
            // Example: randomly move left or right every second
            if (GD.Randi() % 100 < 2) // ~2% chance per frame to change direction
            {
                int dir = (int)(GD.Randi() % 3) - 1; // -1 (left), 0 (idle), 1 (right)
                if (dir == -1)
                {
                    Input.ActionPress("ui_left");
                    Input.ActionRelease("ui_right");
                }
                else if (dir == 1)
                {
                    Input.ActionPress("ui_right");
                    Input.ActionRelease("ui_left");
                }
                else
                {
                    Input.ActionRelease("ui_left");
                    Input.ActionRelease("ui_right");
                }
            }
        }

        // Executive override: if true, this belligerant is controlled by the player, not AI
        public bool ExecutiveOverride { get; set; } = false;

        // Set executive override (call this to take or release control)
        public void SetExecutiveOverride(bool enabled)
        {
            ExecutiveOverride = enabled;
            if (enabled)
            {
                GD.Print($"Executive override enabled for {Name} (iKey: {iKey})");
            }
            else
            {
                GD.Print($"Executive override disabled for {Name} (iKey: {iKey})");
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            // Feed input from IStateTable to this actor's States node each frame
            if (stateManager != null)
                stateManager.FeedInputFromStateTable();
            _attackTimer -= (float)delta;
            // Only run AI if not under executive override
            if (!ExecutiveOverride)
            {
                SimpleAIMove(CurrentControlId);
            }
            // Do not call stateManager._PhysicsProcess(delta) directly; let Godot handle it
        }

        // This should be set by your game manager or test harness
        public static string CurrentControlId = "";

        public void TakeDamage(int damage)
        {
            Health -= damage;
            GD.Print($"{Name} took {damage} damage! Health: {Health}");

            if (Health <= 0)
            {
                if (stateManager != null)
                    stateManager.ChangeState(eStates.Retired); // Use Retired for Dead state
            }
            else
            {
                if (stateManager != null)
                    stateManager.ChangeState(eStates.Staggered);
            }
        }

        public virtual void Die()
        {
            GD.Print($"{Name} has died.");
            QueueFree(); // Removes entity from scene
        }

        public virtual void Attack()
        {
            if (_attackTimer > 0)
            {
                return; // Cooldown check
            }

            GD.Print($"{Name} attacks for {AttackDamage} damage!");
            _attackTimer = AttCooldown;
            if (stateManager != null)
                stateManager.ChangeState(eStates.Attacking); // Transition to Attacking state
        }

        public virtual void UseAbility(string abilityName)
        {
            GD.Print($"{Name} used {abilityName}!");
            // Add logic for transitioning to specific ability states if needed
        }

        public void OnStateChanged(eStates newState)
        {
            GD.Print($"{Name} transitioned to state: {newState}");
            // Handle any additional logic when the state changes
        }
    } // end class Character
} // end namespace
