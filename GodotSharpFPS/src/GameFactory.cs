using Godot;
using GodotSharpFps.src;
using GodotSharpFps.src.nodes;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameFactory
{
    public const string Path_PointProjectile = "res://game/projectiles/point_projectile.tscn";
    public const string Path_StakeProjectile = "res://game/projectiles/stake_projectile.tscn";

    public const string Path_ThrownSword = "res://game/projectiles/prj_thrown_sword.tscn";

    public const string Path_GFXImpact = "res://gfx/gfx_impact.tscn";
    public const string Path_GFXBulletImpact = "res://gfx/gfx_bullet_impact.tscn";

    private Node _root;
    private int _nextEntId = 1;

    // teehee check this is a .net dictionary and not a godot dictionary...
    private Dictionary<int, IActor> _ents = new Dictionary<int, IActor>();

    public GameFactory(Spatial root)
    {
        _root = root;
        Main.i.console.AddCommand("actors", "", "Print actor list", Cmd_PrintActorRegister);

        Main.i.AddObserver(ObserveGlobalEvent, this, false, "GameFactory");
    }

    public void ObserveGlobalEvent(GlobalEventType type, object obj)
    {
        if (type == GlobalEventType.MapChange)
        {
            Console.WriteLine($"GameFactory - clearing Actor register");
            _ents.Clear();
            _nextEntId = 1;
        }
    }

    #region Actor Register

    public bool Cmd_PrintActorRegister(string command, string[] tokens)
    {
        Console.WriteLine($"=== Actor Register ===");
        foreach(int key in _ents.Keys)
        {
            IActor a = _ents[key];
            Console.WriteLine($"{key} - {a} - {a.GetType()}");
        }
        return true;
    }

    public IActor GetActor(int id)
    {
        if (_ents.ContainsKey(id))
        {
            return _ents[id];
        }
        return null;
    }

    public int ReserveActorId(int numIdsToReserve)
    {
        if (numIdsToReserve <= 0) { throw new ArgumentException(nameof(numIdsToReserve)); }
        int result = _nextEntId;
        _nextEntId += numIdsToReserve;
        return result;
    }

    public void RegisterActor(IActor actor)
    {
        if (actor.actorId == 0)
        { actor.SetActorId(ReserveActorId(1)); }
        //if (_ents.ContainsKey(actor.actorId))
        //{ throw new ArgumentException($"Actor Id {actor.actorId} already registered"); }

        Console.WriteLine($"Register actor {actor.actorId} - {actor}");
        _ents.Add(actor.actorId, actor);
    }

    public void DeregisterActor(IActor actor)
    {
        Console.WriteLine($"Deregister actor {actor.actorId}");
        if (actor.ParentActorId != 0)
        {
            Console.WriteLine($"Check parent {actor.ParentActorId}");
            IActor parent = GetActor(actor.ParentActorId);
            // parent might have been removed...
            if (parent != null)
            {
                parent.ChildActorRemoved(actor.actorId);
            }
        }
        _ents.Remove(actor.actorId);
    }

    #endregion

    #region Spawning

    private Node SelectParent(bool addToTree, Node overrideParent)
    {
        Node parent = null;
        if (addToTree)
        {
            if (overrideParent != null)
            { parent = overrideParent; }
            else { parent = _root; }
        }
        return parent;
    }

    public EntMob SpawnMob()
    {
        string path = "game/ent_mob.tscn";
        EntMob mob = ZqfGodotUtils.CreateInstance<EntMob>(path, _root);
        //mob.SetActorId(ReserveActorId(1));
        //RegisterActor(mob);
        return mob;
    }

    public PointProjectile SpawnProjectile(string path, bool addToTree = true, Node overrideParent = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            path = Path_PointProjectile;
        }
        Node parent = SelectParent(addToTree, overrideParent);
        return ZqfGodotUtils.CreateInstance<PointProjectile>(path, parent);
    }

    public GFXQuick SpawnGFX(string path, bool addToTree = true, Node overrideParent = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            path = Path_PointProjectile;
        }
        Node parent = SelectParent(addToTree, overrideParent);
        return ZqfGodotUtils.CreateInstance<GFXQuick>(path, parent);
    }

    public PointProjectile SpawnPointProjectile(bool addToTree = true, Node overrideParent = null)
    {
        Node parent = SelectParent(addToTree, overrideParent);
        return ZqfGodotUtils.CreateInstance<PointProjectile>(Path_PointProjectile, parent);
    }

    public SwordThrowProjectile SpawnThrownSword(bool addToTree = true, Node overrideParent = null)
    {
        Node parent = SelectParent(addToTree, overrideParent);
        SwordThrowProjectile result = ZqfGodotUtils.CreateInstance<SwordThrowProjectile>(Path_ThrownSword, parent);
        // RegisterEnt() // not currently an actor.
        return result;
    }

    #endregion
}
