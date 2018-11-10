using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AH.Max.Gameplay.AI;
using AH.Max.System;

namespace AH.Max.System
{
    public class EntityManager : Singleton<EntityManager>
    {
        ///<Summary>
        /// A reference to the player entity in the scene.
        ///</Summary>
        private Entity _player;

        ///<Summary>
        /// A reference to the player entity in the scene.
        ///</Summary>
        public Entity Player
        {
            get
            { 
                if(!_player)
                {
                    Entity _player = FindPlayer();

                    if(_player == null)
                    {
                        Debug.LogError( "The player has not initialized or is not present in the scene. You may also want to make sure that your player object has the indentity type: Player." );
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

        ///<Summary>
        /// Returns a list of entities of a given Identity.
        ///</Summary>
        public static List <Entity> GetEntities( IdentityType type )
        {
            List <Entity> _entities = new List <Entity>();

            foreach(Entity e in Instance.entities)
            {
                if(e.IdentityType.type == type.type)
                {
                    _entities.Add(e);
                }
            }

            return _entities;
        }

        ///<Summary>
        ///Returns an entity of the given type. The first one found will returned.
        ///</Summary>
        public static Entity GetEntity( IdentityType type )
        {
            foreach(Entity e in Instance.entities)
            {
                if(e.IdentityType.type == type.type)
                {
                    return e;
                }
            }
            return null;
        }
    }
}
