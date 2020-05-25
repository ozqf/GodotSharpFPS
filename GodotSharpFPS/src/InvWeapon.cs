using Godot;
using System;

namespace GodotSharpFps.src
{
    public class InvWeapon
    {
        private Spatial _launchNode;
        private PhysicsBody _ignoreBody;
        private ProjectileDef _projectileDef;

        public InvWeapon(Spatial launchNode, ProjectileDef def, PhysicsBody ignoreBody)
        {
            _launchNode = launchNode;
            _projectileDef = def;
            _ignoreBody = ignoreBody;
        }

        public void Fire()
        {
            PointProjectile prj = Main.instance.factory.SpawnPointProjectile();
            if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }
            prj.Launch(_launchNode.GlobalTransform, _projectileDef, _ignoreBody);
        }
    }
}
