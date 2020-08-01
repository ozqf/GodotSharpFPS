using Godot;
using Godot.Collections;
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

    private Node _root;
    private int _nextEntId = 1;

    //private Dictionary<int, IActor> _ents = new Dictionary<int, IActor>();
    private List<IActor> _ents = new List<IActor>();

    public GameFactory(Spatial root)
    {
        _root = root;
    }

    public IActor GetActor(int id)
    {
        // Sigh, are dictionaries broken here? ffs...
        // https://github.com/godotengine/godot/issues/36351
        //if (_ents.ContainsKey(id))
        //{
        //    return _ents[id];
        //}
        //return null;
        return _ents.First(x => x.actorId == id);
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

        Console.WriteLine($"Register actor {actor.actorId}");
        _ents.Add(actor);
        //_ents.Add(actor.actorId, actor);
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
        //_ents.Remove(actor.actorId);
        _ents.Remove(actor);
    }

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
        mob.SetActorId(ReserveActorId(1));
        RegisterActor(mob);
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
}
