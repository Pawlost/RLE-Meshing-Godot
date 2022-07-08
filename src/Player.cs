using System;
using Godot;

public class Player : Spatial 
{
    [Export] public float MOUSE_SENSITIVITY = 0.002F;
    [Export] public float MOVE_SPEED = 0.9F;

    private Vector3 motion;
    private Vector3 initialRotation;
    private const float RAY_LENGHT = 10;
    private RayCast ray;
    public Camera camera;
    private bool wireframe = false;

    public override void _Input (InputEvent @event) {
        if (Input.IsActionPressed ("toggle_mouse_capture")) {
            Input.SetMouseMode (Input.GetMouseMode () == Input.MouseMode.Captured ?
                Input.MouseMode.Visible :
                Input.MouseMode.Captured);
        } else if (Input.IsActionPressed ("toggle_wireframe_mode")) {
            if (wireframe) {
                GetViewport ().DebugDraw = Viewport.DebugDrawEnum.Wireframe;
            } else {
                GetViewport ().DebugDraw = Viewport.DebugDrawEnum.Disabled;
            }

            wireframe = !wireframe;
        } else if (Input.IsActionPressed ("ui_cancel")) {
            GetTree ().Quit ();
        } else if (@event is InputEventMouseMotion eventKey) {

            if (Input.GetMouseMode () == Input.MouseMode.Captured) {

                Vector3 rotation = new Vector3 (
                    (float) Math.Max (Math.Min (Rotation.x - eventKey.Relative.y * MOUSE_SENSITIVITY, Math.PI / 2), -Math.PI / 2),
                    Rotation.y - eventKey.Relative.x * MOUSE_SENSITIVITY,
                    Rotation.z);
                TerraBasis basis = new TerraBasis (Converter.ConvertVector (rotation));

                Rotation = rotation;
            }

        } else if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed &&
            eventMouseButton.ButtonIndex == 1) {

            Vector3 from = camera.ProjectRayOrigin (eventMouseButton.Position);
            Vector3 to = camera.ProjectRayNormal (eventMouseButton.Position) * RAY_LENGHT;
            GD.Print (to);
           // ray.Translation = from;
            //ray.CastTo = to;
            //ray.Enabled = true;
        }
    }

    public override void _Ready () {
        GD.Print (ConfigManager.BASE_CONFIG_FILE_DIRECTORY_PATH);
        GD.Print (ConfigManager.BASE_DIRECTORY);
        GD.Print (ConfigManager.BASE_CONFIG_FILE_PATH);
        VisualServer.SetDebugGenerateWireframes (true);
        camera = (Camera) FindNode ("Camera");
        initialRotation = new Vector3 ();

        TerraVector3 origin = Converter.ConvertVector (GlobalTransform.origin);
        TerraBasis basis = Converter.ConvertBasis (GlobalTransform.basis);
    }

    public override void _PhysicsProcess (float delta) {
      /*  if (ray.IsColliding ()) {
            picker.Pick (ray.GetCollisionPoint (), ray.GetCollisionNormal ());
            ray.Enabled = false;
        }*/


        Vector3 velocity = new Vector3 ();


        if (Input.IsActionPressed ("walk_left")) {
            motion.x = 1;
        } else if (Input.IsActionPressed ("walk_right")) {
            motion.x = -1;
        } else {
            motion.x = 0;
        }

        if (Input.IsActionPressed ("walk_forward")) {
            motion.z = -1;
        } else if (Input.IsActionPressed ("walk_backward")) {
            motion.z = 1;
        } else {
            motion.z = 0;
        }

        if (Input.IsActionPressed ("move_up")) {
            motion.y = 1;
        } else if (Input.IsActionPressed ("move_down")) {
            motion.y = -1;
        } else {
            motion.y = 0;
        }

        motion = motion.Normalized ();

        if (Input.IsActionPressed ("move_speed")) {
            motion *= 2;
        }


        motion = motion.Rotated (new Vector3 (0, 1, 0), Rotation.y - initialRotation.y)
            .Rotated (new Vector3 (1, 0, 0), (float) Math.Cos (Rotation.y) * Rotation.x)
            .Rotated (new Vector3 (0, 0, 1), -(float) Math.Sin (Rotation.y) * Rotation.x);

        velocity = motion * MOVE_SPEED;

        Vector3 translation = new Vector3 (Translation.x + velocity.x, Translation.y + velocity.y,
            Translation.z + velocity.z);

        Translation = translation;
    }

    public override void _ExitTree () {
        Input.SetMouseMode (Input.MouseMode.Visible);
    }

    public override void _Process (float delta) {

    }
}