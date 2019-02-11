using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Type", menuName = "CompanyName/WeaponType", order = 1)]
public class WeaponType : SerializedScriptableObject
{
    public string[] animations;
    public float clickTime;
    public Handedness handedness;
}
