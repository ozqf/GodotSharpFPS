using Godot;
using System;

public class GameCamera : Camera
{
    public void AttachToTarget(Spatial newParent)
    {
        //Transform t = GlobalTransform;
        Node parent = GetParent();
        parent.RemoveChild(this);
        newParent.AddChild(this);
        //GlobalTransform = t;
    }
}
