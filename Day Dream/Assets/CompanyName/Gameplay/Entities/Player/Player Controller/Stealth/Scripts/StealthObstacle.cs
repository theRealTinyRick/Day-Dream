using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace AH.Max.Gameplay.Stealth
{
    public enum StealthObsacleType
    {
        TallGrass,
        ShortWall,
        Pillar
    }

    public class StealthObstacle : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public Transform[] edges;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public StealthObsacleType stealthObsacleType;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private int numberOfProbesBetweenEdges;
        
        [SerializeField]
        private List <Vector3> hidingPlaces = new List<Vector3>();

        void Start()
        {
            
        }

        void Update()
        {
            
        }

        [Button]
        public void CalculateHidingPlaces()
        {
            hidingPlaces.Clear();

            if(stealthObsacleType == StealthObsacleType.ShortWall)
            {
                for(int _index = 0; _index < edges.Length; _index++)
                {
                    Transform _edgeOne = edges[0];
                    Transform _edgeTwo = edges[1];

                    for(int _count = 0; _count < numberOfProbesBetweenEdges; _count ++)
                    {
                        Debug.Log("alkdsf");
                        hidingPlaces.Add(_edgeTwo.position - _edgeOne.position * (_count));
                    }
                    
                    if(_index == 0 && edges.Length > 1)
                    {
                    }
                }
            }

            if(stealthObsacleType == StealthObsacleType.Pillar)
            {
                // get a spot on all 4 sides
            }

            if(stealthObsacleType == StealthObsacleType.TallGrass)
            {
                // make the player walk towards it to hide
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach(Vector3 _vector in hidingPlaces)
            {
                Gizmos.DrawSphere(_vector, 0.1f);
            }
        }
    }
}
