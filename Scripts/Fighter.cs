using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum FacingDir { Right, Left }
public enum FighterAction { Attack, Counter, Grab }
public partial class Fighter : CharacterBody2D
{
	#region Propriedades
	[Export] public int Speed { get; protected set; } = 300;
	[Export] public int Gravity { get; protected set; } = 800;
	[Export] public int JumpForce { get; protected set; } = -400;
	[Export] public Fighter Opponent { get; set; }
	#endregion

	#region State Mng
	protected enum State { Idle, Walk, Jump, Attack, Block, Hitstun }
	protected State currentState = State.Idle;
	public FacingDir Facing { get; private set; } = FacingDir.Right;
	#endregion

	#region Input Buffer
	private readonly Queue<FighterAction> _inputBuffer = new();
	private const float BufferTime = 0.2f;
	private float _bufferTimer;
	#endregion

	#region Lifecycle
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		float dt = (float)delta;
		
		ApplyGravity(ref velocity, dt);
		ProcessInputBuffer(dt);
		UpdateFacing();
		HandleState(ref velocity, dt);

		Velocity = velocity;
		MoveAndSlide();
	}
	#endregion

	#region Core
	private void ApplyGravity(ref Vector2 velocity, float delta)
	{
		if (!IsOnFloor())
			velocity.Y += Gravity * delta;
		else
			velocity.Y = 0;
	}

	private void ProcessInputBuffer(float delta)
	{
		_bufferTimer -= delta;
		if (_bufferTimer <= 0 && _inputBuffer.Count > 0)
			_inputBuffer.Dequeue();
		
		if (Input.IsActionJustPressed("attack")) BufferInput(FighterAction.Attack);
		if (Input.IsActionJustPressed("counter")) BufferInput(FighterAction.Counter);
		if (Input.IsActionJustPressed("grab")) BufferInput(FighterAction.Grab);
	}

	private void BufferInput(FighterAction action)
	{
		_inputBuffer.Enqueue(action);
		_bufferTimer = BufferTime;
	}
	#endregion

	#region State Handling
	private void HandleState(ref Vector2 velocity, float delta)
	{
		switch (currentState)
		{
			case State.Idle: HandleIdle(ref velocity, delta); break;
			case State.Walk: HandleWalk(ref velocity, delta); break;
			case State.Jump: HandleJump(ref velocity, delta); break;
			case State.Attack: HandleAttack(delta); break;
			case State.Block: HandleBlock(delta); break;
			case State.Hitstun: HandleHitstun(delta); break;
		}
	}

	// MÉTODOS DE STATE
	protected virtual void HandleIdle(ref Vector2 velocity, float delta)
	{
		float input = Input.GetAxis("ui_left", "ui_right");
		if (input != 0)
		{
			currentState = State.Walk;
			velocity.X = input * Speed;
		} 
		else if (_inputBuffer.Count > 0)
		{
			TryStartMove(_inputBuffer.Dequeue());
		}
	}

	protected virtual void HandleWalk(ref Vector2 velocity, float delta)
	{
		float input = Input.GetAxis("ui_left", "ui_right");
		velocity.X = input * Speed;

		if (input == 0)
			currentState = State.Idle;
	}

	protected virtual void HandleJump(ref Vector2 velocity, float delta)
	{
		if (IsOnFloor())
			currentState = State.Idle;
	}

	protected virtual void HandleAttack(float dt)
	{
		//template
	}

	protected virtual void HandleBlock(float delta)
	{
		//template
	}

	protected virtual void HandleHitstun(float delta)
	{
		//template
	}
	#endregion

	// Despionar
	#region Facing
	protected void UpdateFacing()
	{
		if (Opponent == null) return;
		Facing = GlobalPosition.X < Opponent.GlobalPosition.X
			? FacingDir.Right
			: FacingDir.Left;
	}
	

	protected virtual void TryStartMove(FighterAction action)
	{
		switch (action)
		{
			case FighterAction.Attack:
				StartAttack();
				break;
			case FighterAction.Counter:
				StartCounter();
				break;
			case FighterAction.Grab:
				StartGrab();
				break;
		}
	}
	#endregion

	#region ActionTemplate
	protected virtual void StartAttack()
	{
		currentState = State.Attack;
		GD.Print("Ataque normal");
	}

	protected virtual void StartCounter()
	{
		currentState = State.Attack;
		GD.Print("Contra-Ataque Normal");
	}

	protected virtual void StartGrab()
	{
		currentState = State.Attack;
		GD.Print("Grab normal");
	}
	#endregion

	// n sei como nomear isso
	protected void EndMove()
	{
		currentState = State.Idle;
	}

	protected void EnterHitstun(int frames)
	{
		currentState = State.Hitstun;
	}


}
