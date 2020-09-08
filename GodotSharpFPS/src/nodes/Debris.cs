using Godot;

namespace GodotSharpFps.src.nodes
{
    public class Debris : RigidBody
    {
        private bool _active = true;
        private float _timeToLive = 10f;

        public override void _Process(float delta)
        {
            if (_active)
            {
                _timeToLive -= delta;
                if (_timeToLive < 0)
                {
                    _active = false;
                    QueueFree();
                }
            }
        }

        public void LaunchUp()
        {
            AddCentralForce(new Vector3(0, ZqfGodotUtils.RandomRange(30, 50), 0));

            AngularVelocity = new Vector3(
                ZqfGodotUtils.RandomRange(-15, 15),
                ZqfGodotUtils.RandomRange(-5, 5),
                ZqfGodotUtils.RandomRange(-15, 15));
        }
    }
}
