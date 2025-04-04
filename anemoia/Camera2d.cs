using Godot;
using System;

public partial class Camera2d : Camera2D
{


    [Export]


    public  float DeadZone = 160.0f;


    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            Vector2 mousePosition = mouseMotionEvent.Position;
            Vector2 cameraPosition = GlobalPosition;

            // Calculate the distance from the camera to the mouse position
            float distanceToMouse = cameraPosition.DistanceTo(mousePosition);

            // If the distance is greater than the dead zone, move the camera
            if (distanceToMouse > DeadZone)
            {
                Vector2 direction = (mousePosition - cameraPosition).Normalized();
                GlobalPosition += direction * (distanceToMouse - DeadZone);
            }
        }
        if(@event is InputEventMouseButton mouseButtonEvent)
        {
            // Handle mouse button events
            if (mouseButtonEvent.IsPressed())
            {
                // Handle mouse button press event
                GD.Print("Mouse button pressed at: ", mouseButtonEvent.Position);
            }
        }
        
        }
    }


