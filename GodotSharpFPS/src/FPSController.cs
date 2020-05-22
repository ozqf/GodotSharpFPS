using Godot;

namespace GodotSharpFps.src
{
	public class FPSController
	{
		private KinematicBody _body;

		public FPSController(KinematicBody body)
		{
			_body = body;
		}
		
    	public void ProcessMovement(FPSInput input, float delta)
    	{
    	    //Console.WriteLine($"Buttons {input.buttons} delta {delta}");
    	    Transform t = _body.GlobalTransform;
    	    Vector3 move = new Vector3();
    	    if ((input.buttons & FPSInput.BitMoveForward) != 0) { move.z -= 1; }
    	    if ((input.buttons & FPSInput.BitMoveBackward) != 0) { move.z += 1; }
    	    if ((input.buttons & FPSInput.BitMoveLeft) != 0) { move.x -= 1; }
    	    if ((input.buttons & FPSInput.BitMoveRight) != 0) { move.x += 1; }
	
    	    move *= (5 * delta);
    	    t.origin += move;
    	    _body.GlobalTransform = t;

    	}

	}
}
