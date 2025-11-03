using JetBrains.Annotations;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Collider : MonoBehaviour
{
    public enum Type // enum to assign collider types to objects in scene
    {
        POINT,
        AXIS_ALIGNED_RECTANGLE, // used for floor
        CIRCLE //used for collider ball and cannon ball
    }

    public Type type;
}