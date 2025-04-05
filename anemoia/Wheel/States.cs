using Godot;
using System;
using System.Collections.Generic;
using Engine.eStates;

namespace Engine.States
{
    public enum eStates
    {
        Idle,
        Moving,
        Attacking,
        Hook
    }

    // The main state machine class.
    public partial class States : CharacterBody2D
    {
        public float Stamina { get; set; } = 100f;
        private IState.IState currentState;
        private Dictionary<eStates, IState.IState> stateMap;

        public const float Gravity = 9.8f;
        public const float HookPullSpeed = 400f;

        public override void _Ready()
        {
            stateMap = new Dictionary<eStates, IState.IState>()
            {
                { eStates.Idle, new IdleState(this) },
                { eStates.Moving, new MovingState(this) },
                { eStates.Attacking, new AttackingState(this) },
                { eStates.Retired, new DeadState(this) },
                { eStates.Air, new AirState(this) },
                { eStates.gStrike, new GStrike(this) },
                { eStates.gDefending, new gDefending(this) },
                { eStates.Dash, new DashState(this) },
                { eStates.Climb, new ClimbState(this) },
                { eStates.GParry, new ParryState(this) },
                { eStates.Staggered, new GStaggered(this) },
                { eStates.GSymphonCool, new GSymphonCool(this) },
                { eStates.GStrikeSpecialCool, new GStrikeSpecialCool(this) },
                // We add a default HookState with a placeholder target.
                { eStates.Hook, new HookState(this, Vector2.Zero) }
            };




            currentState = stateMap[eStates.Idle];
            currentState.Enter();
        }

        public void ChangeState(eStates newState)
        {
            currentState?.Exit();
            if (stateMap.TryGetValue(newState, out var nextState))
            {
                currentState = nextState;
                currentState.Enter();
                GD.Print("State changed to: " + newState);
            }
            else
            {
                GD.PrintErr("State not found: " + newState);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            currentState?.Update(delta);
            MoveAndSlide();
            Stamina = Math.Min(Stamina + 1 * delta, 100f);
        }
    }

    // --- State Implementations ---

    public class IdleState : IState.IState
    {
        private readonly States owner;
        public IdleState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Idle State");
        public void Update(float delta)
        {
            if (Input.IsActionPressed("ui_right") || Input.IsActionPressed("ui_left"))
                owner.ChangeState(eStates.Moving);
        }
        public void Exit() => GD.Print("Exiting Idle State");
        public void InAirState() => owner.ChangeState(eStates.Attacking);
        public Motif GetMotif() => null;
    }

    public class MovingState : IState.IState
    {
        private readonly States owner;
        public MovingState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Moving State");
        public void Update(float delta)
        {
            if (!Input.IsActionPressed("ui_right") && !Input.IsActionPressed("ui_left"))
                owner.ChangeState(eStates.Idle);
        }
        public void Exit() => GD.Print("Exiting Moving State");
        public void InAirState() => owner.ChangeState(eStates.Hook);
        public Motif GetMotif() => null;
    }

    public class AttackingState : IState.IState
    {
        private readonly States owner;
        public AttackingState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Attacking State");
        public void Update(float delta)
        {
            // For demonstration, we get a motif and log its use.
            Motif motif = GetMotif();
            if (motif != null)
                GD.Print("Attacking with motif: " + motif.Name);
        }
        public void Exit() => GD.Print("Exiting Attacking State");
        public void InAirState() => owner.ChangeState(eStates.Hook);
        public Motif GetMotif() => new Motif(1, "Basic Attack", 10, 1.0f);
    }

    public class HookState : IState.IState
    {
        private readonly States owner;
        private Vector2 hookTarget;
        public HookState(States owner, Vector2 hookTarget)
        {
            this.owner = owner;
            this.hookTarget = hookTarget;
        }
        public void Enter() => GD.Print("Entering Hook State");
        public void Update(float delta)
        {
            Vector2 direction = (hookTarget - owner.Position).Normalized();
            owner.Velocity = direction * HookPullSpeed;

            if (owner.Position.DistanceTo(hookTarget) < 10f)
                owner.ChangeState(eStates.Idle);
        }
        public void Exit() => GD.Print("Exiting Hook State");
        public void InAirState() => owner.ChangeState(eStates.Idle);
        public Motif GetMotif() => null;
    }
}
