using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AH.Max.Gameplay.AI;

namespace AH.Max
{
    public class EntityManager : Singleton<EntityManager>
    {
        private Entity _player;
        public Entity Player
        {
            get
            { 
                if(!_player)
                    Debug.LogError( "The player has not initialized or is not present in the scene." );
                    
                return _player; 
            }
            private set { _player = value; }
        }

        private List <Entity> _aiEntities = new List<Entity>(); 
        public List <Entity> AIEntities
        {
            get { return _aiEntities; }
            private set { _aiEntities = value; }
        }

        private List <IEnemy> _enemies = new List <IEnemy> ();
        public List <IEnemy> Enemies
        {
            get { return _enemies; }
        }

        public void SetPlayer( Entity player )
        {
            _player = player;
        }

        public void AddAIEntity( Entity _entity )
        {
            _aiEntities.Add(_entity);
        }   

        public void AddEnemy ( IEnemy _entity )
        {
            _enemies.Add(_entity);
        }

        public void RemoveAIEntity( Entity _entity )
        {
            _aiEntities.Remove(_entity);
        }
    }
}
