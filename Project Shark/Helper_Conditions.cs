using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Helper_Conditions : MonoBehaviour
{
    public static bool CheckIfComponentIsOnGameObject(GameObject gameObjectToCheck, Behaviour script) => 
        gameObjectToCheck.GetComponent(script.GetType()) != null;
    
    //This determines if the distance between the transforms are less than or equal to a determined value.
    public static bool DistanceBetweenLessThan(Transform posA, Transform posB, float allowance) => 
        Vector2.Distance(posA.position, posB.position) <= allowance;
    
    //This determines if the distance between the vectors are less than or equal to a determined value.
    public static bool DistanceBetweenLessThan(Vector2 posA, Vector2 posB, float allowance) => 
        Vector2.Distance(posA, posB) <= allowance;
    
    //This determines if the distance between the transforms are greater than or equal to a determined value.
    public static bool DistanceBetweenGreaterThan(Transform posA, Transform posB, float allowance) => 
        Vector2.Distance(posA.position, posB.position) > allowance;
    
    //This determines if the distance between the vectors are greater than or equal to a determined value.
    public static bool DistanceBetweenGreaterThan(Vector2 posA, Vector2 posB, float allowance) => 
        Vector2.Distance(posA, posB) > allowance;
    
    public static bool ObjectXPositionGreaterThanOtherObjectXPosition(Transform objectAPosition, Transform objectBPosition) => objectAPosition.position.x >= objectBPosition.position.x; 
    
    public static bool AllTrueCheck(bool[] array) => array.Any(x => !x);
}