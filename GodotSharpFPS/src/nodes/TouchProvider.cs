using Godot;

namespace GodotSharpFps.src.nodes
{
    public class TouchProvider : Spatial, ITouchProvider
    {
        private ITouchable _touchable;

        public ITouchable GetTouchable()
        {
            if (_touchable == null)
            {
                // if no touchable set, assume parent is.
                // force cast so if this is wrong we know!
                return (ITouchable)GetParent();
            }
            return _touchable;
        }

        public void SetTouchable(ITouchable touchable)
        { _touchable = touchable; }
    }
}
