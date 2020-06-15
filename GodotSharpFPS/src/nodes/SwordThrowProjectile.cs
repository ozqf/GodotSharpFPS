using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src.nodes
{
    public class SwordThrowProjectile
    {
        const float ThrowRecallDelay = 0.25f;
        const float RecallFinishDistance = 2;
        const float ThrowSpeedGuided = 40;
        const float ThrowSpeed = 75;
        const float RecallSpeed = 150;
        const float MaxRecallTime = 2;

        private Area _entTouchBody;
        private Spatial _displayNode;

        private float _throwDamage = 100;
        private Node _worldParent;
        private Node _attachParent;
    }
}
