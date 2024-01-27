using Godot;

namespace DodgeTheCreeps.Gaymer
{
    public partial class Gaymer : Area2D
    {
        [Signal]
        public delegate void HitEventHandler();

        [Export]
        public int Speed { get; set; } = 400;
        public Vector2 ScreenSize { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            ScreenSize = GetViewportRect().Size;
            Hide();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            Vector2 velocity = Vector2.Zero;

            if (Input.IsActionPressed("move_right"))
            {
                velocity.X += 1;
            }

            if (Input.IsActionPressed("move_left"))
            {
                velocity.X -= 1;
            }

            if (Input.IsActionPressed("move_up"))
            {
                velocity.Y -= 1;
            }
            if (Input.IsActionPressed("move_down"))
            {
                velocity.Y += 1;
            }

            AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

            // Normalize the velocity vector when moving diagonally
            // because vector math
            if (velocity.Length() > 0)
            {
                velocity = velocity.Normalized() * Speed;
                animatedSprite2D.Play();
            }
            else
            {
                animatedSprite2D.Stop();
            }

            // update sprite position
            Position += velocity * (float)delta;
            Position = new Vector2(
                x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
                y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
            );

            if (velocity.X != 0)
            {
                animatedSprite2D.Animation = "Walk";
                animatedSprite2D.FlipV = false;
                animatedSprite2D.FlipH = velocity.X < 0;
            }
            else if (velocity.Y != 0)
            {
                animatedSprite2D.Animation = "Up";
                animatedSprite2D.FlipV = velocity.Y > 0;
            }
        }

        private void OnBodyEntered(Node2D body)
        {
            Hide(); // player disappears after being hit
            EmitSignal(SignalName.Hit);
            // disabling the area's collision shape can cause an error if it happens in the middle of the engine's collision processing.
            // using SetDeferred() tells godot to wait to disable the shape until it's safe to do so.
            GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        }

        public void Start(Vector2 position)
        {
            Position = position;
            Show();
            GetNode<CollisionShape2D>("CollisionShape2D");
        }
    }
}
