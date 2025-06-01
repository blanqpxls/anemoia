using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Godot;




namespace Target { public partial class Belligerent : CharacterBody2D, IState.IState


{


     public Belligerent(String name, int id, int type, int level, float health, float damage, float speed, int range)
    {
        Id = id;
        Level = level;
        Title = name;
        this.AttackDamage = damage;
        this.Grounded = true;
        this.hype= type;
        this.Level = level;
        this.Health = health;
    }

    [Export] public float MaxRun = 300.0f;
    [Export] public int id;
    [Export] public float JumpVelocity = -900.0f;
    [Export] public float AttackDamage = 10f;

        public bool Grounded { get; private set; }
        public int hype { get; private set; }

        [Export] public float AttCooldown = 0.5f;
    [Export] public float Health = 500.0f;
    [Export] public int SymphonProfundity = 25;
    [Export] public int MaxJumps = 2; // Maximum number of jumps
    [Export] public float Acceleration = 130.0f;
    [Export] public float Deceleration = 130.0f; 

    private int jumpCount = 0;
    private bool isJumping = false;

    public static Belligerent Instance { get; private set; }
        public int Id { get; private set; }
        public int Level { get; private set; }
        public string Title { get; private set; }

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
        // Initialize belligerent-specific logic
    }   
}

