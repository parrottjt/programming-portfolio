using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2ProjectileWall : MonoBehaviour
{

    public GameObject spawnedProj;
    public int numberOfProj, numberOfHoles, additvePosition, additivePosition2;
    public float spacingY;

    public int overridePos1, overridePos2, numberOfWalls;

    public bool wall, split;

    public int positionIndex, wallBlockerIndex;
    int GetTopLimit()
    {
        return numberOfProj / 2;
    }

    // Use this for initialization
    void Start()
    {
        TickTimeTimer.OnTick += OnTick;
    }

    void OnTick(object sender, TickTimeTimer.OnTickEventArgs args)
    {
        overridePos1 += additvePosition;
        overridePos2 += additivePosition2;
        if (!wall)
        {
            for (int i = -numberOfProj / 2; i < numberOfProj / 2 + 1; i++)
            {
                if (!split)
                {
                    CreateProjectilesPath1_NoWall(i);
                }
                else CreateProjectilesPath2_NoWall(i);
            }
        }

        if (wall)
        {
            for (int i = -numberOfProj / 2; i < numberOfProj / 2 + 1; i++)
            {
                Vector3 newPos = new Vector3(transform.position.x,
                    transform.position.y + spacingY * i);
                var proj = Instantiate(spawnedProj, newPos, transform.rotation);
                proj.transform.parent = transform;

            }
            wall = false;
        }
    }

    void CreateProjectilesPath1_NoWall(int i)
    {
        if (i > overridePos1 + numberOfHoles - 2 || i < overridePos1)
        {
            Vector3 newPos = new Vector3(transform.position.x,
                transform.position.y + spacingY * i);
            var proj = Instantiate(spawnedProj, newPos, transform.rotation);
            proj.transform.parent = transform;
        }
    }

    void CreateProjectilesPath2_NoWall(int i)
    {
        if ((i > overridePos1 + numberOfHoles - 2 || i < overridePos1) && (i > overridePos2 + numberOfHoles - 2 || i < overridePos2))
        {
            Vector3 newPos = new Vector3(transform.position.x,
                transform.position.y + spacingY * i);
            var proj = Instantiate(spawnedProj, newPos, transform.rotation);
            proj.transform.parent = transform;
        }
    }

    void OnDisable()
    {
        TickTimeTimer.OnTick -= OnTick;
    }
}
