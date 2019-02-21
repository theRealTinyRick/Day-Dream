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

        public void SetStateFalse(string stateName)
        {
            if (states.ContainsKey(stateName))
            {
                states[stateName] = false;

                if (stateChangedEvent != null)
                {
                    stateChangedEvent.Invoke(stateChangedEvent, stateName);
                }
            }
        }

        public void SetStateTrue(string stateName)
        {
            if (states.ContainsKey(stateName))
            {
                states[stateName] = true;

                if (stateChangedEvent != null)
                {
                    stateChangedEvent.Invoke(stateChangedEvent, stateName);
                }
            }
        }

        public void ReverseState(string stateName)
        {
            if (states.ContainsKey(stateName))
            {
                states[stateName] = !states[stateName];

                if (stateChangedEvent != null)
                {
                    stateChangedEvent.Invoke(stateChangedEvent, stateName);
                }
            }
        }

        public bool AnyStateTrue()
        {
            foreach (string _state in states.Keys)
            {
                if (states[_state])
                {
                    return true;
                }
            }

            return false;
        }

        public bool AnyStateTrue(List<string> states)
        {
            foreach(string _state in states)
            {
                if(this.states[_state])
                {
                    return true;
                }
            }

            return false;
        }

        public bool AnyStateFalse()
        {
            foreach (string _state in states.Keys)
            {
                if (!states[_state])
                {
                    return true;
                }
            }

            return false;
        }

        public bool AnyStateFalse(List<string> states)
        {
            foreach (string _state in states)
            {
                if (this.states.ContainsKey(_state))
                {
                    if (!this.states[_state])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

