using Godot;
using System;
using System.Collections.Generic;

namespace Engine.States
{
    // Updated enum with a new Hook state.
    public enum eStates
    {
        Idle,
        Moving,
        Attacking,
        Retired,
        Air,
        InAir,
        gStrike,
        gDefending,
        Dash,
        Climb,
        GParry,
        Staggered,
        GSymphonCool,
        GStrikeSpecialCool,
        Hook  // New hook state
    }

    // InputCommand struct for all possible input actions
    public struct InputCommand
    {
        public bool MoveLeft;
        public bool MoveRight;
        public bool MoveUp;
        public bool MoveDown;
        public bool Jump;
        public bool Attack;
        public bool Defend;
        public bool Dash;
        public bool Climb;
        public bool Parry;
        public bool Hook;
        // Add more as needed
        public static InputCommand None => new InputCommand();
    }

    // StateOutput struct for output (expand as needed)
    public struct StateOutput
    {
        public bool WantsToMove;
        public bool WantsToAttack;
        public bool WantsToJump;
        public bool WantsToDefend;
        public bool WantsToDash;
        public bool WantsToClimb;
        public bool WantsToParry;
        public bool WantsToHook;
        // Add more as needed
    }

    // Global state table for all actors
    public static class IStateTable
    {
        public static Dictionary<string, StateOutput> Table = new Dictionary<string, StateOutput>();
    }

    // The IState interface for all state classes.
    public interface IState
    {
        void Enter();
        void Update(float delta, InputCommand input);
        void Exit();
        void CurrentState();
        void InAirState();
    }

    // The main state machine class.
    public partial class States : CharacterBody2D
    {
        // Unique identifier for this actor
        public string ScriptKey { get; set; }
        // The current input for this actor
        public InputCommand CurrentInput { get; set; } = InputCommand.None;

        public float Stamina { get; set; } = 100f;
        private IState currentState;
        private Dictionary<eStates, IState> stateMap;

        // Constants for state behavior.
        public const float Gravity = 9.8f;
        public const float RunAccel = 10f;
        public const float AirMult = 0.8f;
        public const float MaxRun = 100f;
        public const float DashTime = 0.5f;
        public const float DashSpeed = 300f;
        public const float ClimbAccel = 5f;
        public const float ClimbUpSpeed = -50f;
        public const float ClimbDownSpeed = 50f;
        public const float ClimbSlipSpeed = 0.1f;

        public const float GStrike = 10f; // Placeholder for GStrike damage

        // Added a hook-pull speed constant.
        public const float HookPullSpeed = 400f;

        public float Speed { get; set; } = 300f; // Add default speed for movement

        public override void _Ready()
        {
            stateMap = new Dictionary<eStates, IState>
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

            currentState = stateMap[eStates.Idle]; // Default to Idle
            currentState.Enter();

            // Assign a unique ScriptKey here (engine/theatre should set this properly)
            if (string.IsNullOrEmpty(ScriptKey))
                ScriptKey = Guid.NewGuid().ToString();
        }

        // 
        public void StartHookState(Vector2 hookTarget)
        {
            // Create a new instance of HookState with the provided target.
            stateMap[eStates.Hook] = new HookState(this, hookTarget);
            ChangeState(eStates.Hook);
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

        // Called by the game loop/engine to update input for this actor
        public void FeedInputFromStateTable()
        {
            if (!string.IsNullOrEmpty(ScriptKey) && IStateTable.Table.TryGetValue(ScriptKey, out var output))
            {
                // Convert StateOutput to InputCommand (simple mapping for now)
                CurrentInput = new InputCommand
                {
                    MoveLeft = output.WantsToMove && false, // Example: set logic for direction
                    MoveRight = output.WantsToMove && true, // Example: set logic for direction
                    Jump = output.WantsToJump,
                    Attack = output.WantsToAttack,
                    Defend = output.WantsToDefend,
                    Dash = output.WantsToDash,
                    Climb = output.WantsToClimb,
                    Parry = output.WantsToParry,
                    Hook = output.WantsToHook
                };
            }
            else
            {
                CurrentInput = InputCommand.None;
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            // Fallback: If no input is set, use InputCommand.None
            var input = CurrentInput;
            currentState?.Update((float)delta, input);
            MoveAndSlide();
            Stamina = Math.Min(Stamina + 1 * (float)delta, 100f);
        }
    }

    // --- State Implementations ---
    public class IdleState : IState
    {
        private readonly States owner;
        public IdleState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Idle State");
        public void Update(float delta, InputCommand input)
        {
            if (input.MoveRight || input.MoveLeft)
                owner.ChangeState(eStates.Moving);
        }
        public void Exit() => GD.Print("Exiting Idle State");
        public void CurrentState() => GD.Print("Current state is IdleState");
        public void InAirState() => owner.ChangeState(eStates.Air);
    }

    public class MovingState : IState
    {
        private readonly States owner;
        public MovingState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Moving State");
        public void Update(float delta, InputCommand input)
        {
            if (!input.MoveRight && !input.MoveLeft)
                owner.ChangeState(eStates.Idle);
            // Example movement logic
            if (input.MoveRight)
                owner.Velocity = new Vector2(owner.Speed, owner.Velocity.Y);
            else if (input.MoveLeft)
                owner.Velocity = new Vector2(-owner.Speed, owner.Velocity.Y);
        }
        public void Exit() => GD.Print("Exiting Moving State");
        public void CurrentState() => GD.Print("Current state is MovingState");
        public void InAirState() => owner.ChangeState(eStates.Air);
    }

    public class AttackingState : IState
    {
        private readonly States owner;
        public AttackingState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Attacking State");
        public void Update(float delta, InputCommand input) { /* Attacking logic here */ }
        public void Exit() => GD.Print("Exiting Attacking State");
        public void CurrentState() => GD.Print("Current state is AttackingState");
        public void InAirState() => owner.ChangeState(eStates.Air);
    }

    public class DeadState : IState
    {
        private readonly States owner;
        public DeadState(States owner) { this.owner = owner; }
        public void Enter()
        {
            GD.Print("Entering Retired State");
            owner.Velocity = Vector2.Zero;
        }
        public void Update(float delta, InputCommand input) { /* Dead state logic */ }
        public void Exit() => GD.Print("Exiting Retired State");
        public void CurrentState() => GD.Print("Current state is DeadState");
        public void InAirState() => owner.ChangeState(eStates.Air);
    }

    public class AirState : IState
    {
        private readonly States owner;
        public AirState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Air State");
        public void Update(float delta, InputCommand input)
        {
            owner.Velocity = new Vector2(
                owner.Velocity.X,
                owner.Velocity.Y + States.Gravity * delta
            );
            if (input.MoveRight)
            {
                owner.Velocity = new Vector2(
                    Math.Min(owner.Velocity.X + States.AirMult * States.RunAccel * delta, States.MaxRun),
                    owner.Velocity.Y
                );
            }
            else if (input.MoveLeft)
            {
                owner.Velocity = new Vector2(
                    Math.Max(owner.Velocity.X - States.AirMult * States.RunAccel * delta, -States.MaxRun),
                    owner.Velocity.Y
                );
            }
            if (owner.IsOnFloor())
                owner.ChangeState(eStates.Idle);
            else if (input.Jump)
                owner.ChangeState(eStates.InAir);
        }
        public void Exit() => GD.Print("Exiting Air State");
        public void CurrentState() => GD.Print("Current state is AirState");
        public void InAirState() => GD.Print("Already in InAirState");
    }

    public class InAirState : IState
    {
        private readonly States owner;
        public InAirState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering In-Air State");
        public void Update(float delta, InputCommand input)
        {
            owner.Velocity = new Vector2(
                owner.Velocity.X,
                owner.Velocity.Y + States.Gravity * delta
            );
            if (input.MoveRight)
            {
                owner.Velocity = new Vector2(
                    Math.Min(owner.Velocity.X + States.AirMult * States.RunAccel * delta, States.MaxRun),
                    owner.Velocity.Y
                );
            }
            else if (input.MoveLeft)
            {
                owner.Velocity = new Vector2(
                    Math.Max(owner.Velocity.X - States.AirMult * States.RunAccel * delta, -States.MaxRun),
                    owner.Velocity.Y
                );
            }
            if (owner.IsOnFloor())
                owner.ChangeState(eStates.Idle);
        }
        public void Exit() => GD.Print("Exiting In-Air State");
        public void CurrentState() => GD.Print("Current state is InAirState");
        void IState.InAirState() => GD.Print("Already in InAirState");
    }

    public class GStrike : IState
    {
        private readonly States owner;
        public GStrike(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Ground Attacking State");
        public void Update(float delta, InputCommand input) { /* GStrike logic */ }
        public void Exit() => GD.Print("Exiting Ground Attacking State");
        public void CurrentState() => GD.Print("Current state is GStrike");
        public void InAirState() { GD.Print("Transitioning to InAirState from GStrike"); owner.ChangeState(eStates.Air); }
    }

    public class gDefending : IState
    {
        private readonly States owner;
        public gDefending(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Ground Defending State");
        public void Update(float delta, InputCommand input) { /* GDefending logic */ }
        public void Exit() => GD.Print("Exiting Ground Defending State");
        public void CurrentState() => GD.Print("Current state is gDefending");
        public void InAirState() { GD.Print("Transitioning to InAirState from gDefending"); owner.ChangeState(eStates.Air); }
    }

    public class DashState : IState
    {
        private readonly States owner;
        private float dashTimer;
        public DashState(States owner) { this.owner = owner; }
        public void Enter()
        {
            GD.Print("Entering Dash State");
            dashTimer = States.DashTime;
            // Dash logic can use input if needed
        }
        public void Update(float delta, InputCommand input)
        {
            dashTimer -= delta;
            if (dashTimer <= 0)
                owner.ChangeState(eStates.Idle);
        }
        public void Exit() => GD.Print("Exiting Dash State");
        public void CurrentState() => GD.Print("Current state is DashState");
        public void InAirState() { GD.Print("Transitioning to InAirState from DashState"); owner.ChangeState(eStates.Air); }
    }

    public class ClimbState : IState
    {
        private readonly States owner;
        public ClimbState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Climb State");
        public void Update(float delta, InputCommand input)
        {
            if (input.MoveUp)
            {
                owner.Velocity = new Vector2(
                    owner.Velocity.X,
                    Math.Max(owner.Velocity.Y - States.ClimbAccel * delta, States.ClimbUpSpeed)
                );
                owner.Stamina -= 1 * delta;
            }
            else if (input.MoveDown)
            {
                owner.Velocity = new Vector2(
                    owner.Velocity.X,
                    Math.Min(owner.Velocity.Y + States.ClimbAccel * delta, States.ClimbDownSpeed)
                );
                owner.Stamina -= 1 * delta;
            }
            else
            {
                owner.Velocity = new Vector2(
                    owner.Velocity.X,
                    Mathf.Lerp(owner.Velocity.Y, 0, States.ClimbSlipSpeed * delta)
                );
            }
            if (owner.Stamina <= 0)
                owner.ChangeState(eStates.Air);
        }
        public void Exit() => GD.Print("Exiting Climb State");
        public void CurrentState() => GD.Print("Current state is ClimbState");
        public void InAirState() { GD.Print("Transitioning to InAirState from ClimbState"); owner.ChangeState(eStates.Air); }
    }

    public class ParryState : IState
    {
        private readonly States owner;
        public ParryState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Parry State");
        public void Update(float delta, InputCommand input) { /* Parry logic */ }
        public void Exit() => GD.Print("Exiting Parry State");
        public void CurrentState() => GD.Print("Current state is ParryState");
        public void InAirState() { GD.Print("Transitioning to InAirState from ParryState"); owner.ChangeState(eStates.Air); }
    }

    public class GStaggered : IState
    {
        private readonly States owner;
        public GStaggered(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Staggered State");
        public void Update(float delta, InputCommand input) { /* Staggered logic */ }
        public void Exit() => GD.Print("Exiting Staggered State");
        public void CurrentState() => GD.Print("Current state is Staggered");
        public void InAirState() { GD.Print("Transitioning to InAirState from GStaggered"); owner.ChangeState(eStates.Air); }
    }

    public class GSymphonCool : IState
    {
        private readonly States owner;
        public GSymphonCool(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering GSymphon Cooldown State");
        public void Update(float delta, InputCommand input) { /* Cooldown logic */ }
        public void Exit() => GD.Print("Exiting GSymphon Cooldown State");
        public void CurrentState() => GD.Print("Current state is GSymphonCool");
        public void InAirState() { GD.Print("Transitioning to InAirState from GSymphonCool"); owner.ChangeState(eStates.Air); }
    }

    public class GStrikeSpecialCool : IState
    {
        private readonly States owner;
        public GStrikeSpecialCool(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering GStrike Special Cooldown State");
        public void Update(float delta, InputCommand input) { /* Cooldown logic */ }
        public void Exit() => GD.Print("Exiting GStrike Special Cooldown State");
        public void CurrentState() => GD.Print("Current state is GStrikeSpecialCool");
        public void InAirState() { GD.Print("Transitioning to InAirState from GStrikeSpecialCool"); owner.ChangeState(eStates.Air); }
    }

    public class HookState : IState
    {
        private readonly States owner;
        private Vector2 hookTarget;
        private readonly float pullSpeed = States.HookPullSpeed;
        public HookState(States owner, Vector2 hookTarget)
        {
            this.owner = owner;
            this.hookTarget = hookTarget;
        }
        public void Enter()
        {
            GD.Print("Entering Hook State");
        }
        public void Update(float delta, InputCommand input)
        {
            Vector2 direction = (hookTarget - owner.Position).Normalized();
            owner.Velocity = direction * pullSpeed;
            if (owner.Position.DistanceTo(hookTarget) < 10f)
                owner.ChangeState(eStates.Idle);
        }
        public void Exit() => GD.Print("Exiting Hook State");
        public void CurrentState() => GD.Print("Current state is HookState");
        public void InAirState() => owner.ChangeState(eStates.Air);
    }
}
