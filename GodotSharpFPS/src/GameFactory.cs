using Godot;
using GodotSharpFps.src;
using GodotSharpFps.src.nodes;
using System.Collections.Generic;
using System.Linq;

public class GameFactory
{
    // Projectiles
    public const string Path_PointProjectile = "res://game/projectiles/point_projectile.tscn";
    public const string Path_StakeProjectile = "res://game/projectiles/stake_projectile.tscn";
    public const string Path_ThrownSword = "res://game/projectiles/prj_thrown_sword.tscn";

    // GFX
    public const string Path_GFXImpact = "res://gfx/gfx_impact.tscn";
    public const string Path_GFXBulletImpact = "res://gfx/gfx_bullet_impact.tscn";
    public const string Path_GFXBloodImpact = "res://gfx/gfx_blood_impact.tscn";

    // Player
    private const string Path_EntPlayer = "game/player/ent_player.tscn";

    // Mob scenes
    public const string Path_EntMob = "game/mob/ent_mob.tscn";
    public const string Path_EntMobPinkie = "game/mob/ent_mob_pinkie.tscn";
    public const string Path_EntMobTitan = "game/mob/ent_mob_titan.tscn";

    // Mob types
    public const string MobType_Humanoid = "Humanoid";
    public const string MobType_Pinkie = "Pinkie";
    public const string MobType_Titan = "Titan";

    // mob class -> prefab lookup
    private Dictionary<string, MobDef> _mobTypes = new Dictionary<string, MobDef>();

    private Node _root;
    
    public GameFactory(Spatial root)
    {
        _root = root;
        _mobTypes.Add(GameFactory.MobType_Humanoid, new MobDef()
        {
            mobTypeName = GameFactory.MobType_Humanoid,
            prefabPath = GameFactory.Path_EntMob,
            defaultHealth = 200,
            evadeRange = 10,
            walkSpeed = 8,
            friction = 4,
            accelForce = 25,
            pushMultiplier = 1
        });
        _mobTypes.Add(GameFactory.MobType_Pinkie, new MobDef()
        {
            mobTypeName = GameFactory.MobType_Pinkie,
            prefabPath = GameFactory.Path_EntMobPinkie,
            defaultHealth = 200,
            evadeRange = 6,
            walkSpeed = 10,
            friction = 6,
            accelForce = 25,
            pushMultiplier = 1
        });
        _mobTypes.Add(GameFactory.MobType_Titan, new MobDef()
        {
            mobTypeName = GameFactory.MobType_Titan,
            prefabPath = GameFactory.Path_EntMobTitan,
            defaultHealth = 4000,
            evadeRange = 30,
            walkSpeed = 5.5f,
            stuntime = 1,
            // very hard to push around
            friction = 12,
            accelForce = 200,
            pushMultiplier = 0.1f
        });
    }

    public string[] GetMobTypeList()
    {
        return _mobTypes.Keys.ToArray();
    }

    public MobDef GetMobType(string mobTypeName)
    {
        if (!_mobTypes.ContainsKey(mobTypeName))
        {
            return _mobTypes[GameFactory.MobType_Humanoid];
        }
        return _mobTypes[mobTypeName];
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

    public EntMob SpawnMob(string mobTypeName = "")
    {
        MobDef mobType = GetMobType(mobTypeName);
        // Pass a null parent as we need to init the mob before adding it to the tree
        // which will invoke _ready
        EntMob mob = ZqfGodotUtils.CreateInstance<EntMob>(mobType.prefabPath, null);
        mob.SetMobType(mobType);
        _root.AddChild(mob);
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
