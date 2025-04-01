using Godot;
using System;
using Current.InstanceStates; // Updated namespace

public partial class Air : Node, IState
{
    private readonly States owner;
    private int airSupply;
    private int maxAirSupply = 100;

    public Air(States owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        GD.Print("Entering Air State");
        // Add logic for entering the Air state
    }

    public void Update(float delta)
    {
        // Add logic for updating the Air state
        DecreaseAirSupply(1); // Example logic
    }

    public void Exit()
    {
        GD.Print("Exiting Air State");
        // Add logic for exiting the Air state
    }

    public void DecreaseAirSupply(int amount)
    {
        airSupply = Math.Max(airSupply - amount, 0);
        GD.Print("Air supply decreased: " + airSupply);
    }

    public void IncreaseAirSupply(int amount)
    {
        airSupply = Math.Min(airSupply + amount, maxAirSupply);
        GD.Print("Air supply increased: " + airSupply);
    }

    public int GetAirSupply()
    {
        return airSupply;
    }
}
