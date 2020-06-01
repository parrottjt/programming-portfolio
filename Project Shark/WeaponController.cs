using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void EquipWeapon();
    //basically Update
    void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null);
    void FireMethod(Transform[] spawnPositions, bool shadowShot);
}

public class WeaponController
{
    IWeapon currentWeapon { get; set; }

    public WeaponList.BaseWeapon CurrentWeapon => currentWeapon as WeaponList.BaseWeapon;
    public IWeapon CurrentIWeapon => currentWeapon;
    
    public Transform[] projectileSpawns;

    public void ChangeWeapon(IWeapon newWeapon)
    {
        currentWeapon = newWeapon;
        currentWeapon.EquipWeapon();
    }

    public void ExecuteOperations(bool fire, LineRenderer lineRenderer = null) => 
        currentWeapon?.Execute(fire, projectileSpawns, false, lineRenderer);

    //This are for if individual uses are needed
    public void FireWeapon(bool shadowShot) => currentWeapon?.FireMethod(projectileSpawns, shadowShot);
}