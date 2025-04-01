using Godot;
using System;
using Current.InstanceStates; // Updated namespace

public partial class GStrike : Node, IState
{
    private readonly States owner;
    private int strikePower;

    public GStrike(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering GStrike State");
        // Add logic for entering the GStrike state
    }

    public void Update(float delta)
    {
        // Add logic for updating the GStrike state
    }

    public void Exit()
    {
        GD.Print("Exiting GStrike State");
        // Add logic for exiting the GStrike state
    }

    public void PerformStrike(Player player)
    {
        GD.Print("Player performed a strike with power: " + strikePower);
        // Add more detailed logic here
    }

    public void SetStrikePower(int power)
    {
        strikePower = power;
    }
}
