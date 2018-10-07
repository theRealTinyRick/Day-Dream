using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max
{
    public abstract class Singleton_MonoBehavior <T> : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get // NOT DONE NOT DONE NOT
            {
                Debug.LogError( "The Singleton_Monobehaviour class is not finished. Please don't make a reference to it" );
                if(_instance == null)
                {
                    var newGameObject = new GameObject();
                    // newGameObject.AddComponent
                }

                return _instance;
            }
        }
    }
}
