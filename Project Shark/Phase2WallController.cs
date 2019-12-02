using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Phase2WallController : MonoBehaviour
{
    [SerializeField] GameObject wall;
    Phase2ProjectileWall wallScript;
    public bool tutOver;

    [Header("Wall Setup")]
    [SerializeField] WallTravs[] wallTraversals;
    [SerializeField] WaveMovement[] tutorialTraversals;
    WaveMovement[] currenTraversal;

    int count, wallTraversalInt, wallKey;

    [Serializable]
    public struct WallTravs
    {
        public string name;
        public WaveMovement[] wallTraversals;
    }

    //On created set variables that will be needed as soon as the script becomes created.
    void Awake()
    {
        wallScript = wall.GetComponent<Phase2ProjectileWall>();
        wallKey = Random.Range(0, wallTraversals.Length);
        currenTraversal = tutOver ? wallTraversals[0].wallTraversals : tutorialTraversals;
    }

    void Start()
    {
        //Subscribe to the Tick Event
        TickTimeTimer.OnTick += OnTick;
    }

    // Update is called once per frame
    void Update()
    {
        //Keep the projectile wall up to date with the current state of the variables.  
        wallScript.additvePosition = currenTraversal[wallTraversalInt].additivePos1;
        wallScript.additivePosition2 = currenTraversal[wallTraversalInt].additivePos2;
        wallScript.numberOfHoles = currenTraversal[wallTraversalInt].numOfHoles;
        wallScript.split = currenTraversal[wallTraversalInt].split;
        wallScript.wall = currenTraversal[wallTraversalInt].createWall;

        if (currenTraversal[wallTraversalInt].numOfPosChanges == count)
        {
            wallTraversalInt++;
            count = 0;
            if (wallTraversalInt >= currenTraversal.Length)
            {
                wallTraversalInt = 0;
                WallChangeFunctionality();
            }
        }
    }

    //If the first static wall has ended turn off the the set wall.
    //Call the random range and set the current traversal to the new wall traversal.
    void WallChangeFunctionality()
    {
        if (!tutOver)
        {
            tutOver = true;
        }
        wallKey = Random.Range(0, wallTraversals.Length);
        currenTraversal = wallTraversals[wallKey].wallTraversals;
    }

    //What done on the Tick Event
    void OnTick(object sender, TickTimeTimer.OnTickEventArgs args)
    {
        count++;
    }

    //Unsubscribe from the Tick Event to remove garbage and handle performance better.
    void OnDisable()
    {
        TickTimeTimer.OnTick -= OnTick;
    }
}