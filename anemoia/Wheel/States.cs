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
        Hook, // New hook state
        inRefrain,
    }

    // The IState interface for all state classes.
    public interface IState
    {
        void Enter();
        void Update(float delta);
        void Exit();
        void CurrentState();
        void InAirState();

        
    }

    // The main state machine class.
    public partial class States : CharacterBody2D
    {
        public float Stamina { get; set; } = 100f;
        private IState currentState;
        private Dictionary<eStates, IState> stateMap;
        private Archi archi;

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

        // Added a hook-pull speed constant.
        public const float HookPullSpeed = 400f;

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

            archi = GetNode<Archi>("Path/To/Archi");
        }

        // Optionally, you can add a method to set a new hook target and switch to HookState.
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

        public override void _PhysicsProcess(double delta)
        {
            currentState?.Update((float)delta);

            MoveAndSlide();
         



            Stamina = Math.Min(Stamina + 1 * (float)delta, 100f);
        }

        public void UpdateState()
        {
            archi.HandleInventoryAndUI();
        }
    }

    // --- State Implementations ---
    public class IdleState : IState
    {
        private readonly States owner;
        public IdleState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Idle State");
        public void Update(float delta)
        {
            if (Input.IsActionPressed("ui_right") || Input.IsActionPressed("ui_left"))
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
        public void Update(float delta)
        {
            if (!Input.IsActionPressed("ui_right") && !Input.IsActionPressed("ui_left"))
                owner.ChangeState(eStates.Idle);
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
        public void Update(float delta) { /* Attacking logic here */ }
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
        public void Update(float delta) { /* Dead state logic */ }
        public void Exit() => GD.Print("Exiting Retired State");
        public void CurrentState() => GD.Print("Current state is DeadState");
        public void InAirState() => owner.ChangeState(eStates.Air);
    }

    public class AirState : IState
    {
        private readonly States owner;
        public AirState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Air State");
        public void Update(float delta)
        {
            owner.Velocity = new Vector2(
                owner.Velocity.X,
                owner.Velocity.Y + States.Gravity * delta
            );

            if (Input.IsActionPressed("ui_right"))
            {
                owner.Velocity = new Vector2(
                    Math.Min(owner.Velocity.X + States.AirMult * States.RunAccel * delta, States.MaxRun),
                    owner.Velocity.Y
                );
            }
            else if (Input.IsActionPressed("ui_left"))
            {
                owner.Velocity = new Vector2(
                    Math.Max(owner.Velocity.X - States.AirMult * States.RunAccel * delta, -States.MaxRun),
                    owner.Velocity.Y
                );
            }

            if (owner.IsOnFloor())
                owner.ChangeState(eStates.Idle);
            else if (Input.IsActionJustPressed("ui_jump"))
                owner.ChangeState(eStates.InAir);
        }
        public void Exit() => GD.Print("Exiting Air State");
        public void CurrentState() => GD.Print("Current state is AirState");
        public void InAirState() => GD.Print("Already in AirState");
    }

    public class InAirState 
    {
        private readonly States owner;
        public InAirState(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering In-Air State");
        public void Update(float delta)
        {
            owner.Velocity = new Vector2(
                owner.Velocity.X,
                owner.Velocity.Y + States.Gravity * delta
            );

            if (Input.IsActionPressed("ui_right"))
            {
                owner.Velocity = new Vector2(
                    Math.Min(owner.Velocity.X + States.AirMult * States.RunAccel * delta, States.MaxRun),
                    owner.Velocity.Y
                );
            }
            else if (Input.IsActionPressed("ui_left"))
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
       
    }

    public class GStrike : IState
    {
        private readonly States owner;
        public GStrike(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Ground Attacking State");
        public void Update(float delta) { /* GStrike logic */ }
        public void Exit() => GD.Print("Exiting Ground Attacking State");
        public void CurrentState() => GD.Print("Current state is GStrike");
        public void InAirState() { GD.Print("Transitioning to InAirState from GStrike"); owner.ChangeState(eStates.Air); }
    }

    public class gDefending : IState
    {
        private readonly States owner;
        public gDefending(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Ground Defending State");
        public void Update(float delta) { /* GDefending logic */ }
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

            Vector2 dashDirection = new Vector2(
                Input.IsActionPressed("ui_right") ? 1 : Input.IsActionPressed("ui_left") ? -1 : 0,
                Input.IsActionPressed("ui_down") ? 1 : Input.IsActionPressed("ui_up") ? -1 : 0
            ).Normalized();

            if (dashDirection == Vector2.Zero)
            {
                dashDirection = new Vector2(Math.Sign(owner.Velocity.X), 0);
                if (dashDirection == Vector2.Zero)
                {
                    owner.ChangeState(eStates.Idle);
                    return;
                }
            }
            owner.Velocity = dashDirection * States.DashSpeed;
        }
        public void Update(float delta)
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
        public void Update(float delta)
        {
            if (Input.IsActionPressed("ui_up"))
            {
                owner.Velocity = new Vector2(
                    owner.Velocity.X,
                    Math.Max(owner.Velocity.Y - States.ClimbAccel * delta, States.ClimbUpSpeed)
                );
                owner.Stamina -= 1 * delta;
            }
            else if (Input.IsActionPressed("ui_down"))
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
        public void Update(float delta) { /* Parry logic */ }
        public void Exit() => GD.Print("Exiting Parry State");
        public void CurrentState() => GD.Print("Current state is ParryState");
        public void InAirState() { GD.Print("Transitioning to InAirState from ParryState"); owner.ChangeState(eStates.Air); }
    }

    public class GStaggered : IState
    {
        private readonly States owner;
        public GStaggered(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering Staggered State");
        public void Update(float delta) { /* Staggered logic */ }
        public void Exit() => GD.Print("Exiting Staggered State");
        public void CurrentState() => GD.Print("Current state is Staggered");
        public void InAirState() { GD.Print("Transitioning to InAirState from GStaggered"); owner.ChangeState(eStates.Air); }
    }

    public class GSymphonCool : IState
    {
        private readonly States owner;
        public GSymphonCool(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering GSymphon Cooldown State");
        public void Update(float delta) { /* Cooldown logic */ }
        public void Exit() => GD.Print("Exiting GSymphon Cooldown State");
        public void CurrentState() => GD.Print("Current state is GSymphonCool");
        public void InAirState() { GD.Print("Transitioning to InAirState from GSymphonCool"); owner.ChangeState(eStates.Air); }
    }

    public class GStrikeSpecialCool : IState
    {
        private readonly States owner;
        public GStrikeSpecialCool(States owner) { this.owner = owner; }
        public void Enter() => GD.Print("Entering GStrike Special Cooldown State");
        public void Update(float delta) { /* Cooldown logic */ }
        public void Exit() => GD.Print("Exiting GStrike Special Cooldown State");
        public void CurrentState() => GD.Print("Current state is GStrikeSpecialCool");
        public void InAirState() { GD.Print("Transitioning to InAirState from GStrikeSpecialCool"); owner.ChangeState(eStates.Air); }
    }

    // --- New Hook State ---
    public class HookState : IState
    {
        private readonly States owner;
        private Vector2 hookTarget;
        // You can adjust the pull speed as needed.
        private readonly float pullSpeed = States.HookPullSpeed;

        // The hook target should be passed in when the state is created.

        public HookState(States owner, Vector2 hookTarget)
        {
            this.owner = owner;
            this.hookTarget = hookTarget;
        }

        public void Enter()
        {
            GD.Print("Entering Hook State");
            // Optionally, you can set up visuals or fire a hook projectile here.
        }

        public void Update(float delta)
        {
            // Calculate the direction vector toward the hook target.
            Vector2 direction = (hookTarget - owner.Position).Normalized();

            // Pull the player toward the hook target.public void QuickGrapple()
   /* {
      if (this.frozen || this.tongued || this.webbed || this.stoned || this.dead)
        return;
      if (PlayerInput.GrappleAndInteractAreShared)
      {
        if (Main.HoveringOverAnNPC || Main.SmartInteractShowingGenuine || Main.SmartInteractShowingFake || this._quickGrappleCooldown > 0 && !Main.mapFullscreen || WiresUI.Settings.DrawToolModeUI)
          return;
        int num = this.controlUseTile ? 1 : 0;
        bool releaseUseTile = this.releaseUseTile;
        if (num == 0 && !releaseUseTile)
          return;
        Tile tileSafely = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
        if (tileSafely.active() && (tileSafely.type == (ushort) 4 || tileSafely.type == (ushort) 33 || tileSafely.type == (ushort) 372 || tileSafely.type == (ushort) 174 || tileSafely.type == (ushort) 49) || this.inventory[this.selectedItem].type == 3384)
          return;
      }
      if (this.noItems)
        return;
      if (this.mount.Active)
        this.mount.Dismount(this);
      Item obj = this.QuickGrapple_GetItemToUse();
      if (obj == null)
        return;
      if (obj.shoot == 73)
      {
        int num = 0;
        for (int index = 0; index < 1000; ++index)
        {
          if (Main.projectile[index].active && Main.projectile[index].owner == Main.myPlayer && (Main.projectile[index].type == 73 || Main.projectile[index].type == 74))
            ++num;
        }
        if (num > 1)
          obj = (Item) null;
      }
      else if (obj.shoot == 165)
      {
        int num = 0;
        for (int index = 0; index < 1000; ++index)
        {
          if (Main.projectile[index].active && Main.projectile[index].owner == Main.myPlayer && Main.projectile[index].type == 165)
            ++num;
        }
        if (num > 8)
          obj = (Item) null;
      }
      else if (obj.shoot == 372)
      {
        int num = 0;
        for (int index = 0; index < 1000; ++index)
        {
          if (Main.projectile[index].active && Main.projectile[index].owner == Main.myPlayer && Main.projectile[index].type == 372)
            ++num;
        }
        if (num > 2)
          obj = (Item) null;
      }
      else if (obj.shoot == 652)
      {
        int num = 0;
        for (int index = 0; index < 1000; ++index)
        {
          if (Main.projectile[index].active && Main.projectile[index].owner == Main.myPlayer && Main.projectile[index].type == 652)
            ++num;
        }
        if (num > 1)
          obj = (Item) null;
      }
      else if (obj.type == 3572)
      {
        int num = 0;
        bool flag = false;
        for (int index = 0; index < 1000; ++index)
        {
          if (Main.projectile[index].active && Main.projectile[index].owner == Main.myPlayer && Main.projectile[index].type >= 646 && Main.projectile[index].type <= 649)
          {
            ++num;
            if ((double) Main.projectile[index].ai[0] == 2.0)
              flag = true;
          }
        }
        if (num > 4 || !flag && num > 3)
          obj = (Item) null;
      }
      else
      {
        for (int index = 0; index < 1000; ++index)
        {
          if (Main.projectile[index].active && Main.projectile[index].owner == Main.myPlayer && Main.projectile[index].type == obj.shoot && (double) Main.projectile[index].ai[0] != 2.0)
          {
            obj = (Item) null;
            break;
          }
        }
      }
      if (obj == null)
        return;
      this.UpdateBlacklistedTilesForGrappling();
      SoundEngine.PlaySound(obj.UseSound, this.position);
      if (Main.netMode == 1 && this.whoAmI == Main.myPlayer)
        NetMessage.SendData(51, number: this.whoAmI, number2: 2f);
      int Type = obj.shoot;
      float shootSpeed = obj.shootSpeed;
      int damage = obj.damage;
      float knockBack = obj.knockBack;
      if (Type == 13 || Type == 32 || Type == 315 || Type >= 230 && Type <= 235 || Type == 331 || Type == 753 || Type == 865 || Type == 935)
      {
        this.grappling[0] = -1;
        this.grapCount = 0;
        for (int index = 0; index < 1000; ++index)
        {
          if (Main.projectile[index].active && Main.projectile[index].owner == this.whoAmI)
          {
            switch (Main.projectile[index].type)
            {
              case 13:
              case 230:
              case 231:
              case 232:
              case 233:
              case 234:
              case 235:
              case 315:
              case 331:
              case 753:
              case 865:
              case 935:
                Main.projectile[index].Kill();
                continue;
              default:
                continue;
            }
          }
        }
      }
      if (Type == 256)
      {
        int num1 = 0;
        int index1 = -1;
        int num2 = 100000;
        for (int index2 = 0; index2 < 1000; ++index2)
        {
          if (Main.projectile[index2].active && Main.projectile[index2].owner == this.whoAmI && Main.projectile[index2].type == 256)
          {
            ++num1;
            if (Main.projectile[index2].timeLeft < num2)
            {
              index1 = index2;
              num2 = Main.projectile[index2].timeLeft;
            }
          }
        }
        if (num1 > 1)
          Main.projectile[index1].Kill();
      }
      if (Type == 652)
      {
        int num3 = 0;
        int index3 = -1;
        int num4 = 100000;
        for (int index4 = 0; index4 < 1000; ++index4)
        {
          if (Main.projectile[index4].active && Main.projectile[index4].owner == this.whoAmI && Main.projectile[index4].type == 652)
          {
            ++num3;
            if (Main.projectile[index4].timeLeft < num4)
            {
              index3 = index4;
              num4 = Main.projectile[index4].timeLeft;
            }
          }
        }
        if (num3 > 1)
          Main.projectile[index3].Kill();
      }
      if (Type == 73)
      {
        for (int index = 0; index < 1000; ++index)
        {
          if (Main.projectile[index].active && Main.projectile[index].owner == this.whoAmI && Main.projectile[index].type == 73)
            Type = 74;
        }
      }
      if (obj.type == 3572)
      {
        int num5 = -1;
        int num6 = -1;
        for (int index = 0; index < 1000; ++index)
        {
          Projectile projectile = Main.projectile[index];
          if (projectile.active && projectile.owner == this.whoAmI && projectile.type >= 646 && projectile.type <= 649 && (num6 == -1 || num6 < projectile.timeLeft))
          {
            num5 = projectile.type;
            num6 = projectile.timeLeft;
          }
        }
        switch (num5)
        {
          case -1:
          case 649:
            Type = 646;
            break;
          case 646:
            Type = 647;
            break;
          case 647:
            Type = 648;
            break;
          case 648:
            Type = 649;
            break;
        }
      }
      Vector2 vector2 = new Vector2(this.position.X + (float) this.width * 0.5f, this.position.Y + (float) this.height * 0.5f);
      float f1 = (float) Main.mouseX + Main.screenPosition.X - vector2.X;
      float f2 = (float) Main.mouseY + Main.screenPosition.Y - vector2.Y;
      if ((double) this.gravDir == -1.0)
        f2 = Main.screenPosition.Y + (float) Main.screenHeight - (float) Main.mouseY - vector2.Y;
      float num7 = (float) Math.Sqrt((double) f1 * (double) f1 + (double) f2 * (double) f2);
      float num8;
      if (float.IsNaN(f1) && float.IsNaN(f2) || (double) f1 == 0.0 && (double) f2 == 0.0)
      {
        f1 = (float) this.direction;
        f2 = 0.0f;
        num8 = shootSpeed;
      }
      else
        num8 = shootSpeed / num7;
      float SpeedX = f1 * num8;
      float SpeedY = f2 * num8;
      Projectile.NewProjectile(vector2.X, vector2.Y, SpeedX, SpeedY, Type, damage, knockBack, this.whoAmI);
    }

    public Item QuickGrapple_GetItemToUse()
            owner.Velocity = direction * pullSpeed;

            // If the player is close enough to the target, return to Idle state.
            if (owner.Position.DistanceTo(hookTarget) < 10f)
                owner.ChangeState(eStates.Idle);
        }*/

        public void Exit() => GD.Print("Exiting Hook State");
        public void CurrentState() => GD.Print("Current state is HookState");
        public void InAirState() => owner.ChangeState(eStates.Air);
    }
}
