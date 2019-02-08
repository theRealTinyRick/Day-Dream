using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay.Components
{
    public enum MoveType
    {
        Vertical,
        Horizontal,
        Freeform
    }

    public class MoveToComponent : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public List<Transform> toPoints = new List<Transform>();

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public GameObject objectToMove;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public float speed;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public MoveType moveType;

        [TabGroup(Tabs.Events)]
        [SerializeField]
        public MoveStartedEvent moveStartedEvent = new MoveStartedEvent();

        [TabGroup(Tabs.Events)]
        [SerializeField]
        public DestinationReachedEvent destinationReachedEvent = new DestinationReachedEvent();

        public bool playOnStart;

        private Coroutine coroutine;

        private void Start()
        {
            if(playOnStart)
            {
                MoveTo(0);
            }
        }

        public void MoveTo(int index)
        {
            if (objectToMove == null)
            {
                return;
            }

            if(index > toPoints.Count - 1)
            {
                return;
            }

            if(coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(MoveCoroutine(toPoints[index].position));
        }

        private IEnumerator MoveCoroutine(Vector3 toPosition)
        {
            if(moveStartedEvent != null)
            {
                moveStartedEvent.Invoke();
            }

            Vector3 _toPosition = toPosition;
            Vector3 _fromPosition = objectToMove.transform.position;

            if(moveType == MoveType.Freeform)
            {

            }
            else if(moveType == MoveType.Horizontal)
            {
                _toPosition.y = objectToMove.transform.position.y;
            }
            else
            {
                _toPosition = toPosition;
                _toPosition.x = objectToMove.transform.position.x;
                _toPosition.z = objectToMove.transform.position.z;
            }

            while (Vector3.Distance(objectToMove.transform.position, _toPosition) > 0.01)
            {
                Vector3 _direction = _toPosition - _fromPosition;

                objectToMove.transform.position += _direction * (speed * Time.deltaTime);

                yield return new WaitForFixedUpdate();
            }

            if(destinationReachedEvent != null)
            {
                destinationReachedEvent.Invoke();
            }

            yield break;
        }
    }
}
