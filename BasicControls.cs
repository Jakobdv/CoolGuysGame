using Godot;
using System.Threading.Tasks;

public partial class BasicControls : CharacterBody2D
{
	[Export]
	public int speed { get; set; } = 400;
	float inputDirection;

	[Export]
	public int jumpForce { get; set; } = 60;

	[Export]
	public int gravity { get; set; } = -10;

	bool jumpPressed = false;
	bool jumping = false;

	AnimatedSprite2D sprite;

	public override void _Ready()
	{
		sprite = GetChild<AnimatedSprite2D>(0);
	}

	Vector2 vel;
	public void GetInput()
	{
		inputDirection = Input.GetAxis("left", "right");

		jumpPressed = Input.IsActionJustPressed("jump");
	}

	public override void _PhysicsProcess(double delta)
	{
		if ((IsOnFloor() || IsOnCeiling()) && jumping)
		{
			Land();
			if (IsOnCeiling())
				vel.Y = 0;
		}
		

		GetInput();
		vel.X = inputDirection * speed;

//		if(vel.Y < gravity)
			vel += Vector2.Up * gravity;

		if (jumpPressed && !jumping)
		{
			GD.Print("bop!");
			Jump();
		}

		Velocity = vel;
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		if (!jumping)
		{
			if (inputDirection != 0)
			{
				sprite.FlipH = inputDirection < 0;
				sprite.Play("walking");
			}
			else
			{
				sprite.Play("default");
			}
		}
		else {
			if (inputDirection != 0)
				sprite.FlipH = inputDirection < 0;
	
			if (sprite.Frame == 1) {
				sprite.Play("jumping");
			}
		}
	}

	void Jump() {
		vel.Y = UpDirection.Y * jumpForce;
		jumping = true;
		sprite.Play("jump");
	}

	void Land() {
		//sprite.Play("land");
		//await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
		jumping = false;

	}
}