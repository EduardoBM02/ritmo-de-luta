// TODO: REFATORAR PARA STATE MACHINE. RESOLVER O PROBLEMA DOS JOGADORES ESTAREM OLHANDO UM PRO OUTRO.

using Godot;
using System;

public enum PlayerState{
	Idle,
	Walk,
	Jump,
	Attack
}

public partial class Player1 : CharacterBody2D
{
	private PlayerState _currentState = PlayerState.Idle;
	[Export] public string MoveLeftAction { get; set; } = "ui_left";
	[Export] public string MoveRightAction { get; set; } = "ui_right";
	[Export] public string JumpAction { get; set; } = "ui_up";

	[Export] private int _speed = 300;
	[Export] private int _gravity = 800;
	[Export] private int _jumpForce = -400;
	[Export] private int _jumpHorizontalSpeed = 200;

	[Export] public bool _facingRight { get; set; } = true;
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

		Vector2 currentScale = Scale;

		UpdateFacingDirection();

		GD.Print("Player: " + this);
		GD.Print("other Player of " + this + ": " + _otherPlayer);
		GD.Print(_facingRight);

		CheckDirection("Player1");
		CheckDirection("Player2");
	}


	public override void _Process(double delta)
	{
		GD.Print(_currentState);
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
	
	private void UpdateFacingDirection(){
		_facingRight = GlobalPosition.X < _otherPlayer.GlobalPosition.X;
		Scale = new Vector2(_facingRight ? 1 : -1, Scale.Y);
	}

	private void CheckDirection(string playerName){
		if (!_facingRight) {
			if (this.Name == playerName){
				GD.Print("facing left..." + Scale.X);
			}
		} else {
			if (Name == playerName){
				GD.Print("facing right..." + Scale.X);
			}
		}
	}
}
