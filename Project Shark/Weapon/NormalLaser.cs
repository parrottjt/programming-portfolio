using Helpers;
using UnityEngine;

public class NormalLaser : BaseWeapon, IWeapon
{
    public override WeaponName WeaponName => WeaponName.NormalLazer;

    public void EquipWeapon()
    {
        weaponInfo = WeaponList.weaponDictionary[WeaponName.NormalLazer].weapon_Info;
    }

    public void ExecuteBasicFunctionality(bool fire, GameObject projectile, Transform[] spawnPositions, out bool shadowShot,
        LineRenderer lineRenderer = null)
    {
        shadowShot = false;
        if (fire)
        {
            NormalFireMethod(projectile, spawnPositions);
            shadowShot = ShadowShotCalculation();
        }
    }

    public void ExecuteUpgradeFunctionality(bool fire, GameObject projectile, Transform[] spawnPositions, out bool shadowShot,
        LineRenderer lineRenderer = null)
    {
        shadowShot = false;
        if (CurrentWeaponLevel() == 1) return;
        if (fire)
        {
            UpgradeFireMethod(projectile, spawnPositions);
            shadowShot = ShadowShotCalculation();
        }
    }

    public override void NormalFireMethod(GameObject projectile, Transform[] spawnPositions)
    {
        Handlers.FireProjectile(projectile, spawnPositions[0]);

        ResetFireTimer();

        PlaySoundEffect(SoundManager.instance.RedLaserSfx);
    }

    public override void UpgradeFireMethod(GameObject projectile, Transform[] spawnPositions)
    {
        Handlers.FireProjectile(projectile, spawnPositions[0]);

        ResetFireTimer();

        PlaySoundEffect(SoundManager.instance.RedLaserSfx);
    }
}