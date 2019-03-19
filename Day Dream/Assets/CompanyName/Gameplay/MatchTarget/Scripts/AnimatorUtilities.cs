using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorUtilites
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="animator"> The animator that you are calling this on </param>
    /// <param name="position"> position to move your character to </param>
    /// <param name="rotation"> rotation you what to move your character to </param>
    /// <param name="normalizedStartTime"> the normalized start time in the animation you want to   </param>
    /// <param name="normalizedStopTime"></param>
    /// <param name="positionSpeed">  </param>
    /// <param name="rotationSpeed">  </param>
    public static void MatchTarget(Animator animator, HumanBodyBones bodyPart, string animationName, Transform characterTransform, Vector3 position, Quaternion rotation, float normalizedStartTime, float normalizedStopTime, float positionSpeed, float rotationSpeed)
    {
        AnimatorStateInfo _state = animator.GetCurrentAnimatorStateInfo(0);

        if(_state.IsName(animationName) && _state.normalizedTime >= normalizedStartTime && _state.normalizedTime < normalizedStopTime)
        {
            Vector3 _rootOffset = animator.GetBoneTransform(bodyPart).position - characterTransform.position;
            Vector3 _targetPosition = position - _rootOffset;

            Debug.DrawRay(position, -_rootOffset, Color.red, 10);

            characterTransform.position = Vector3.Lerp(characterTransform.position, _targetPosition, positionSpeed);
            characterTransform.rotation = Quaternion.RotateTowards(characterTransform.rotation, rotation, rotationSpeed);
        }
    }
}
