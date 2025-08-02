using Godot;
using System.Threading.Tasks;

public partial class BasicControls : CharacterBody2D
{
	[Export]
	public int speed { get; set; } = 400;
	Vector2 inputDirection;
	float direction = 0;

	[Export]
	public int jumpForce { get; set; } = 60;

	[Export]
	public int gravity { get; set; } = -10;

	[Export]
	public int maxGrav { get; set; } = -10;

	bool jumpPressed = false;
	bool jumping = false;

	[Export]
	public Node2D gravCenter;

	AnimatedSprite2D sprite;

	public override void _Ready()
	{
		sprite = GetChild<AnimatedSprite2D>(0);
	}

	[Export]
	Vector2 hVel, vVel;
	public void GetInput()
	{
		Vector2 sideDirection;
		if (IsOnFloor())
		{
			sideDirection = GetPerpendicularClockwise(GetFloorNormal());
			GD.Print("Using floor normal");
		}
		else {
			sideDirection = GetPerpendicularClockwise(UpDirection);//new Vector2(-UpDirection.Y, UpDirection.X);
		}
		direction = Input.GetAxis("left", "right");
		inputDirection = sideDirection * direction;

		jumpPressed = Input.IsActionJustPressed("jump");
	}

	public override void _PhysicsProcess(double delta)
	{
		if ((IsOnFloor() || IsOnCeiling()) && jumping)
		{
			Land();
			//if (IsOnCeiling())
			//vVel = Vector2.Zero;
		}


		GetInput();
		hVel = inputDirection * speed;

		if(!IsOnFloor()) //isonfloor
			vVel += UpDirection * gravity;

		if (jumpPressed && !jumping)
		{
			Jump();
		}

		Velocity = hVel + vVel;

		MoveAndSlide();

		if (IsOnFloor())
			Rotation = GetFloorNormal().Angle() + Mathf.DegToRad(90);
		else
			Rotation = UpDirection.Angle() + Mathf.DegToRad(90);
	}

	public override void _Process(double delta)
	{
		if (!jumping)
		{
			if (direction != 0)
			{
				sprite.FlipH = direction < 0;
				sprite.Play("walking");
			}
			else
			{
				sprite.Play("default");
			}
		}
		else {
			if (direction != 0)
				sprite.FlipH = direction < 0;
	
			if (sprite.Frame == 1) {
				sprite.Play("jumping");
			}
		}

		if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			Vector2 mousePos = GetGlobalMousePosition();
			gravCenter.GlobalPosition = mousePos;
			vVel = Vector2.Zero;
		}

		UpDirection = -(gravCenter.GlobalPosition - Position).Normalized();

	}

	void Jump() {
		vVel = UpDirection * jumpForce;
		jumping = true;
		sprite.Play("jump");
	}

	void Land() {
		//sprite.Play("land");
		//await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
		jumping = false;

	}


	Vector2 GetPerpendicularClockwise(Vector2 v) {
		return new Vector2(-v.Y, v.X);
	}
}