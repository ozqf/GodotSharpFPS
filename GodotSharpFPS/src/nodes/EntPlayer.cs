using Godot;
using System;
using GodotSharpFps.src;
using GodotSharpFps.src.nodes;

public class EntPlayer : Spatial
{
	private const string MoveForward = "move_forward";
	private const string MoveBackward = "move_backward";
	private const string MoveLeft = "move_left";
	private const string MoveRight = "move_right";

	private const string MoveUp = "move_up";
	private const string MoveDown = "move_down";

	private const string LookLeft = "ui_left";
	private const string LookRight = "ui_right";
	private const string LookUp = "ui_up";
	private const string LookDown = "ui_down";

	private const string Attack1 = "attack_1";
	private const string Attack2 = "attack_2";

	private FPSInput _input = new FPSInput();
	private FPSController _fpsCtrl;
	private Spatial _head;
	private ActorInventory _inventory;
	private LaserDot _laserDot;
	private SwordThrowProjectile _thrownSword;
	private HUDPlayerState _hudState = new HUDPlayerState();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Main m = GetNode<Main>("/root/main");

		// find Godot scene nodes
		var body = GetNode<KinematicBody>("actor_base");
		//ActorProvider body = GetNode<ActorProvider>("actor_base");
		_head = GetNode<Spatial>("actor_base/head");
		_laserDot = GetNode<LaserDot>("laser_dot");
		_laserDot.CustomInit(_head, uint.MaxValue, 1000);

		_thrownSword = m.factory.SpawnThrownSword(false);
		m.AddOrphanNode(_thrownSword);

		// init components
		_fpsCtrl = new FPSController(body, _head);

		// Inventory
		_inventory = new ActorInventory();
		_inventory.Init(_head, 1);

		// Add weapons
		InvWeapon weapon = AttackFactory.CreatePlayerShotgun(_head, body);
		_inventory.AddWeapon(weapon);

		m.cam.AttachToTarget(_head);
	}

	private void ApplyInputButtonBit(FPSInput input, string keyName, int bit)
	{ if (Input.IsActionPressed(keyName)) { input.buttons |= bit; } }

	public override void _PhysicsProcess(float delta)
	{
		// Clear all inputs and reapply
		_input.buttons = 0;

		if (Main.i.gameInputActive)
		{
			ApplyInputButtonBit(_input, MoveForward, FPSInput.BitMoveForward);
			ApplyInputButtonBit(_input, MoveBackward, FPSInput.BitMoveBackward);
			ApplyInputButtonBit(_input, MoveLeft, FPSInput.BitMoveLeft);
			ApplyInputButtonBit(_input, MoveRight, FPSInput.BitMoveRight);

			ApplyInputButtonBit(_input, LookUp, FPSInput.BitLookUp);
			ApplyInputButtonBit(_input, LookDown, FPSInput.BitLookDown);
			ApplyInputButtonBit(_input, LookLeft, FPSInput.BitLookLeft);
			ApplyInputButtonBit(_input, LookRight, FPSInput.BitLookRight);

			ApplyInputButtonBit(_input, Attack1, FPSInput.BitAttack1);
			ApplyInputButtonBit(_input, Attack2, FPSInput.BitAttack2);
		}

		_fpsCtrl.ProcessMovement(_input, delta);
		Main.i.SetDebugText(_fpsCtrl.debugStr);

		_inventory.Tick(
			delta,
			(_input.buttons & FPSInput.BitAttack1) != 0,
			(_input.buttons & FPSInput.BitAttack2) != 0
			);

		_hudState.health = 80;
		_hudState.ammoLoaded = 999;
		_hudState.weaponName = "Stakegun";
		Main.i.ui.SetHudState(_hudState);
	}

	//private void FireProjectile()
	//{
	//	_weapon.FirePrimary();
	//}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		
	}

	public override void _Input(InputEvent @event)
	{
		if (Main.i.gameInputActive == false) { return; }
		InputEventMouseMotion motion = @event as InputEventMouseMotion;
		if (motion == null) { return; }
		_fpsCtrl.ProcessMouseMotion(motion, Main.i.GetWindowToScreenRatio());
	}
}
