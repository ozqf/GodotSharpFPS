using Godot;
using Godot.Collections;

public class LaserDot : Spatial
{
    private Spatial _originNode;
    private float _length = 1000;
    public void SetOriginNode(Spatial origin)
    {
        _originNode = origin;
    }

    public override void _PhysicsProcess(float delta)
    {
        Transform t = _originNode.GlobalTransform;
        Vector3 origin = t.origin;
        Vector3 forward = -t.basis.z;
        Vector3 dest = origin + (forward * _length);
        Vector3 finalPos = dest;
        uint mask = uint.MaxValue;
        PhysicsDirectSpaceState space = GetWorld().DirectSpaceState;
        Dictionary hitResult = space.IntersectRay(origin, dest, null, mask);
        
        if (hitResult.Keys.Count > 0)
        {
            finalPos = (Vector3)hitResult["position"];
        }
        Transform selfT = GlobalTransform;
        selfT.origin = finalPos;
        GlobalTransform = selfT;
    }
}
