using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper_Methods : MonoBehaviour
{
    public delegate void EmptyVoidDelegate();
    EmptyVoidDelegate emptyVoidDelegate;
    
    public static void SetActiveOnGameObjects(bool value, params GameObject[] gameObjects)
    {
        foreach (var g in gameObjects)
        {
            g.SetActive(value);
        }
    }

    public static void SetActiveOnGameObjects(bool value, params GameObject[][] gameObjects)
    {
        foreach (var gArray in gameObjects)
        {
            SetActiveOnGameObjects(value, gArray);
        }
    }
    public static void SetEnableOnBehaviorComponentTo(bool value, params Behaviour[] behaviours)
    {
        foreach (var type in behaviours)
        {
            type.enabled = value;
        }
    }
    public static void SetEnableOnBehaviorArrayComponentTo(bool value, params Behaviour[][] behaviors)
    {
        foreach (var behaviour in behaviors)
        {
            SetEnableOnBehaviorComponentTo(value, behaviour);
        }
    }
    public static void SetEnableOnRendererComponentTo(bool value, params Renderer[] renderers)
    {
        foreach (var r in renderers)
        {
            r.enabled = value;
        }
    }
    public static void SetEnableOnRendererArrayComponentTo(bool value, params Renderer[][] renderers)
    {
        foreach (var renderer in renderers)
        {
            SetEnableOnRendererComponentTo(value, renderer);
        }
    }
}