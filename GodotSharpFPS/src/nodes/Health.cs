using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
    public class Health : Area, ITouchable
    {
        [Export]
        public int health = 100;
        // health reset back to if revived
        [Export]
        public int maxHealth = 100;
        [Export]
        public Team teamId = Team.None;

        private bool _dead = false;

        // callbacks
        private HealthChange _onHealthChange = null;
        private Death _onDeath = null;
        
        public void Resurrect()
        {
            health = maxHealth;
            _dead = false;
        }
        
        public void OverrideStats(Team newTeamId, int current, int max)
        {
            teamId = newTeamId;
            health = current;
            maxHealth = max;
        }

        public void SetCallbacks(HealthChange onHealthChange, Death onDeath)
        {
            _onHealthChange = onHealthChange;
            _onDeath = onDeath;
        }

        public TouchResponseData Touch(TouchData touchData)
        {
            if (_dead || !Game.CheckTeamVsTeam(touchData.teamId, teamId))
            { return TouchResponseData.empty; }

            Console.WriteLine($"Health {teamId} hurt by team {touchData.teamId}");
            int previous = health;
            health -= touchData.damage;

            TouchResponseData result;
            result.damageTaken = touchData.damage;
            GFXQuick gfx = Main.i.factory.SpawnGFX(GameFactory.Path_GFXBloodImpact);
            gfx.Spawn(touchData.hitPos, touchData.hitNormal);
            if (health <= 0)
            {
                _dead = true;
                result.responseType = TouchResponseType.Killed;
                _onDeath?.Invoke();
            }
            else
            {
                int change = health - previous;
                result.responseType = TouchResponseType.Damaged;
                _onHealthChange?.Invoke(health, change, touchData);
            }

            return result;
        }
    }
}
