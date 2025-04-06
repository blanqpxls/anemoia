using System;
using System.Collections.Generic;
using Godot;


namespace Engine
{
    /// <summary>
    /// Defines the various states that a character can be in.
    /// These states are used throughout the engine to determine behavior, animations, and logic.
    /// </summary>
  public enum eStates
{
    Idle,
    Moving,
    Attacking,
    Retired,
    Air,
    GStrike,
    GDefending,
    Dash,
    Climb,
    GParry,
    Staggered,
    GSymphonCool,
    GStrikeSpecialCool,
    Grappling, // <-- Add this if missing
        InAir,
        gDefending,
        gStrike
    }

    
  
    public partial class GameEngine : Node
    {
        public static GameEngine Instance { get; private set; }
        private List<eStates> states = new List<eStates>();

        public override void _Ready()
        {
            Instance = this;
            foreach (eStates state in Enum.GetValues(typeof(eStates)))
            {
                states.Add(state);
            }
        }

        public void NotifyStateChange(eStates newState)
        {
            GD.Print("Engine notified of state change: " + newState);
            Target.Instance?.OnStateChanged(newState);
        }

        // Other methods...
    }
}

