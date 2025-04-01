using Godot;
using NUnit.Framework;
using System.Collections.Generic;
using HerNostalgia.States;

[TestFixture]
public class StatesTest
{
    private States states;
    private MockWheel mockWheel;

    [SetUp]
    public void SetUp()
    {
        mockWheel = new MockWheel();
        states = new States(mockWheel);
    }

    [Test]
    public void TestInitialStateIsIdle()
    {
        Assert.AreEqual(Engine.eStates.Idle, mockWheel.LastState);
    }

    [Test]
    public void TestChangeStateToMoving()
    {
        states.ChangeState(Engine.eStates.Moving);
        Assert.AreEqual(Engine.eStates.Moving, mockWheel.LastState);
    }

    [Test]
    public void TestChangeStateToAir()
    {
        states.ChangeState(Engine.eStates.Air);
        Assert.AreEqual(Engine.eStates.Air, mockWheel.LastState);
    }

    [Test]
    public void TestInvalidStateChange()
    {
        states.ChangeState((Engine.eStates)999); // Invalid state
        Assert.AreNotEqual(999, mockWheel.LastState);
    }

    [Test]
    public void TestStateTransitionCallsEnterAndExit()
    {
        var mockState = new MockState();
        states.ChangeState(Engine.eStates.Idle); // Ensure initial state
        states.ChangeState(Engine.eStates.Moving);

        Assert.IsTrue(mockState.Entered);
        Assert.IsTrue(mockState.Exited);
    }

    private class MockWheel : Wheel
    {
        public Engine.eStates LastState { get; private set; }

        public override void OnStateChange(Engine.eStates state)
        {
            LastState = state;
        }
    }

    private class MockState : IState
    {
        public bool Entered { get; private set; }
        public bool Exited { get; private set; }

        public void Enter()
        {
            Entered = true;
        }

        public void Update(float delta) { }

        public void Exit()
        {
            Exited = true;
        }
    }
}

