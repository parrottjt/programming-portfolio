using UnityEngine;

public class Helper_AI : MonoBehaviour
{
    public static Quaternion RotationLookAt_XAxis(Transform t1, Transform t2)
    {
        return Quaternion.AngleAxis(GetRotationAngle_Vector2(t1.position, t2.position), Vector3.forward);
    }

    //This calculates the rotation of gameObject towards another gameObject.
    //The rotation this calculates focuses the y-axis toward the target gameObject.
    public static Quaternion RotationLookAt_YAxis(Transform t1, Transform t2)
    {
        return Quaternion.AngleAxis(GetRotationAngle_Vector2(t1.position, t2.position) - 90, Vector3.forward);
    }

    public static float GetRotationAngle_Vector2(Vector2 v1, Vector2 v2)
    {
        Vector2 vectorToPlayerFlip = v1 - v2;
        return Mathf.Atan2(vectorToPlayerFlip.y, vectorToPlayerFlip.x) * Mathf.Rad2Deg;
    }

    public static void FireProjectile_Enemy(GameObject projectile, Transform[] spawnPoint)
    {
        foreach (var t in spawnPoint)
        {
            var proj = ObjectPooling.GetAvailableObject(projectile);
            proj.transform.position = t.position;
            proj.transform.rotation = t.rotation;
            proj.SetActive(true);
        }
    }

    public static void FireProjectile_Enemy(GameObject projectile, Transform spawnPoint)
    {
        var proj = ObjectPooling.GetAvailableObject(projectile);
        proj.transform.position = spawnPoint.position;
        proj.transform.rotation = spawnPoint.rotation;
        proj.SetActive(true);
    }

    public static void ProjectileDestroy<T>() where T : Object
    {
        if (FindObjectsOfType<T>() == null) return;
        foreach (var t in FindObjectsOfType<T>())
        {
            var gameObject = t as GameObject;
            if(gameObject != null) gameObject.SetActive(false);
        }
    }
}