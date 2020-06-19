using Godot;
using System;
using GodotSharpFps.src;
using GodotSharpFps.src.nodes;

public class EntPlayer : Spatial, IActor, IActorProvider
{
	private FPSInput _input = new FPSInput();
	private FPSController _fpsCtrl;
	private Spatial _head;
	private ActorInventory _inventory;
	private LaserDot _laserDot;
	private SwordThrowProjectile _thrownSword;
	private HUDPlayerState _hudState = new HUDPlayerState();
	private KinematicWrapper _body;

	public IActor GetActor() => this;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Main m = GetNode<Main>("/root/main");

		// find Godot scene nodes
		_body = GetNode<KinematicWrapper>("actor_base");
		_body.actor = this;
		_body.HideModels();

		_head = GetNode<Spatial>("actor_base/head");

		_laserDot = GetNode<LaserDot>("laser_dot");
		_laserDot.CustomInit(_head, uint.MaxValue, 1000);

		_thrownSword = m.factory.SpawnThrownSword(false);
		m.AddOrphanNode(_thrownSword);

		// init components
		_fpsCtrl = new FPSController(_body, _head);

		// Inventory
		_inventory = new ActorInventory();
		_inventory.Init(_head, 1);

		// Add weapons
		_inventory.AddWeapon(AttackFactory.CreatePlayerShotgun(_head, _body));
		_inventory.AddWeapon(AttackFactory.CreateStakegun(_head, _body));

		m.cam.AttachToTarget(_head);
	}

	public override void _PhysicsProcess(float delta)
	{
		// Clear all inputs and reapply
		_input.buttons = 0;

		if (Main.i.gameInputActive)
		{
			_input.ReadGodotInputs();
		}

		_fpsCtrl.ProcessMovement(_input, delta);
		Main.i.SetDebugText(_fpsCtrl.debugStr);

		_inventory.Tick(
			delta,
			_input.isBitOn(FPSInput.BitAttack1),
			_input.isBitOn(FPSInput.BitAttack2)
			);

		_hudState.health = 80;
		_hudState.ammoLoaded = 999;
		_hudState.weaponName = "Stakegun";
		Main.i.ui.SetHudState(_hudState);
	}

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

	public TouchResponseData ActorTouch(TouchData touchData)
	{
		Console.WriteLine($"Player hit for {touchData.damage}");
		TouchResponseData result;
		result.damageTaken = touchData.damage;
		result.responseType = TouchResponseType.Damaged;
		return result;
	}
}
