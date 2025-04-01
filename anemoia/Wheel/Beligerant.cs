using Godot;
using System;
using HerNostalgia.States;

public partial class Character : CharacterBody2D
{
    [Export] public float Speed = 300.0f;
    [Export] public float JumpVelocity = 600.0f;
    [Export] public float AttackDamage = 10f;
    [Export] public float AttCooldown = 0.5f;
    [Export] public float Health = 500.0f;
    [Export] public int SymphonProfundity = 25;

    private float _attackTimer = 0f;
    private States stateManager;

    public override void _Ready()
    {
        base._Ready();
        stateManager = GetNode<States>("States"); // Ensure the States node is properly set up
    }

    public override void _PhysicsProcess(double delta)
    {
        _attackTimer -= (float)delta;
        stateManager._PhysicsProcess(delta); // Delegate processing to the state manager
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        GD.Print($"{Name} took {damage} damage! Health: {Health}");

        if (Health <= 0)
        {
            stateManager.ChangeState(Engine.eStates.Dead); // Transition to Dead state
        }
        else
        {
            stateManager.ChangeState(Engine.eStates.Staggered); // Transition to Staggered state
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
        stateManager.ChangeState(Engine.eStates.Attacking); // Transition to Attacking state
    }

    public virtual void UseAbility(string abilityName)
    {
        GD.Print($"{Name} used {abilityName}!");
        // Add logic for transitioning to specific ability states if needed
    }

    public void OnStateChanged(Engine.eStates newState)
    {
        GD.Print($"{Name} transitioned to state: {newState}");
        // Handle any additional logic when the state changes
    }
}
