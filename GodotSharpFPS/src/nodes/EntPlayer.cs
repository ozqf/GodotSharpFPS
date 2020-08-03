using Godot;
using System;
using GodotSharpFps.src;
using GodotSharpFps.src.nodes;

public class EntPlayer : Spatial, IActor, IActorProvider
{
	private FPSInput _input;
	private FPSController _fpsCtrl;
	private Spatial _head;
	private ActorInventory _inventory;
	private LaserDot _laserDot;
	private SwordThrowProjectile _thrownSword;
	private HUDPlayerState _hudState;
	private KinematicWrapper _body;
	private int _health = 100;
	private Team _team = Team.Player;

	private int _entId = 0;
	public int ParentActorId { get; set; }

	public IActor GetActor() => this;

	public Team GetTeam() { return _team; }

	public void ActorTeleport(Transform t)
	{
		_body.GlobalTransform = t;
	}

	public Transform GetTransformForTarget() { return _body.GetTransformForTarget(); }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Main m = GetNode<Main>("/root/main");

		_input = new FPSInput();

		_hudState = new HUDPlayerState();
		_hudState.health = 80;
		_hudState.ammoLoaded = 999;
		_hudState.weaponName = "Stakegun";

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

		if (_entId == 0)
		{
			// no id previous set, request one
			_entId = m.game.ReserveActorId(1);
			m.game.RegisterActor(this);
		}

		m.Broadcast(GlobalEventType.PlayerSpawned, this);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
	}

	public override void _PhysicsProcess(float delta)
	{
		// Clear all inputs and reapply
		_input.buttons = 0;

		if (Main.i.gameInputActive)
		{
			_input.ReadGodotInputs();
			// TODO - replace this with stuff in FPSInput
			// bleh, mouse wheel events are ONLY on just release...?
			// https://godotengine.org/qa/30666/how-do-i-get-input-from-the-mouse-wheel
			if (Input.IsActionJustReleased("next_slot"))
			{
				_input.buttons |= FPSInput.BitNextSlot;
			}
			if (Input.IsActionJustReleased("prev_slot"))
			{
				_input.buttons |= FPSInput.BitPrevSlot;
			}
		}

		_fpsCtrl.ProcessMovement(_input, delta);
		Main.i.SetDebugText(_fpsCtrl.debugStr);

		if (_input.isBitOn(FPSInput.BitNextSlot))
		{ _inventory.SelectNextWeapon(); }
		if (_input.isBitOn(FPSInput.BitPrevSlot))
		{ _inventory.SelectPrevWeapon(); }

		if (_input.isBitOn(FPSInput.BitSlot1))
		{ _inventory.SelectWeaponByIndex(0); }
		if (_input.isBitOn(FPSInput.BitSlot2))
		{ _inventory.SelectWeaponByIndex(1); }
		if (_input.isBitOn(FPSInput.BitSlot3))
		{ _inventory.SelectWeaponByIndex(2); }
		if (_input.isBitOn(FPSInput.BitSlot4))
		{ _inventory.SelectWeaponByIndex(3); }

		AttackSource src = new AttackSource();
		src.team = GetTeam();
		src.ignoreBody = _body;
		src.actorId = _entId;

		_inventory.Tick(
			delta,
			_input.isBitOn(FPSInput.BitAttack1),
			_input.isBitOn(FPSInput.BitAttack2),
			src);

		_inventory.FillHudStatus(_hudState);
		_hudState.health = _health;
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
		_fpsCtrl.ProcessMouseMotion(motion, ZqfGodotUtils.GetWindowToScreenRatio());
	}

	public void SetActorId(int newId)
	{
		_entId = newId;
	}

	public int actorId { get { return _entId; } }

	public void ChildActorRemoved(int id) { }

	public TouchResponseData ActorTouch(TouchData touchData)
	{
		
		TouchResponseData result;
		if (!Game.CheckTeamVsTeam(touchData.teamId, _team))
		{
			return TouchResponseData.empty;
		}
		Console.WriteLine($"Player hit for {touchData.damage}");
		result.damageTaken = touchData.damage;
		_health -= touchData.damage;
		if (_health <= 0)
		{
			_team = Team.NonCombatant;
			Main.i.game.DeregisterActor(this);
			Main.i.Broadcast(GlobalEventType.PlayerDied, this);
			QueueFree();
			result.responseType = TouchResponseType.Killed;
		}
		else
		{
			result.responseType = TouchResponseType.Damaged;
		}
		return result;
	}
}
