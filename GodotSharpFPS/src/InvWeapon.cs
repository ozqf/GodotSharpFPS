using Godot;
using System;

namespace GodotSharpFps.src
{
    public class InvWeapon
    {
        private Spatial _launchNode;
        private ProjectileDef _projectileDef;

        public InvWeapon(Spatial launchNode, ProjectileDef def)
        {
            _launchNode = launchNode;
            _projectileDef = def;
        }

        public void Fire()
        {
            PointProjectile prj = Main.instance.factory.SpawnPointProjectile();
            if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }
            prj.Launch(_launchNode.GlobalTransform, _projectileDef);
        }
    }
}
