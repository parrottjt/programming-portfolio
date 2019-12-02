using System.Collections;
using System.Collections.Generic;
using Ludiq;
using UnityEngine;

public class AIHelperFunctions : MonoBehaviour
{
    //This will destroy the objects that have the T class.
    public static void ProjectileDestroy<T>() where T : Object
    {
        if (FindObjectsOfType<T>() != null)
        {
            foreach (var t in FindObjectsOfType<T>())
            {
                Destroy(t.GameObject());
            }
        }
    }

    //This calculates the rotation of gameObject towards another gameObject.
    //The rotation this calculates focuses the X-axis toward the target gameObject.
    public static Quaternion RotationLookAt_Right(Transform t1, Transform t2)
    {
        Vector2 vectorToPlayerFlip = t1.position - t2.transform.position;
        float angle = Mathf.Atan2(vectorToPlayerFlip.y, vectorToPlayerFlip.x) * Mathf.Rad2Deg;

        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    //This calculates the rotation of gameObject towards another gameObject.
    //The rotation this calculates focuses the y-axis toward the target gameObject.
    public static Quaternion RotationLookAt_Up(Transform t1, Transform t2)
    {
        Vector2 vectorToPlayerFlip = t1.position - t2.transform.position;
        float angle = Mathf.Atan2(vectorToPlayerFlip.y, vectorToPlayerFlip.x) * Mathf.Rad2Deg;

        return Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    //This determines if the distance between the transforms are less than or equal to a determined value.
    public static bool DistanceBetweenLessThan_Transform(Transform posA, Transform posB, float allowance)
    {
        return Vector2.Distance(posA.position, posB.position) <= allowance;
    }

    //This determines if the distance between the transforms are greater than or equal to a determined value.
    public static bool DistanceBetweenGreaterThan(Transform posA, Transform posB, float allowance)
    {
        return Vector2.Distance(posA.position, posB.position) >= allowance;
    }

    //This determines if the distance between the vectors are less than or equal to a determined value.
    public static bool DistanceBetweenLessThan_Vector2(Vector2 posA, Vector2 posB, float allowance)
    {
        return Vector2.Distance(posA, posB) <= allowance;
    }

    //This determines if the distance between the vectors are greater than or equal to a determined value.
    public static bool DistanceBetweenGreaterThan_Vector2(Vector2 posA, Vector2 posB, float allowance)
    {
        return Vector2.Distance(posA, posB) >= allowance;
    }
}