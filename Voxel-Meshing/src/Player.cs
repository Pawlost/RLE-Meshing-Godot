using System;
using Godot;
using Godotmeshing.src.Other;

public partial class Player : Node3D
{
    [Export] public float MOUSE_SENSITIVITY = 0.002F;
    [Export] public float MOVE_SPEED = 0.9F;

    private Vector3 motion;
    private Vector3 initialRotation;
    private const float RAY_LENGHT = 10;
    private RayCast3D ray;
    public Camera3D camera;
    private bool wireframe = false;

    public override void _Input(InputEvent @event)
    {

        if (Input.IsActionPressed("toggle_wireframe_mode"))
        {
            if (wireframe)
            {
                GetViewport().DebugDraw = SubViewport.DebugDrawEnum.Wireframe;
            }
            else
            {
                GetViewport().DebugDraw = SubViewport.DebugDrawEnum.Disabled;
            }

            wireframe = !wireframe;
        }else if (Input.IsActionJustReleased("toggle_mouse_capture"))
        {
            Input.MouseMode = Input.MouseMode ==
                Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible :
                Input.MouseModeEnum.Captured;
        }
        else if (@event is InputEventMouseMotion eventKey)
        {

            if (Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                Vector3 rotation = new Vector3(
                    (float)Math.Max(Math.Min(Rotation.X - eventKey.Relative.Y * MOUSE_SENSITIVITY, Math.PI / 2), -Math.PI / 2),
                    Rotation.Y - eventKey.Relative.X * MOUSE_SENSITIVITY,
                    Rotation.Z);
                TerraBasis basis = new TerraBasis(Converter.ConvertVector(rotation));

                Rotation = rotation;
            }
        }
      /*  else if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed)/* &&
            eventMouseButton.ButtonIndex == 1)
        {

            Vector3 from = camera.ProjectRayOrigin(eventMouseButton.Position);
            Vector3 to = camera.ProjectRayNormal(eventMouseButton.Position) * RAY_LENGHT;
            GD.Print(to);
            // ray.Translation = from;
            //ray.CastTo = to;
            //ray.Enabled = true;
        }*/
    }

    public override void _Ready()
    {
        GD.Print(ConfigManager.BASE_CONFIG_FILE_DIRECTORY_PATH);
        GD.Print(ConfigManager.BASE_DIRECTORY);
        GD.Print(ConfigManager.BASE_CONFIG_FILE_PATH);
        RenderingServer.SetDebugGenerateWireframes(true);
  //      camera = (Camera3D)FindNode("Camera3D");
        initialRotation = new Vector3();

               Input.MouseMode = Input.MouseModeEnum.Captured;

        //    TerraVector3 origin = Converter.ConvertVector(GlobalTransform.origin);
        //  TerraBasis basis = Converter.ConvertBasis(GlobalTransform.basis);
    }

    public override void _PhysicsProcess(double delta)
    {
        /*  if (ray.IsColliding ()) {
              picker.Pick (ray.GetCollisionPoint (), ray.GetCollisionNormal ());
              ray.Enabled = false;
          }*/


        Vector3 velocity = new Vector3();


        if (Input.IsActionPressed("walk_left"))
        {
            motion.X = -1;
        }
        else if (Input.IsActionPressed("walk_right"))
        {
            motion.X = 1;
        }
        else
        {
            motion.X = 0;
        }

        if (Input.IsActionPressed("walk_forward"))
        {
            motion.Z = -1;
        }
        else if (Input.IsActionPressed("walk_backward"))
        {
            motion.Z = 1;
        }
        else
        {
            motion.Z = 0;
        }

        if (Input.IsActionPressed("move_up"))
        {
            motion.Y = 1;
        }
        else if (Input.IsActionPressed("move_down"))
        {
            motion.Y = -1;
        }
        else
        {
            motion.Y = 0;
        }

        motion = motion.Normalized();

        motion = motion.Rotated(new Vector3(0, 1, 0), Rotation.Y - initialRotation.Y)
            .Rotated(new Vector3(1, 0, 0), (float)Math.Cos(Rotation.Y) * Rotation.X)
            .Rotated(new Vector3(0, 0, 1), -(float)Math.Sin(Rotation.Y) * Rotation.X);

        velocity = motion * MOVE_SPEED;

        Vector3 translation = new Vector3(Position.X + velocity.X, Position.Y + velocity.Y,
            Position.Z + velocity.Z);

        Position = translation;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}