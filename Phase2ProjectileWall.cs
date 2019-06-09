using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2ProjectileWall : MonoBehaviour
{

    public GameObject spawnedProj;
    public int numberOfProj, numberOfHoles, additvePosition;
    public float spacingY, wallSpawnTime;

    public int overridePos1, overridePos2, numberOfWalls;

    public bool noHoleInWall;

    int positionIndex, lastOverride1, wallBlockerIndex;

    float wallSpawnTimer;

    // Use this for initialization

    void Start()
    {
        positionIndex = overridePos1;
        lastOverride1 = overridePos1;
    }

    void Update()
    {
        if (lastOverride1 != overridePos1)
        {
            positionIndex = overridePos1;
            lastOverride1 = overridePos1;
        }

        wallSpawnTimer += Time.deltaTime;
        if (wallSpawnTimer >= wallSpawnTime)
        {
            overridePos1 += additvePosition;
            if (!noHoleInWall)
            {
                for (int i = -numberOfProj / 2; i < numberOfProj / 2 + 1; i++)
                {

                    if (i != positionIndex)
                    {
                        Vector3 newPos = new Vector3(transform.position.x,
                            transform.position.y + spacingY * i);
                        var proj = Instantiate(spawnedProj, newPos, transform.rotation);
                        proj.transform.parent = transform;
                        wallSpawnTimer = 0;
                    }

                    if (i == positionIndex && positionIndex < overridePos1 + numberOfHoles - 1)
                    {
                        positionIndex++;
                        if (positionIndex >= overridePos1 + numberOfHoles - 1)
                        {
                            positionIndex = overridePos1;
                        }
                    }
                }
            }

            if (noHoleInWall)
            {
                for (int i = -numberOfProj / 2; i < numberOfProj / 2 + 1; i++)
                {
                    Vector3 newPos = new Vector3(transform.position.x,
                        transform.position.y + spacingY * i);
                    var proj = Instantiate(spawnedProj, newPos, transform.rotation);
                    proj.transform.parent = transform;
                    wallSpawnTimer = 0;
                    
                }
                wallBlockerIndex++;
                if (wallBlockerIndex >= numberOfWalls)
                {
                    noHoleInWall = false;
                    wallBlockerIndex = 0;
                }
            }
        }
    }
}
