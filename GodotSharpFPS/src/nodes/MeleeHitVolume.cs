﻿using Godot;
using System;
using System.Collections.Generic;

namespace GodotSharpFps.src.nodes
{
    public class MeleeHitVolume: Area
    {
        private int _ticks = 0;
        private int _damage = 15;
        private bool _on = false;
        private CollisionShape _shape;
        private MeshInstance _debugMesh;
        private List<IActor> _hits = new List<IActor>(10);
        private AttackSource _source = new AttackSource();

        public override void _Ready()
        {
            base._Ready();
            _shape = GetNode<CollisionShape>("CollisionShape");
            _debugMesh = GetNode<MeshInstance>("debug_cube_1x1x1");
            Connect("body_entered", this, "OnBodyEntered");
            Off();
        }

        public void SetDamage(int dmg)
        {
            _damage = dmg;
        }

        private void PerformHits()
        {
            Console.WriteLine($"Performing {_hits.Count} melee hits");
            TouchData touch = new TouchData();
            touch.damage = _damage;
            touch.teamId = _source.team;
            touch.touchType = TouchType.Melee;
            for (int i = 0; i < _hits.Count; ++i)
            {
                IActor a = _hits[i];
                Vector3 origin = _source.ignoreBody.GlobalTransform.origin;
                Vector3 dest = a.GetTransformForTarget().origin;
                Godot.Collections.Dictionary hit =
                    ZqfGodotUtils.CastRay(this, origin, dest, uint.MaxValue, _source.ignoreBody);
                if (hit.Keys.Count == 0) { continue; }

                touch.hitPos = (Vector3)hit["position"];
                touch.hitNormal = (Vector3)hit["normal"];
                Console.WriteLine($"Hitting actor {a.actorId}");
                TouchResponseData response = Game.TouchGameObject(touch, hit["collider"]);
                //a.ActorTouch(touch);
            }
            _hits.Clear();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_hits.Count > 0)
            {
                PerformHits();
            }
            if (!_on)
            {
                return;
            }
            if (_ticks >= 1)
            {
                Off();
                return;
            }
            _ticks++;
        }

        public void OnBodyEntered(Node body)
        {
            if (body == _source.ignoreBody)
            {
                Console.WriteLine($"Melee hit self");
                return;
            }
            IActor a = Game.ExtractActor(body);
            if (a == null) { return; }
            _hits.Add(a);
        }

        /*
        Manage life time:
        */

        private void On()
        {
            _on = true;
            _ticks = 0;
            _shape.Disabled = false;
            _debugMesh.Show();
        }

        private void Off()
        {
            _on = false;
            _shape.Disabled = true;
            _debugMesh.Hide();
        }

        public void Fire(AttackSource src)
        {
            Console.WriteLine($"Melee weapon fire");
            _source = src;
            On();
        }

    }
}
