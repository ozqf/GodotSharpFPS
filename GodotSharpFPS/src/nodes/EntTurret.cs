using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src.nodes
{
    public class EntTurret : Spatial
    {
        private ProjectileDef _prjDef;
        private PatternDef _patternDef;

        private float _tick = 1;
        private float _tickMax = 0.5f;
        private List<Transform> _transforms = null;

        public override void _Ready()
        {
            _prjDef = new ProjectileDef();
            _prjDef.damage = 10;
            _prjDef.launchSpeed = 100;
            _prjDef.prefabPath = GameFactory.Path_PointProjectile;
            _prjDef.moveMode = ProjectileDef.MoveMode.Accel;
            _prjDef.maxSpeed = 100;
            _prjDef.minSpeed = 10;
            _prjDef.accelPerSecond = -300;

            _patternDef = new PatternDef();
            _patternDef.count = 8;
            _patternDef.patternType = PatternType.HorizontalLine;
            _patternDef.scale = new Vector3(8, 8, 0);

            _transforms = new List<Transform>(_patternDef.count);

            IActor actorParent = Game.DescendTreeToActor(this);
            Console.WriteLine($"Turret found parent actor {actorParent.actorId}");
        }

        private void Shoot()
        {
            SpawnPatterns.FillPattern(GlobalTransform, _patternDef, _transforms);
            for (int i = 0; i < _transforms.Count; ++i)
            {
                PointProjectile prj = Main.i.factory.SpawnPointProjectile();
                prj.Launch(_transforms[i].origin, -_transforms[i].basis.z, _prjDef, null, Team.Mobs);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            IActor actor = Main.i.game.CheckTarget(0, Team.Mobs);
            if (actor == null) { return; }

            LookAt(actor.GetTransformForTarget().origin, Vector3.Up);

            if (_tick <= 0)
            {
                _tick = _tickMax;
                Shoot();
            }
            else
            {
                _tick -= delta;
            }
        }
    }
}
