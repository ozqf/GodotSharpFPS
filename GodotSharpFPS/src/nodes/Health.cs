using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
    public class Health : Area, ITouchable
    {
        private int _health = 100;
        // health reset back to if revived
        private int _maxHealth = 100;
        private bool _dead = false;

        // callbacks
        private HealthChange _onHealthChange = null;
        private Death _onDeath = null;
        
        public void Resurrect()
        {
            _health = _maxHealth;
            _dead = false;
        }
        
        public void SetHealth(int current, int max)
        {
            _health = current;
            _maxHealth = max;
        }

        public void SetCallbacks(HealthChange onHealthChange, Death onDeath)
        {
            _onHealthChange = onHealthChange;
            _onDeath = onDeath;
        }

        public TouchResponseData Touch(TouchData touchData)
        {
            if (_dead) { return TouchResponseData.empty; }

            int previous = _health;
            _health -= touchData.damage;

            TouchResponseData result;
            result.damageTaken = touchData.damage;
            GFXQuick gfx = Main.i.factory.SpawnGFX(GameFactory.Path_GFXBloodImpact);
            gfx.Spawn(touchData.hitPos, touchData.hitNormal);
            if (_health <= 0)
            {
                _dead = true;
                result.responseType = TouchResponseType.Killed;
                _onDeath?.Invoke();
            }
            else
            {
                int change = _health - previous;
                result.responseType = TouchResponseType.Damaged;
                _onHealthChange?.Invoke(_health, change, touchData);
            }

            return result;
        }
    }
}
