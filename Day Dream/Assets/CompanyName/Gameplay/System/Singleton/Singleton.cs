using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max
{
    public abstract class Singleton <T> where T : class, new ()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
    }
}
