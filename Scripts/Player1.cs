// TODO: REFATORAR PARA STATE MACHINE. RESOLVER O PROBLEMA DOS JOGADORES ESTAREM OLHANDO UM PRO OUTRO.

using Godot;
using System;

public partial class Player1 : CharacterBody2D
{

	[Export] public string MoveLeftAction { get; set; } = "ui_left";
	[Export] public string MoveRightAction { get; set; } = "ui_right";
	[Export] public string JumpAction { get; set; } = "ui_up";

	[Export] private int _speed = 300;
	[Export] private int _gravity = 800;
	[Export] private int _jumpForce = -400;
	[Export] private int _jumpHorizontalSpeed = 200;

	private bool _facingRight = true;
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
	}


	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		Vector2 velocity = Velocity;

		if (!IsOnFloor()){
			velocity.Y += _gravity * (float)delta;
		}

		// movimentação
		float direction = Input.GetAxis(MoveLeftAction, MoveRightAction);
		if (IsOnFloor()){
			velocity.X = direction * _speed;

			_facingRight = GlobalPosition.X < _otherPlayer.GlobalPosition.X;

			// função comentada abaixo
			UpdateFacingDirection();
		}

		// pulos
		if (Input.IsActionPressed(JumpAction) && IsOnFloor()){
			if (Input.IsActionPressed(MoveRightAction)){
				velocity.Y = _jumpForce;
				velocity.X = _jumpHorizontalSpeed;
			} else if (Input.IsActionPressed(MoveLeftAction)){
				velocity.Y = _jumpForce;
				velocity.X = -_jumpHorizontalSpeed;
			} else {
				velocity.Y = _jumpForce;
				velocity.X = 0;
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	// EU SIMPLESMENTE NÃO CONSIGO ENTENDER POR QUE ISSO N FUNCIONA.
	
	private void UpdateFacingDirection(){
		if (_otherPlayer == null) return;

		Scale = new Vector2(_facingRight ? 1 : -1, Scale.Y);
	}

}
