using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay.System.Components
{
    public class StateComponent : SerializedMonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public Dictionary<string, bool> states = new Dictionary<string, bool>();

        [TabGroup(Tabs.Events)]
        [SerializeField]
        StateChangedEvent stateChangedEvent = new StateChangedEvent();

        public bool GetState(string stateName)
        {
            bool _stateValue;

            if(states.TryGetValue(stateName, out _stateValue))
            {
                return _stateValue;
            }

            Debug.LogWarning("The state component could not find the state: " + stateName + ", so we are refturning false ", gameObject);
            return false;
        }

        public bool GetOrCreateState(string stateName, bool defaultValue = false)
        {
            bool _stateValue;

            if (states.TryGetValue(stateName, out _stateValue))
            {
                return _stateValue;
            }

            states.Add(stateName, defaultValue);
            return defaultValue;
        }

        public bool HasState(string stateName)
        {
            if (states.ContainsKey(stateName))
            {
                return true;
            }

            return false;
        }

        public bool SetState(string stateName, bool value)
        {
            if(states.ContainsKey(stateName))
            {
                states[stateName] = value;

                if(stateChangedEvent != null)
                {
                    stateChangedEvent.Invoke(stateChangedEvent, stateName);
                }

                return true;
            }

            return false;
        }

        public bool SetStateFalse(string stateName)
        {
            if (states.ContainsKey(stateName))
            {
                states[stateName] = false;

                if (stateChangedEvent != null)
                {
                    stateChangedEvent.Invoke(stateChangedEvent, stateName);
                }

                return true;
            }

            return false;
        }

        public bool SetStateTrue(string stateName)
        {
            if (states.ContainsKey(stateName))
            {
                states[stateName] = true;

                if (stateChangedEvent != null)
                {
                    stateChangedEvent.Invoke(stateChangedEvent, stateName);
                }

                return true;
            }

            return false;
        }
    }
}

