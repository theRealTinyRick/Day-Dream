using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Damagable : MonoBehaviour
{
    public delegate void Damaged();
    public Damaged OnDamaged;
}