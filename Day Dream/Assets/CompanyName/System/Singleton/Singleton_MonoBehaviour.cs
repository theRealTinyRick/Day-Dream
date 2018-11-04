using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max
{
    public class Singleton_MonoBehavior <T> : MonoBehaviour where T : class
    {
        public static T _instance;
        public static T Instance
        {
            get 
            {
                return _instance;
            }
        }

        private void Awake()
        {
            // Debug.Log(gameObject.name);
            // if(_instance == null)
            // {
            //     _instance = gameObject;
            // }
        }
    }
}
