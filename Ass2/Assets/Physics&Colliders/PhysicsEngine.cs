using JetBrains.Annotations;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsEngine : MonoBehaviour
{
    [SerializeField] private Projectile Projectile;
    bool CollisionDestionDestroy = false;
    void Update() // runs every frame as a physics engine checking for collisions and updating positions and velocities
    {
        UpdateVelocities();
        UpdatePositions();
        Collisions();
    }



    void UpdateVelocities() // function looking after updating velocities of all physics bodies
    {
        PhysicsBody[] bodies = GameObject.FindObjectsOfType<PhysicsBody>();
        foreach (PhysicsBody b in bodies)
        {
            b.velocity += b.acceleration * Time.deltaTime;
            b.velocity += b.gravity * Time.deltaTime;
        }
    }

    void UpdatePositions() // function looking after updating positions of all physics bodies
    {
        PhysicsBody[] bodies = GameObject.FindObjectsOfType<PhysicsBody>();
        foreach (PhysicsBody b in bodies)
        {
            b.gameObject.transform.position += b.velocity * Time.deltaTime;
        }
    }


    void Collisions() // function looking after detecting and responding to collisions between colliders
    {
        Collider[] colliders = GameObject.FindObjectsOfType<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider a = colliders[i];
            for (int j = i + 1; j < colliders.Length; j++)
            {
                Collider b = colliders[j];

                bool pointToRect = (a.type == Collider.Type.POINT && b.type == Collider.Type.AXIS_ALIGNED_RECTANGLE ||
                    b.type == Collider.Type.POINT && a.type == Collider.Type.AXIS_ALIGNED_RECTANGLE);


                if (pointToRect) // if one collider is a point and the other is an axis-aligned rectangle
                {
                    Collider point = a.type == Collider.Type.POINT ? a : b;
                    Collider aaRect = b.type == Collider.Type.AXIS_ALIGNED_RECTANGLE ? b : a;

                    float width = aaRect.transform.localScale.x;
                    float height = aaRect.transform.localScale.y;

                    float lhs = aaRect.transform.position.x - (width / 2f);
                    float rhs = aaRect.transform.position.x + (width / 2f);
                    float top = aaRect.transform.position.y + (height / 2f);
                    float bot = aaRect.transform.position.y - (height / 2f);

                    bool onLHS = (point.transform.position.x < lhs);
                    bool onRHS = (point.transform.position.x > rhs);
                    bool below = (point.transform.position.y < bot);
                    bool above = (point.transform.position.y > top);

                    if (onLHS || onRHS || above || below) continue;
                    //Debug.Log("collision detected");
                    {
                        
                    }
                }

                bool circleToCircle = (a.type == Collider.Type.CIRCLE && b.type == Collider.Type.CIRCLE);
                if (circleToCircle) // if both colliders are circles
                {
                    float aRad = a.transform.localScale.x / 2f;
                    float bRad = b.transform.localScale.x / 2f;
                    Vector3 aToB = b.transform.position - a.transform.position;

                    float intersectionDepth = (aRad + bRad) - aToB.magnitude;
                    if (intersectionDepth > 0)
                    {
                        Vector3 bToA = a.transform.position - b.transform.position;
                        Vector3 bToAN = bToA.normalized;
                        Vector3 aRes = bToAN * intersectionDepth;
                        a.transform.position += aRes;

                        Vector3 aToBN = aToB.normalized;
                        Vector3 bRes = aToBN * intersectionDepth;
                        b.transform.position += bRes;

                        //print("circle to circle collision detected");
                        Destroy(a.gameObject, 4f); // after 4 seconds destroy both objects
                    }
                }

                bool circleToRect = (a.type == Collider.Type.CIRCLE && b.type == Collider.Type.AXIS_ALIGNED_RECTANGLE ||
                                     a.type == Collider.Type.AXIS_ALIGNED_RECTANGLE && b.type == Collider.Type.CIRCLE);
                if (circleToRect) // if one collider is a circle and the other is an axis-aligned rectangle, did prevents ball going through the floor
                {
                    Collider circle = a.type == Collider.Type.CIRCLE ? a : b;
                    Collider aaRect = b.type == Collider.Type.AXIS_ALIGNED_RECTANGLE ? b : a;

                    float radius = circle.gameObject.transform.localScale.x / 2.0f;
                    Vector3 circlePos = circle.transform.position;
                    Vector3 rectPos = aaRect.transform.position;
                    float halfWidth = aaRect.transform.localScale.x / 2.0f;
                    float halfHeight = aaRect.transform.localScale.y / 2.0f;

                    float closestX = Mathf.Clamp(circlePos.x, rectPos.x - halfWidth, rectPos.x + halfWidth);
                    float closestY = Mathf.Clamp(circlePos.y, rectPos.y - halfHeight, rectPos.y + halfHeight);
                    Vector3 closestPoint = new Vector3(closestX, closestY, circlePos.z);

                    Vector3 circleToClosest = closestPoint - circlePos;
                    if (circleToClosest.sqrMagnitude < radius * radius)
                    {
                        //print("CIRCLE TO RECTANGLE COLLISION DETECTED");
                        Destroy(circle.gameObject); // deletes the circle on collision with rectangle floor
                    }
                }
            }
        }
    }
}
