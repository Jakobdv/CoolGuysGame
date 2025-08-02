using Godot;
using System.Threading.Tasks;

public partial class InvertableControls : CharacterBody2D
{
	[Export]
	public int speed { get; set; } = 400;
	float direction;
	bool inverted = false;
	Vector2 inputDirection;

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

	[Export]
	Vector2 hVel, vVel;
	public void GetInput()
	{
		direction = Input.GetAxis("left", "right");
		inputDirection = Vector2.Right * direction;

		jumpPressed = Input.IsActionJustPressed("jump");

		//Inverting gravity (I think I'll TRYYYYYYYY INVERRRRRRRRRRRRRTING GRAAAAVITYYYY)
		if (Input.IsActionJustPressed("invert_test"))
		{
			UpDirection = -UpDirection;
			vVel = Vector2.Zero;
			inverted = !inverted;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		//Move from jumping to grounded state
		if ((IsOnFloor() || IsOnCeiling()) && jumping)
		{
			Land();
			if (IsOnCeiling())
				vVel = Vector2.Zero; //Prevents player from sticking to ceilings when jumping
		}

		GetInput();
		hVel = inputDirection * speed;

		if (!IsOnFloor()) //Gravity is only added when in the air
		{ 
			vVel += UpDirection * gravity;
			if (Mathf.Abs(GlobalPosition.Y - jumpStart.Y) > jumpMaxRecordedHeight)
				jumpMaxRecordedHeight = Mathf.Abs(GlobalPosition.Y - jumpStart.Y);
		}
		else if (jumpPressed && !jumping)
		{
			Jump();
		}

		Velocity = hVel + vVel; //I separated the horizontal and vertical velocities so I could more freely edit them while making the point gravity system. Not sure it's really needed now but left it in for the hell of it

		MoveAndSlide();
		
		//Rotates player so they're facing up (relative to gravity)
		if (IsOnFloor())
			Rotation = GetFloorNormal().Angle() + Mathf.DegToRad(90);
		else
			Rotation = UpDirection.Angle() + Mathf.DegToRad(90);
	}

	//For testing out max distance/ height jump gets with current settings
	public Vector2 jumpStart, jumpEnd;
	[Export]
	public float jumpMaxRecordedHeight, jumpRecordedDistance;
	void Jump()
	{
		vVel = UpDirection * jumpForce;
		jumping = true;
		sprite.Play("jump");

		jumpStart = GlobalPosition;
		jumpMaxRecordedHeight = 0;
	}

	void Land()
	{
		//Testing out a landing animation
		//sprite.Play("land"); 
		//await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
		jumping = false;

		jumpEnd = GlobalPosition;
		jumpRecordedDistance = (jumpStart - jumpEnd).Length();
	}

	public override void _Process(double delta)
	{
		//Handles changing and flipping the animations depending on state/ direction
		if (!jumping)
		{
			if (direction != 0)
			{
				sprite.FlipH = direction * (inverted ? -1 : 1) < 0;
				sprite.Play("walking");
			}
			else
			{
				sprite.Play("default");
			}
		}
		else {
			if (direction != 0)
				sprite.FlipH = direction * (inverted ? -1 : 1)  < 0;
	
			if (sprite.Frame == 1) {
				sprite.Play("jumping");
			}
		}
	}

	//Used for adjusting horizontal movement when I was trying point gravity. Currently unused
	Vector2 GetPerpendicularClockwise(Vector2 v) {
		return new Vector2(-v.Y, v.X);
	}
}