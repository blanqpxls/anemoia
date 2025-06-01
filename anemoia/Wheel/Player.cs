using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] public float MaxRun = 300.0f;
    [Export] public float JumpVelocity = -900.0f;
    [Export] public float AttackDamage = 10f;	
    [Export] public float AttCooldown = 0.5f;
    [Export] public float Health = 500.0f;
    [Export] public int SymphonProfundity = 25;
    [Export] public int MaxJumps = 2; // Maximum number of jumps
    [Export] public float Acceleration = 130.0f;
    [Export] public float Deceleration = 130.0f; 

    private int jumpCount = 0;
    private bool isJumping = false;

    // Adjusted to match the constructor of Target.Belligerent
    Target.Belligerent MyPlayer = new Target.Belligerent(
        "Emilia", // Name
        1,         // ID
        13,        // Type
        7,         // Level
        600.0f,    // Health
        10.0f,     // AttackDamage
        300.0f,    // Speed
        50         // Range
    );

    public static Player Instance { get; private set; }

    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Attacking
    }

    public PlayerState currentState = PlayerState.Idle;

    public override void _Ready()
    {
        Instance = this;
        // Initialize player-specific logic
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                HandleIdleState();
                break;
            case PlayerState.Running:
                HandleRunningState();
                break;
            case PlayerState.Jumping:
                HandleJumpingState();
                break;
            case PlayerState.Attacking:
                HandleAttackingState();
                break;
        }
    }

    private void HandleIdleState()
    {
        // Implement idle state logic here
    }

    private void HandleRunningState()
    {
        // Implement running state logic here
    }

    private void HandleJumpingState()
    {
        // Implement jumping state logic here
    }

    private void HandleAttackingState()
    {
        // Implement attacking state logic here
    }

    private void Jump()
    {
        if (jumpCount < MaxJumps)
        {
            Velocity = new Vector2(Velocity.X, JumpVelocity);
            jumpCount++;
            isJumping = true;
            GD.Print("Player jumped");
        }
    }

    private void Land()
    {
        jumpCount = 0;
        isJumping = false;
        GD.Print("Player landed");
    }

    public void OnStateChanged(Engine.eStates newState)
    {
        GD.Print("Player notified of state change: " + newState);
        switch (newState)
        {
            case Engine.eStates.Idle:
                currentState = PlayerState.Idle;
                Land(); // Reset jump count when transitioning to Idle
                break;
            case Engine.eStates.Moving:
                currentState = PlayerState.Running;
                break;
            case Engine.eStates.Jumping:
                currentState = PlayerState.Jumping;
                Jump(); // Trigger jump logic
                break;
            case Engine.eStates.Attacking:
                currentState = PlayerState.Attacking;
                break;
            default:
                GD.Print($"Unhandled state transition: {newState}");
                break;
        }
    }
}