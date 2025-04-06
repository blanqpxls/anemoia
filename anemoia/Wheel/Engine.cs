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
        gStrike,
        SymphonCoolState,
    }
}


