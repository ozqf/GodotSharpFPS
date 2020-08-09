using Godot;
using GodotSharpFps.src.nodes;
using System.Collections.Generic;

public class GameFactory
{
    public const string Path_PointProjectile = "res://game/projectiles/point_projectile.tscn";
    public const string Path_StakeProjectile = "res://game/projectiles/stake_projectile.tscn";

    public const string Path_ThrownSword = "res://game/projectiles/prj_thrown_sword.tscn";

    public const string Path_GFXImpact = "res://gfx/gfx_impact.tscn";
    public const string Path_GFXBulletImpact = "res://gfx/gfx_bullet_impact.tscn";
    public const string Path_GFXBloodImpact = "res://gfx/gfx_blood_impact.tscn";

    // Mobs
    private const string Path_EntMob = "game/ent_mob.tscn";
    private const string Path_EntMobPinkie = "game/ent_mob_pinkie.tscn";
    private const string Path_EntPlayer = "game/player/ent_player.tscn";

    public const string MobType_Humanoid = "Humanoid";
    public const string MobType_Pinkie = "Pinkie";

    // mob class -> prefab lookup
    private Dictionary<string, string> _mobTypes = new Dictionary<string, string>
    {
        { MobType_Humanoid, Path_EntMob },
        { MobType_Pinkie, Path_EntMobPinkie }
    };

    private Node _root;
    
    public GameFactory(Spatial root)
    {
        _root = root;
    }

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

    public EntPlayer SpawnPlayer()
    {
        EntPlayer plyr = ZqfGodotUtils.CreateInstance<EntPlayer>(Path_EntPlayer, _root);
        return plyr;
    }

    public EntMob SpawnMob(string mobType = "")
    {
        string path = Path_EntMob;
        if (_mobTypes.ContainsKey(mobType))
        {
            path = _mobTypes[mobType];
        }
        EntMob mob = ZqfGodotUtils.CreateInstance<EntMob>(path, _root);
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
        return result;
    }

    #endregion
}
