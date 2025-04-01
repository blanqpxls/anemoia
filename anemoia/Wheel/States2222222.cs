using Godot;
using System;
using System.Collections.Generic;
using Engine;
using Engine.States; // Ensure this namespace is included for IState
using Engine.IState; // Ensure this namespace is included for IState
using Engine.States; // Ensure this namespace is included for state classes like InAirState
using Engine; // Ensure this namespace is included for eStates


namespace IState // Changed from Current.InstanceStates to IState
{
    public interface IState
    {
        void Enter();
        void Update(float delta);
        void Exit();
        void CurrentState(); // Ensure this method exists in all implementations
        void InAirState();   // Ensure this method exists in all implementations
    }
}

public partial class NestedStates : CharacterBody2D
{


    public void ChangeState(Engine.eStates newState)
    {
        statesInstance.ChangeState(newState);
    }
}

public partial class States : CharacterBody2D
{
    public float Stamina { get; set; } = 100f;

    private IState; // Ensure currentState is defined
    private Dictionary<Engine.eStates, IState.IState> stateMap;

    public States()
    {
        stateMap = new Dictionary<Engine.eStates, IState>
        {
            { Engine.eStates.Idle, new IdleState(this) },
            { Engine.eStates.Moving, new MovingState(this) },
            { Engine.eStates.Attacking, new AttackingState(this) },
            { Engine.eStates.Retired, new DeadState(this) },
            { Engine.eStates.Air, new AirState(this) },
            { Engine.eStates.gStrike, new GStrike(this) },
            { Engine.eStates.gDefending, new gDefending(this) },
            { Engine.eStates.Dash, new DashState(this) },
            { Engine.eStates.Climb, new ClimbState(this) },
            { Engine.eStates.GParry, new ParryState(this) },
            { Engine.eStates.Staggered, new GStaggered(this) },
            { Engine.eStates.GSymphonCool, new GSymphonCool(this) },
            { Engine.eStates.GStrikeSpecialCool, new GStrikeSpecialCool(this) } // Added missing state
        };
    }

    public void ChangeState(Engine.eStates newState)
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

    public override void _PhysicsProcess(double delta)
    {
        currentState?.Update((float)delta);
        Velocity = MoveAndSlide(); // Apply the movement
        Stamina = Math.Min(Stamina + 1 * (float)delta, 100f); // Regenerate stamina
    }
}

public class AttackingState : IState.IState
{
    private readonly States owner;

    public AttackingState(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering Attacking State");
    }

    public void Update(float delta)
    {
        // Attacking logic
    }

    public void Exit()
    {
        GD.Print("Exiting Attacking State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is AttackingState");
    }

    public void InAirState()
    {
        GD.Print("Transitioning to InAirState from AttackingState");
        owner.ChangeState(Engine.eStates.Air);
    }
}

public class DeadState : IState.IState
{
    private readonly States owner;

    public DeadState(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering Retired State");
        owner.Velocity = Vector2.Zero; // Stop all movement
    }

    public void Update(float delta)
    {
        // Dead state logic, e.g., wait for respawn
    }

    public void Exit()
    {
        GD.Print("Exiting Retired State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is DeadState");
    }

    public void InAirState()
    {
        GD.Print("Transitioning to InAirState from DeadState");
        owner.ChangeState(Engine.eStates.Air);
    }
}

public class AirState : IState.IState
{
    private readonly States owner;

    public AirState(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering Air State");
    }

    public void Update(float delta)
    {
        owner.Velocity.y += Gravity * delta;

        if (owner.Input.IsActionPressed("ui_right"))
        {
            owner.Velocity.x = Math.Min(owner.Velocity.x + AirMult * RunAccel * delta, MaxRun);
        }
        else if (owner.Input.IsActionPressed("ui_left"))
        {
            owner.Velocity.x = Math.Max(owner.Velocity.x - AirMult * RunAccel * delta, -MaxRun);
        }

        if (owner.IsOnFloor())
        {
            owner.ChangeState(Engine.eStates.Idle);
        }
        else if (owner.Input.IsActionJustPressed("ui_jump"))
        {
            owner.ChangeState(Engine.eStates.InAir); // Transition to InAirState
        }
    }

    public void Exit()
    {
        GD.Print("Exiting Air State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is AirState");
    }

    public void InAirState()
    {
        GD.Print("Already in AirState");
    }
}

public class InAirState : IState.IState
{
    private readonly States owner;

    public InAirState(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering In-Air State");
    }

    public void Update(float delta)
    {
        owner.Velocity.y += Gravity * delta;

        if (owner.Input.IsActionPressed("ui_right"))
        {
            owner.Velocity.x = Math.Min(owner.Velocity.x + AirMult * RunAccel * delta, MaxRun);
        }
        else if (owner.Input.IsActionPressed("ui_left"))
        {
            owner.Velocity.x = Math.Max(owner.Velocity.x - AirMult * RunAccel * delta, -MaxRun);
        }

        if (owner.IsOnFloor())
        {
            owner.ChangeState(Engine.eStates.Idle);
        }
    }

    public void Exit()
    {
        GD.Print("Exiting In-Air State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is InAirState");
    }

    public void InAirState()
    {
        GD.Print("Already in InAirState");
    }
}

public class GStrike : IState.IState
{
    private readonly States owner;

    public GStrike(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering Ground Attacking State");
    }

    public void Update(float delta)
    {
        // Ground attacking logic
    }

    public void Exit()
    {
        GD.Print("Exiting Ground Attacking State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is GStrike");
    }

    public void InAirState()
    {
        GD.Print("Transitioning to InAirState from GStrike");
        owner.ChangeState(Engine.eStates.Air);
    }
}

public class gDefending : IState.IState
{
    private readonly States owner;

  

    public void Enter()
    {
        GD.Print("Entering Ground Defending State");
    }

    public void Update(float delta)
    {
        // Ground defending logic
    }

    public void Exit()
    {
        GD.Print("Exiting Ground Defending State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is gDefending");
    }

    public void InAirState()
    {
        GD.Print("Transitioning to InAirState from gDefending");
        owner.ChangeState(Engine.eStates.Air);
    }
}

public class DashState : IState.IState
{
    private readonly States owner;
    private float dashTimer;

    public DashState(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering Dash State");
        dashTimer = DashTime;

        Vector2 dashDirection = new Vector2(
            owner.Input.IsActionPressed("ui_right") ? 1 : owner.Input.IsActionPressed("ui_left") ? -1 : 0,
            owner.Input.IsActionPressed("ui_down") ? 1 : owner.Input.IsActionPressed("ui_up") ? -1 : 0
        ).Normalized();

        if (dashDirection == Vector2.Zero)
        {
            dashDirection = new Vector2(Math.Sign(owner.Velocity.X), 0);

            if (dashDirection == Vector2.Zero)
            {
                owner.ChangeState(Engine.eStates.Idle);
                return;
            }
        }

        owner.Velocity = dashDirection * DashSpeed;
    }

    public void Update(float delta)
    {
        dashTimer -= delta;
        if (dashTimer <= 0)
        {
            owner.ChangeState(Engine.eStates.Idle);
        }
    }

    public void Exit()
    {
        GD.Print("Exiting Dash State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is DashState");
    }

    public void InAirState()
    {
        GD.Print("Transitioning to InAirState from DashState");
        owner.ChangeState(Engine.eStates.Air);
    }
}

public class ClimbState : IState.IState
{
    private readonly States owner;

    public ClimbState(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering Climb State");
    }

    public void Update(float delta)
    {
        if (owner.Input.IsActionPressed("ui_up"))
        {
            owner.Velocity.y = Math.Max(owner.Velocity.y - ClimbAccel * delta, ClimbUpSpeed);
            owner.Stamina -= 1 * delta; // Consume stamina
        }
        else if (owner.Input.IsActionPressed("ui_down"))
        {
            owner.Velocity.y = Math.Min(owner.Velocity.y + ClimbAccel * delta, ClimbDownSpeed);
            owner.Stamina -= 1 * delta; // Consume stamina
        }
        else
        {
            owner.Velocity.y = Mathf.Lerp(owner.Velocity.y, 0, ClimbSlipSpeed * delta);
        }

        if (owner.Stamina <= 0)
        {
            owner.ChangeState(Engine.eStates.Air);
        }
    }

    public void Exit()
    {
        GD.Print("Exiting Climb State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is ClimbState");
    }

    public void InAirState()
    {
        GD.Print("Transitioning to InAirState from ClimbState");
        owner.ChangeState(Engine.eStates.Air);
    }
}

public class ParryState : IState.IState
{
    private readonly States owner;

    public ParryState(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering Parry State");
    }

    public void Update(float delta)
    {
        // Parry logic
    }

    public void Exit()
    {
        GD.Print("Exiting Parry State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is ParryState");
    }

    public void InAirState()
    {
        GD.Print("Transitioning to InAirState from ParryState");
        owner.ChangeState(Engine.eStates.Air);
    }
}

public class GStaggered : IState.IState
{
    private readonly States owner;

    public GStaggered(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering Staggered State");
    }

    public void Update(float delta)
    {
        // Staggered logic
    }

    public void Exit()
    {
        GD.Print("Exiting Staggered State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is GStaggered");
    }

    public void InAirState()
    {
        GD.Print("Transitioning to InAirState from GStaggered");
        owner.ChangeState(Engine.eStates.Air);
    }
}

public class GSymphonCool : IState.IState
{
    private readonly States owner;

    public GSymphonCool(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering GSymphon Cooldown State");
    }

    public void Update(float delta)
    {
        // Cooldown logic
    }

    public void Exit()
    {
        GD.Print("Exiting GSymphon Cooldown State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is GSymphonCool");
    }

    public void InAirState()
    {
        GD.Print("Transitioning to InAirState from GSymphonCool");
        owner.ChangeState(Engine.eStates.Air);
    }
}

public class GStrikeSpecialCool : IState.IState
{
    private readonly States owner;

    public GStrikeSpecialCool(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering GStrike Special Cooldown State");
    }

    public void Update(float delta)
    {
        // Cooldown logic
    }

    public void Exit()
    {
        GD.Print("Exiting GStrike Special Cooldown State");
    }

    public void CurrentState()
    {
        GD.Print("Current state is GStrikeSpecialCool");
    }

    public void InAirState()
    {
        GD.Print("Transitioning to InAirState from GStrikeSpecialCool");
        owner.ChangeState(Engine.eStates.Air);
    }
}
