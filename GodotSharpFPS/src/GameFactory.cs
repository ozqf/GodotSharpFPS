using Godot;
using System;

public class GameFactory
{
    const string Path_PointProjectile = "res://game/projectiles/point_projectile.tscn";
    
    public PointProjectile SpawnPointProjectile()
    {
        Spatial parent = Main.instance;
        return ZqfGodotUtils.CreateInstance<PointProjectile>(Path_PointProjectile, parent);
    }
}
