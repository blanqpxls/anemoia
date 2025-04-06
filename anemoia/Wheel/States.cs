using Godot;
using System;
using System.Collections.Generic;
using Engine.eStates;

namespace Engine.eStates
{
    public enum eStates
    {
        Idle,
        Moving,
        Attacking,
        Hook,
        Retired,
        Air,
        gStrike,
        gDefending,
        Dash,
        Climb,
        GParry,
        Staggered,
        GSymphonCool,
        GStrikeSpecialCool,

        GeStaggered,
        HookeState,
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
            { eStates.Air, new InAirState(this) },
            { eStates.gStrike, new GStrike(this) },
            { eStates.gDefending, new GDefending(this) },
            { eStates.Dash, new DashState(this) },
            { eStates.Climb, new ClimbState(this) },
            { eStates.GParry, new ParryState(this) },
            { eStates.Staggered, new GeStaggered(this) },
            { eStates.GSymphonCool, new SymphonCoolState(this) },
            { eStates.GStrikeSpecialCool, new GStrikeSpecialCool(this) },
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


    public class DeadState : IState.IState
    {
        private readonly States owner;
        public DeadState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Dead State");
        public void Update(float delta) { /* Handle death logic */ }
        public void Exit() => GD.Print("Exiting Dead State");
        public void InAirState() => owner.ChangeState(eStates.Idle);
        public Motif GetMotif() => null;
    }
    public class GStrike : IState.IState
    {
        private readonly States owner;
        public GStrike(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering GStrike State");
        public void Update(float delta) { /* Handle GStrike logic */ }
        public void Exit() => GD.Print("Exiting GStrike State");
        public void InAirState() => owner.ChangeState(eStates.Idle);
        public Motif GetMotif() => null;
    }

    public class GDefending : IState.IState

    {
        private readonly States owner;
        public GDefending(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering gDefending State");
        public void Update(float delta) { /* Handle gDefending logic */ }
        public void Exit() => GD.Print("Exiting gDefending State");
        public void InAirState() => owner.ChangeState(eStates.Idle);
        public Motif GetMotif() => null;


        public class RetiredeState : IState.IState
        {
            private readonly States owner;
            public RetiredeState(States owner) { this.owner = owner; }
            public void Enter() => GD.Print("Entering Retired State");
            public void Update(float delta) { /* Handle Retired logic */ }
            public void Exit() => GD.Print("Exiting Retired State");
            public void InAirState() => owner.ChangeState(eStates.Idle);
            public Motif GetMotif() => null;

            public class DasheState : IState.IState
            {
                private readonly States owner;
                public DasheState(States owner) { this.owner = owner; }
                public void Enter() => GD.Print("Entering Dash State");
                public void Update(float delta) { /* Handle Dash logic */ }
                public void Exit() => GD.Print("Exiting Dash State");
                public void InAirState() => owner.ChangeState(eStates.Idle);
                public Motif GetMotif() => null;
            }
            public class ClimbeState : IState.IState
            {
                private readonly States owner;
                public ClimbeState(States owner) { this.owner = owner; }
                public void Enter() => GD.Print("Entering Climb State");
                public void Update(float delta) { /* Handle Climb logic */ }
                public void Exit() => GD.Print("Exiting Climb State");
                public void InAirState() => owner.ChangeState(eStates.Idle);
                public Motif GetMotif() => null;
            }
            public class eParryState : IState.IState
            {
                private readonly States owner;
                public eParryState(States owner) { this.owner = owner; }
                public void Enter() => GD.Print("Entering Parry State");
                public void Update(float delta) { /* Handle Parry logic */ }
                public void Exit() => GD.Print("Exiting Parry State");
                public void InAirState() => owner.ChangeState(eStates.Idle);
                public Motif GetMotif() => null;
            }

       
            public class SymphonCoolState : IState.IState
            {
                private readonly States owner;
                public SymphonCoolState(States owner) { this.owner = owner; }
                public void Enter() => GD.Print("Entering Symphon Cool State");
                public void Update(float delta) { /* Handle Symphon Cool logic */ }
                public void Exit() => GD.Print("Exiting Symphon Cool State");
                public void InAirState() => owner.ChangeState(eStates.Idle);
                public Motif GetMotif() => null;
            }
         
            public class HookeState : IState.IState
            {
                private readonly States owner;
                private Vector2 hookTarget;
                public HookeState(States owner, Vector2 hookTarget)
                {
                    this.owner = owner;
                    this.hookTarget = hookTarget;
                }
                public void Enter() => GD.Print("Entering Hook State");
                public void Update(float delta)
                {
                    Vector2 direction = (hookTarget - owner.Position).Normalized();
                    owner.Velocity = direction * States.HookPullSpeed;

                    if (owner.Position.DistanceTo(hookTarget) < 10f)
                        owner.ChangeState(eStates.Idle);
                }
                public void Exit() => GD.Print("Exiting Hook State");
                public void InAirState() => owner.ChangeState(eStates.Idle);
                public Motif GetMotif() => null;
            }
            public class DashState : IState.IState
            {
                private readonly States owner;
                public DasheState(States owner) { this.owner = owner; }
                public void Enter() => GD.Print("Entering Dash State");
                public void Update(float delta) { /* Handle Dash logic */ }
                public void Exit() => GD.Print("Exiting Dash State");
                public void InAirState() => owner.ChangeState(eStates.Idle);
                public Motif GetMotif() => null;
            }


            public class ClimbrState : IState.IState
            {
                private readonly States owner;
                public ClimbrState(States owner) { this.owner = owner; }
                public void Enter() => GD.Print("Entering Climb State");
                public void Update(float delta) { /* Handle Climb logic */ }
                public void Exit() => GD.Print("Exiting Climb State");
                public void InAirState() => owner.ChangeState(eStates.Idle);
                public Motif GetMotif() => null;
            }
            public override GeStaggered : IState.IState
            {
                private readonly States owner;
                public GeStaggered(States owner) { this.owner = owner; }
                public void Enter() => GD.Print("Entering Staggered State");
                public void Update(float delta) { /* Handle Staggered logic */ }
                public void Exit() => GD.Print("Exiting Staggered State");
                public void InAirState() => owner.ChangeState(eStates.Idle);
                public Motif GetMotif() => null;
            }

            public class GStrikeSpecialCool : IState.IState
            {
                private readonly States owner;
                public GStrikeSpecialCool(States owner) { this.owner = owner; }
                public void Enter() => GD.Print("Entering GStrike Special Cool State");
                public void Update(float delta) { /* Handle GStrike Special Cool logic */ }
                public void Exit() => GD.Print("Exiting GStrike Special Cool State");
                public void InAirState() => owner.ChangeState(eStates.Idle);
                public Motif GetMotif() => null;
            }

        }
public class GeStaggered : IState.IState
{
    private readonly States owner;
    public GeStaggered(States owner) { this.owner = owner; }
    public void Enter() => GD.Print("Entering Staggered State");
    public void Update(float delta) { /* Handle Staggered logic */ }
    public void Exit() => GD.Print("Exiting Staggered State");
    public void InAirState() => owner.ChangeState(eStates.Idle);
    public Motif GetMotif() => null;
}
    }
 }
