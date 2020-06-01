using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FiringController : AbstractBehavior
{
    PlayerWeaponInventory laser;
    readonly WeaponController weaponController = new WeaponController();
    
    [SerializeField] Transform[] laserPositions;
    [SerializeField] LineRenderer taserLineRenderer;
    
    [SerializeField] GameObject taserSpriteHolder;
    
    bool emptySoundPlay;

    float fireTime;
    float fireTimer;
    
    //Ticks
    int emptySoundTick;

    public bool ReloadCheck() => fireTimer < fireTime;

    public float GetFireTimer => fireTimer;
    public float GetChargeTimer => weaponController.CurrentWeapon.GetChargeTimer();

    bool CanThePlayerFire()
    {
        return GeneralFireCheck() && FireTypeConditions() && ProGenConditionsCheck();
    }

    public void DecreaseAmmoByOne()
    {
        laser.currAmmo[laser.weaponIndex]--;
    }
    public void ResetFireTimeToZero() => fireTimer = 0;

    bool GeneralFireCheck() =>
        !general.GetIsDashing() && !general.isDeadSharky && !general.weaponWheel &&
        !GameManager.gameSettings[Settings.Paused] && playerInput.GetFireInput();

    bool FireTypeConditions()
    {
        switch (laser.currentWeapon.fireType)
        {
            case PlayerWeapons.FireFunctionType.Click:
                return fireTimer >= fireTime && laser.currAmmo[laser.weaponIndex] > 0;
            case PlayerWeapons.FireFunctionType.Charge :
                return true;
            case PlayerWeapons.FireFunctionType.Hold:
                return laser.currAmmo[laser.weaponIndex] > 0;
            default:
                return false;
        }
    }

    bool ProGenConditionsCheck()
    {
        if (!StatManager.IsInitialized)
        {
            return true;
        }
        return !StatManager.instance.GetAquaholic()
               || (StatManager.instance.GetAquaholic() && general.GetIsMoving());
    }
    void Start()
    {
        TickTimeTimer.OnTick += OnTick;
        laser = GameManager.instance.laser;
        weaponController.projectileSpawns = laserPositions;
        weaponController.ChangeWeapon(WeaponList.weaponDictionary[laser.currentWeapon.weaponName].weapon_Function);
    }

    void Update()
    {
        fireTimer += Time.deltaTime;
        GetVariableInformationForCurrentWeapon();
        taserSpriteHolder.SetActive(laser.currentWeapon.weaponName == WeaponNames.TaserLazer);
        weaponController.ChangeWeapon(WeaponList.weaponDictionary[laser.currentWeapon.weaponName].weapon_Function);
        weaponController.ExecuteOperations(CanThePlayerFire(), taserLineRenderer);
    }

    void OnDestroy()
    {
        TickTimeTimer.OnTick -= OnTick;
    }

    void GetVariableInformationForCurrentWeapon()
    {
        fireTime = UpdatedStatManager.GetStat(GameEnums.PermanentStats.ReloadCooldownAdjust).GetStatValue(laser.currentWeapon.fireTime);
        //print(fireTime);
        if (laser.currAmmo[laser.weaponIndex] <= 0)
        {
            if (emptySoundPlay)
            {
                SoundManager.instance.RandomizeSfx(!DevTools.devToolsDictionary[DevTool.KawaiiMode]
                    ? SoundManager.instance.NoAmmoSfx
                    : SoundManager.instance.KawaiiEmptySfx);
                emptySoundPlay = false;
            }
        }
    }

    void OnTick(object sender, TickTimeTimer.OnTickEventArgs args)
    {
        laserPositions[5].localRotation = Quaternion.Euler(0f, 0f, Random.Range(-7.5f, 7.5f));
        //Empty Ammo Sound Play
        if (!emptySoundPlay)
        {
            emptySoundTick++;
            if (emptySoundTick >= 5)
            {
                emptySoundTick = 0;
                emptySoundPlay = true;
            }
        }
    }
}