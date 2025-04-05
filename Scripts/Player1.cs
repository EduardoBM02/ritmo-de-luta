using Godot;
using System;

public enum PlayerState{
	Idle,
	Walk,
	Jump,
	Attack
}

public enum FacingDirection{
	Right,
	Left
}

public partial class Player1 : CharacterBody2D
{
	private PlayerState _currentState = PlayerState.Idle;
	[Export] private FacingDirection _currentFacing;
	[Export] private Texture2D _rightColor;
	[Export] private Texture2D _leftColor;
	private Sprite2D _sprite;
	[Export] public string MoveLeftAction { get; set; } = "ui_left";
	[Export] public string MoveRightAction { get; set; } = "ui_right";
	[Export] public string JumpAction { get; set; } = "ui_up";

	[Export] private int _speed = 300;
	[Export] private int _gravity = 800;
	[Export] private int _jumpForce = -400;
	[Export] private int _jumpHorizontalSpeed = 200;

	//[Export] public bool _facingRight { get; set; }
	private Player1 _otherPlayer;

	public override void _Ready()
	{
		base._Ready();

		foreach (Node node in GetParent().GetChildren()){
			if (node is Player1 player && player != this){
				_otherPlayer = player;
				break;
			}
		}

		GD.Print("Player: " + this);
		GD.Print("other Player of " + this + ": " + _otherPlayer);
		//UpdateFacingDirection();
		_sprite = new Sprite2D();
		AddChild(_sprite);
		UpdateFacing();
	}


	public override void _Process(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor()) {
			velocity.Y += _gravity * (float)delta;
		}
		else {
			velocity.Y = 0;
		}

		switch (_currentState){
			case PlayerState.Idle:
				HandleIdle(ref velocity);
				break;
			case PlayerState.Walk:
				HandleWalk(ref velocity);
				break;
			case PlayerState.Jump:
				HandleJump(ref velocity);
				break;
		}

		Velocity = velocity;
		UpdateFacing();
		MoveAndSlide();
	}

	private void HandleIdle(ref Vector2 velocity){
		velocity.X = 0;

		float direction = Input.GetAxis(MoveLeftAction, MoveRightAction);

		if (IsOnFloor() && direction != 0){
			_currentState = PlayerState.Walk;
			return;
		}
		if (Input.IsActionJustPressed(JumpAction)) {
			velocity.Y = _jumpForce;
			_currentState = PlayerState.Jump;
		}
	}

	private void HandleWalk(ref Vector2 velocity){
		float direction = Input.GetAxis(MoveLeftAction, MoveRightAction);

		velocity.X = direction * _speed;

		if (direction == 0){
			_currentState = PlayerState.Idle;
		}

		if (Input.IsActionPressed(JumpAction)){
			velocity.Y = _jumpForce;
			_currentState = PlayerState.Jump;
		}
	}

	private void HandleJump(ref Vector2 velocity){
		if (IsOnFloor()) {
			_currentState = Input.GetAxis(MoveLeftAction, MoveRightAction) != 0
				? PlayerState.Walk 
				: PlayerState.Idle;
		}
	}

	// EU SIMPLESMENTE NÃO CONSIGO ENTENDER POR QUE ISSO N FUNCIONA.

	private void UpdateFacing(){
		if (IsOnFloor())
		{
			_currentFacing = GlobalPosition.X < _otherPlayer.GlobalPosition.X 
				? FacingDirection.Right 
				: FacingDirection.Left;
		}
		UpdateAppearance();
	}

	private void UpdateAppearance(){
		_sprite.Texture = _currentFacing == FacingDirection.Right ? _rightColor : _leftColor;
	}
}
