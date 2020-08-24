using Godot;
using System;
using GodotSharpFps.src;
using GodotSharpFps.src.nodes;
using GodotSharpFps.src.extended;

public class EntPlayer : Spatial, IActor, IActorProvider
{
	private ViewModel _viewModel;

	private Main _main;
	private FPSInput _input;
	private FPSController _fpsCtrl;
	private Spatial _head;
	private ViewModel _handsPlaceholder;
	private ViewModel _gunPlaceholder;
	private MeleeHitVolume _meleeVolume;
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
	public void ActorTeleport(Transform t) { _body.GlobalTransform = t; }
	public Transform GetTransformForTarget() { return _body.GetTransformForTarget(); }
	public void RemoveActor() { this.QueueFree(); }

	public string GetActorDebugText()
	{
		return _fpsCtrl.debugStr;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//_main = GetNode<Main>("/root/main");
		_main = Main.i;

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
		_meleeVolume = _head.GetNode<MeleeHitVolume>("melee_hit_volume");

		///////////////////////////////////////
		// view models

		// grab hands placeholder and attach it to the head node
		_handsPlaceholder = GetNode<ViewModel>("hands_placeholder");
		Transform t = _handsPlaceholder.GlobalTransform;
		RemoveChild(_handsPlaceholder);
		_head.AddChild(_handsPlaceholder);
		_handsPlaceholder.GlobalTransform = t;
		_handsPlaceholder.SetEnabled(false);

		// same for placeholder gun
		_gunPlaceholder = GetNode<ViewModel>("view_placeholder_gun");
		ZqfGodotUtils.SwapSpatialParent(_gunPlaceholder, _head);
		_gunPlaceholder.SetEnabled(true);
		_viewModel = _gunPlaceholder;

		_laserDot = GetNode<LaserDot>("laser_dot");
		_laserDot.CustomInit(_head, uint.MaxValue, 1000);

		_thrownSword = _main.factory.SpawnThrownSword(false);
		_main.AddOrphanNode(_thrownSword);

		// init components
		_fpsCtrl = new FPSController(_body, _head);


		// Inventory
		_inventory = new ActorInventory();
		_inventory.Init(_head, 1);

		// Add weapons
		SwordThrowProjectile prj = _main.factory.SpawnThrownSword();
		_inventory.AddWeapon(AttackFactory.CreatePlayerMelee(_meleeVolume, prj, _laserDot));
		_inventory.AddWeapon(AttackFactory.CreatePlayerShotgun(_head, _body));
		_inventory.AddWeapon(AttackFactory.CreateStakegun(_head, _body));
		_inventory.AddWeapon(AttackFactory.CreateLauncher(_head, _body));
		_inventory.AddWeapon(new InvWeapGodhand(_head, _laserDot));
		_inventory.SelectWeaponByIndex(1);

		_main.cam.AttachToTarget(_head, GameCamera.ParentType.Player);

		if (_entId == 0)
		{
			// no id previous set, request one
			_entId = _main.game.ReserveActorId(1);
			_main.game.RegisterActor(this);
		}

		_main.Broadcast(GlobalEventType.PlayerSpawned, this);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
	}

	public override void _Input(InputEvent @event)
	{
		if (Main.i.gameInputActive == false) { return; }
		InputEventMouseMotion motion = @event as InputEventMouseMotion;
		if (motion == null) { return; }
		_fpsCtrl.ProcessMouseMotion(motion, ZqfGodotUtils.GetWindowToScreenRatio());
	}

	public override void _PhysicsProcess(float delta)
	{
		if (Main.i.gameInputActive) { _input.ReadGodotInputs(); }
		else { _input.Clear(); }

		_fpsCtrl.ProcessMovement(_input, delta);
		//Main.i.SetDebugText(_fpsCtrl.debugStr);

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
		if (_input.isBitOn(FPSInput.BitSlot5))
		{ _inventory.SelectWeaponByIndex(4); }
		if (_input.isBitOn(FPSInput.BitSlot6))
		{ _inventory.SelectWeaponByIndex(5); }
		if (_input.isBitOn(FPSInput.BitSlot7))
		{ _inventory.SelectWeaponByIndex(6); }
		if (_input.isBitOn(FPSInput.BitSlot8))
		{ _inventory.SelectWeaponByIndex(7); }

		AttackSource src = new AttackSource();
		src.team = GetTeam();
		src.ignoreBody = _body;
		src.actorId = _entId;

		bool secondaryOnToggled = _input.hasBitToggledOn(FPSInput.BitAttack2);
		
		EquippableTickInfo info = new EquippableTickInfo(
			_input.isBitOn(FPSInput.BitAttack1),
			_input.hasBitToggledOn(FPSInput.BitAttack1),
			_input.isBitOn(FPSInput.BitAttack2),
			_input.hasBitToggledOn(FPSInput.BitAttack2),
			src);

		_inventory.Tick(delta, info);

		_inventory.FillHudStatus(_hudState);
		_hudState.health = _health;
		if (_viewModel != null)
		{
			_viewModel.SetViewModelState(_hudState.view);
		}
		Main.i.ui.SetHudState(_hudState);
	}

	public void SetActorId(int newId)
	{
		_entId = newId;
	}

	public int actorId { get { return _entId; } }

	public void ChildActorRemoved(int id) { }

#if false
	public TouchResponseData ActorTouch(TouchData touchData)
	{
		
		TouchResponseData result;
		if (!Game.CheckTeamVsTeam(touchData.teamId, _team))
		{
			return TouchResponseData.empty;
		}
		Console.WriteLine($"Player hit for {touchData.damage}");
		result.damageTaken = touchData.damage;
		if (!_main.game.GodModeOn())
		{
			_health -= touchData.damage;
		}
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
#endif
}
