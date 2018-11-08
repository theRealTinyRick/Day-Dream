using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AH.Max.Gameplay.AI;
using AH.Max.System;

namespace AH.Max.System
{
    public class EntityManager : Singleton<EntityManager>
    {
        private Entity _player;
        public Entity Player
        {
            get
            { 
                if(!_player)
                {
                    Entity _player = FindPlayer();

                    if(_player == null)
                    {
                        Debug.LogError( "The player has not initialized or is not present in the scene." );
                        return null;
                    }
                }
                return _player; 
            }
            private set { _player = value; }
        }

        private List <Entity> _enemies = new List <Entity> ();
        public List <Entity> Enemies
        {
            get { return _enemies; }
        }

        private List <Entity> entities = new List <Entity>();
        public List <Entity> Entities
        {
            get { return entities; }
        }

        public void RegisterEntity( Entity entity )
        {
            if(!entities.Contains(entity))
            {
                entities.Add(entity);
            }

            switch (entity.IdentityType.type)
            {
                case IdentityTypes.Player:
                    _player = entity;
                    break;

                case IdentityTypes.Enemy:
                    if(!_enemies.Contains(entity))
                    {
                        _enemies.Add(entity);
                    }
                    break;
            }
        }

        public void RemoveEntity( Entity entity )
        {
            if(entities.Contains(entity))
            {
                entities.Remove(entity);
            }

            if(_player == entity)
            {
                _player = null;
            }

            if(_enemies.Contains(entity))
            {
                _enemies.Remove(entity);
            }
        }

        private Entity FindPlayer()
        {
            foreach(Entity e in entities)
            {
                if(e.IdentityType.type == IdentityTypes.Player)
                {
                    return e;
                }
            }

            return null;
        }
    }
}
