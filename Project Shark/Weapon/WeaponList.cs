using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;

public enum WeaponNames
{
    NormalLazer,
    ChargeLazer,
    TrackingLazer,
    SplitLazer,
    TaserLazer,
    RapidFireLazer,
    BounceLazer,
    ShotgunLazer,
    BakaCannon,
    TeethGun,
    
    //Not Story
    BurstLaser,
    FlameThrower,
    PeaShooter,
    SharkRepellent,
}

public class WeaponList : MonoBehaviour
{
    public static Dictionary<WeaponNames, WeaponInfo> weaponDictionary = new Dictionary<WeaponNames, WeaponInfo>();
    
    //This list has to be in order of the WeaponNames Enum
    public static List<IWeapon> storyWeaponTypes = new List<IWeapon>
    {
        new NormalLaser(),
        new ChargeLaser(),
        new TrackingLaser(),
        new SplitLaser(),
        new TaserLaser(),
        new RapidFireLaser(),
        new BounceLaser(),
        new ShotgunLaser(),
        new BakaCannon(),
        new BurstLaser()
    };

    public struct WeaponInfo
    {
        public PlayerWeapons weapon_Info;
        public IWeapon weapon_Function;

        public WeaponInfo(PlayerWeapons _info, IWeapon _function)
        {
            weapon_Info = _info;
            weapon_Function = _function;
        }
    }

    public void Awake()
    {
        foreach (var weaponName in Wehking_EnumUtils.GetValues<WeaponNames>())
        {
            if (Resources.Load($"PlayerWeapons/{weaponName.ToString()}") as PlayerWeapons == null) continue;
            var weapon = Resources.Load($"PlayerWeapons/{weaponName.ToString()}") as PlayerWeapons;
            weaponDictionary.Add(weaponName, new WeaponInfo(weapon, storyWeaponTypes[(int)weaponName]));
        }
    }

    static void PlaySoundEffect(SoundClip clipToPlay)
    {
        if (GameManager.instance.laser.currAmmo[GameManager.instance.laser.weaponIndex] > 0)
        {
            SoundManager.instance.RandomizeSfx(!DevTools.devToolsDictionary[DevTool.KawaiiMode]
                ? clipToPlay
                : SoundManager.instance.KawaiiFunSfx);
        }
    }

    static void DecreaseAmmoByOne()
    {
        GameManager.instance.laser.currAmmo[GameManager.instance.laser.weaponIndex]--;
    }

    static void ResetFireTimer() => GameManager.instance.playerCode.SetFireTimerToZero();

    static bool ShadowShotCalculation()
    {
        if (GameManager.instance.playerCode.GetShadowShotAdjust() > 0)
        {
            return Range(0f, 100.00f) <= GameManager.instance.playerCode.GetShadowShotAdjust() * 100;
        }

        return false;
    }

    public class BaseWeapon
    {
        //Base for all weapons
        protected PlayerWeapons currentWeapon;
        protected bool canFireShadowShot;
        protected float shadowShotTimer;

        protected bool hasCharge, hasFired;
        protected float chargeTimer;

        public float GetChargeTimer() => chargeTimer;
    }
    
    // Easier viewing on inherited scripts for context
    // In structure as seperate scripts
    #region Story IWEAPON Classes

    class NormalLaser : BaseWeapon, IWeapon
    {
        public void EquipWeapon()
        {
            currentWeapon = weaponDictionary[WeaponNames.NormalLazer].weapon_Info;
        }

        public void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null)
        {
            if (fire)
            {
                //print("[Normal Laser] I hit this");
                FireMethod(spawnPositions, shadowShot);
                if (!canFireShadowShot)
                {
                    canFireShadowShot = ShadowShotCalculation();
                }
            }

            if (canFireShadowShot)
            {
                shadowShotTimer += Time.deltaTime;
                if (!(shadowShotTimer >= .15f)) return;
                FireMethod(spawnPositions, true);
                shadowShotTimer = 0;
                canFireShadowShot = false;
            }
        }

        public void FireMethod(Transform[] spawnPositions, bool shadowShot)
        {
            var poolProjectile = ObjectPooling.GetAvailableObject(!shadowShot
                ? currentWeapon.projectile[0]
                : currentWeapon.shadowProjectile[0]);
            poolProjectile.transform.position = spawnPositions[0].position;
            poolProjectile.transform.rotation = spawnPositions[0].rotation;
            poolProjectile.SetActive(true);

            ResetFireTimer();

            PlaySoundEffect(SoundManager.instance.RedLaserSfx);
        }
    }

    class ChargeLaser : BaseWeapon,IWeapon
    {
        public void EquipWeapon()
        {
            currentWeapon = weaponDictionary[WeaponNames.ChargeLazer].weapon_Info;
        }

        public void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null)
        {
            //Convert To Switch
            //Charge
            if (fire && !hasFired)
            {
                if (chargeTimer < 2f)
                {
                    chargeTimer += Time.deltaTime;
                }

                GameManager.instance.playerCode.ChargeSystemOne.SetActive(chargeTimer < 2);
                GameManager.instance.playerCode.ChargeSystemFull.SetActive(chargeTimer >= 2);
                
                hasCharge = true;
            }

            //Fire
            if (!fire && hasCharge && !hasFired)
            {
                FireMethod(spawnPositions, shadowShot);
                
                GameManager.instance.playerCode.ChargeSystemOne.SetActive(false);
                GameManager.instance.playerCode.ChargeSystemFull.SetActive(false);
                canFireShadowShot = ShadowShotCalculation();
                hasFired = true;
            }

            //Recharge
            if (hasFired)
            {
                if (chargeTimer > 0)
                {
                    chargeTimer -= Time.deltaTime * (1 / GameManager.instance.laser.currentWeapon.chargeDecayModifier);
                    hasCharge = false;
                }

                if (chargeTimer <= 0)
                {
                    chargeTimer = 0;
                    hasFired = false;
                }
            }

            if (canFireShadowShot)
            {
                shadowShotTimer += Time.deltaTime;
                if (shadowShotTimer < .15f) return;
                FireMethod(spawnPositions, shadowShot);
                shadowShotTimer = 0;
                canFireShadowShot = false;
            }
        }

        public void FireMethod(Transform[] spawnPositions, bool shadowShot)
        {
            var poolProjectile = ObjectPooling.GetAvailableObject(!shadowShot
                ? currentWeapon.projectile[ChargeLaserProjectileIndexSelector()]
                : currentWeapon.shadowProjectile[ChargeLaserProjectileIndexSelector()]);
            poolProjectile.transform.position = spawnPositions[0].position;
            poolProjectile.transform.rotation = spawnPositions[0].rotation;
            poolProjectile.SetActive(true);

            PlaySoundEffect(SoundManager.instance.ChargeLaserSfx);
        }

        int ChargeLaserProjectileIndexSelector()
        {
            if (chargeTimer < .75f) return 0;
            if (chargeTimer >= .75f && chargeTimer < 1.5f) return 1;
            if (chargeTimer >= 1.5f && chargeTimer < 2f) return 2;
            return 3;
        }
    }

    class TrackingLaser : BaseWeapon,IWeapon
    {
        public void EquipWeapon()
        {
            currentWeapon = weaponDictionary[WeaponNames.TrackingLazer].weapon_Info;
        }

        public void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null)
        {
            if (fire)
            {
                FireMethod(spawnPositions, shadowShot);
                if (!canFireShadowShot)
                {
                    canFireShadowShot = ShadowShotCalculation();
                }
            }

            if (canFireShadowShot)
            {
                shadowShotTimer += Time.deltaTime;
                if (!(shadowShotTimer >= .15f)) return;
                FireMethod(spawnPositions, true);
                shadowShotTimer = 0;
                canFireShadowShot = false;
            }
        }

        public void FireMethod(Transform[] spawnPositions, bool shadowShot)
        {
            var poolProjectile = ObjectPooling.GetAvailableObject(!shadowShot
                ? currentWeapon.projectile[0]
                : currentWeapon.shadowProjectile[0]);
            poolProjectile.transform.position = spawnPositions[0].position;
            poolProjectile.transform.rotation = spawnPositions[0].rotation;
            poolProjectile.SetActive(true);

            DecreaseAmmoByOne();
            ResetFireTimer();

            PlaySoundEffect(SoundManager.instance.RedLaserSfx);
        }
    }

    class SplitLaser : BaseWeapon,IWeapon
    {
        public void EquipWeapon()
        {
            currentWeapon = weaponDictionary[WeaponNames.SplitLazer].weapon_Info;
        }

        public void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null)
        {
            if (fire)
            {
                FireMethod(spawnPositions, shadowShot);
                if (!canFireShadowShot)
                {
                    canFireShadowShot = ShadowShotCalculation();
                }
            }

            if (canFireShadowShot)
            {
                shadowShotTimer += Time.deltaTime;
                if (!(shadowShotTimer >= .15f)) return;
                FireMethod(spawnPositions, true);
                shadowShotTimer = 0;
                canFireShadowShot = false;
            }
        }

        public void FireMethod(Transform[] spawnPositions, bool shadowShot)
        {
            var spawns = spawnPositions[6].GetComponentsInChildren<Transform>();

            foreach (var spawn in spawns)
            {
                var poolProjectile = ObjectPooling.GetAvailableObject(!shadowShot
                    ? currentWeapon.projectile[0]
                    : currentWeapon.shadowProjectile[0]);
                poolProjectile.transform.position = spawn.position;
                poolProjectile.transform.rotation = spawn.rotation;
                poolProjectile.SetActive(true);
            }

            DecreaseAmmoByOne();
            ResetFireTimer();

            PlaySoundEffect(SoundManager.instance.RedLaserSfx);
        }
    }

    class TaserLaser : BaseWeapon,IWeapon
    {
        public void EquipWeapon()
        {
            currentWeapon = weaponDictionary[WeaponNames.TaserLazer].weapon_Info;
        }

        public void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null)
        {
            lineRenderer.enabled = fire;
        }

        public void FireMethod(Transform[] spawnPositions, bool shadowShot)
        {
            throw new NotImplementedException();
        }
    }

    class RapidFireLaser : BaseWeapon,IWeapon
    {
        public void EquipWeapon()
        {
            currentWeapon = weaponDictionary[WeaponNames.RapidFireLazer].weapon_Info;
        }

        public void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null)
        {
            if (fire)
            {
                FireMethod(spawnPositions, shadowShot);
                if (!canFireShadowShot)
                {
                    canFireShadowShot = ShadowShotCalculation();
                }
            }

            if (canFireShadowShot)
            {
                shadowShotTimer += Time.deltaTime;
                if (!(shadowShotTimer >= .15f)) return;
                FireMethod(spawnPositions, true);
                shadowShotTimer = 0;
                canFireShadowShot = false;
            }
        }

        public void FireMethod(Transform[] spawnPositions, bool shadowShot)
        {
            var poolProjectile = ObjectPooling.GetAvailableObject(!shadowShot
                ? currentWeapon.projectile[0]
                : currentWeapon.shadowProjectile[0]);
            poolProjectile.transform.position = spawnPositions[5].position;
            poolProjectile.transform.rotation = spawnPositions[5].rotation;
            poolProjectile.SetActive(true);

            DecreaseAmmoByOne();
            ResetFireTimer();

            PlaySoundEffect(SoundManager.instance.RedLaserSfx);
        }
    }

    class BounceLaser : BaseWeapon,IWeapon
    {
        public void EquipWeapon()
        {
            currentWeapon = weaponDictionary[WeaponNames.BounceLazer].weapon_Info;
        }

        public void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null)
        {
            if (fire)
            {
                FireMethod(spawnPositions, shadowShot);
                if (!canFireShadowShot)
                {
                    canFireShadowShot = ShadowShotCalculation();
                }
            }

            if (canFireShadowShot)
            {
                shadowShotTimer += Time.deltaTime;
                if (!(shadowShotTimer >= .15f)) return;
                FireMethod(spawnPositions, true);
                shadowShotTimer = 0;
                canFireShadowShot = false;
            }
        }

        public void FireMethod(Transform[] spawnPositions, bool shadowShot)
        {
            var poolProjectile = ObjectPooling.GetAvailableObject(!shadowShot
                ? currentWeapon.projectile[0]
                : currentWeapon.shadowProjectile[0]);
            poolProjectile.transform.position = spawnPositions[0].position;
            poolProjectile.transform.rotation = spawnPositions[0].rotation;
            poolProjectile.SetActive(true);

            DecreaseAmmoByOne();
            ResetFireTimer();

            PlaySoundEffect(SoundManager.instance.RedLaserSfx);
        }
    }

    class ShotgunLaser : BaseWeapon,IWeapon
    {
        public void EquipWeapon()
        {
            currentWeapon = weaponDictionary[WeaponNames.ShotgunLazer].weapon_Info;
        }

        public void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null)
        {
            if (fire)
            {
                FireMethod(spawnPositions, shadowShot);
                if (!canFireShadowShot)
                {
                    canFireShadowShot = ShadowShotCalculation();
                }
            }

            if (canFireShadowShot)
            {
                shadowShotTimer += Time.deltaTime;
                if (!(shadowShotTimer >= .15f)) return;
                FireMethod(spawnPositions, true);
                shadowShotTimer = 0;
                canFireShadowShot = false;
            }
        }

        public void FireMethod(Transform[] spawnPositions, bool shadowShot)
        {
            for (int i = 0; i < 5; i++)
            {
                var poolProjectile = ObjectPooling.GetAvailableObject(!shadowShot
                    ? currentWeapon.projectile[0]
                    : currentWeapon.shadowProjectile[0]);
                poolProjectile.transform.position = spawnPositions[i].position;
                poolProjectile.transform.rotation = spawnPositions[i].rotation;
                poolProjectile.SetActive(true);
            }

            DecreaseAmmoByOne();
            ResetFireTimer();

            PlaySoundEffect(SoundManager.instance.RedLaserSfx);
        }
    }

    class BakaCannon : BaseWeapon,IWeapon
    {
        public void EquipWeapon()
        {
            currentWeapon = weaponDictionary[WeaponNames.BakaCannon].weapon_Info;
        }

        public void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null)
        {
            if (fire && !hasFired)
            {
                if (chargeTimer < currentWeapon.fireTime)
                {
                    chargeTimer += Time.deltaTime;
                }

                hasCharge = true;
            }

            if (!fire && hasCharge && !hasFired && chargeTimer >= currentWeapon.fireTime)
            {
                FireMethod(spawnPositions, shadowShot);
                canFireShadowShot = ShadowShotCalculation();
                hasFired = true;
            }

            if (hasFired)
            {
                if (chargeTimer > 0)
                {
                    chargeTimer -= Time.deltaTime * (1 / GameManager.instance.laser.currentWeapon.chargeDecayModifier);
                    hasCharge = false;
                }

                if (chargeTimer <= 0)
                {
                    chargeTimer = 0;
                    hasFired = false;
                }
            }

            if (canFireShadowShot)
            {
                shadowShotTimer += Time.deltaTime;
                if (shadowShotTimer < .15f) return;
                FireMethod(spawnPositions, shadowShot);
                shadowShotTimer = 0;
                canFireShadowShot = false;
            }
        }

        public void FireMethod(Transform[] spawnPositions, bool shadowShot)
        {
            var poolProjectile = ObjectPooling.GetAvailableObject(!shadowShot
                ? currentWeapon.projectile[0]
                : currentWeapon.shadowProjectile[0]);
            poolProjectile.transform.position = spawnPositions[0].position;
            poolProjectile.transform.rotation = spawnPositions[0].rotation;
            poolProjectile.SetActive(true);

            PlaySoundEffect(SoundManager.instance.ChargeLaserSfx);
        }
    }

    #endregion

    #region Other IWEAPON Classes

    class BurstLaser : BaseWeapon,IWeapon
    {
        float burstTimer;
        int fireCount;

        public void EquipWeapon()
        {
            currentWeapon = weaponDictionary[WeaponNames.BurstLaser].weapon_Info;
        }

        public void Execute(bool fire, Transform[] spawnPositions, bool shadowShot, LineRenderer lineRenderer = null)
        {
            //This will have to be updated
            if (fire)
            {
                burstTimer += Time.deltaTime;
                if (fireCount < 3 && burstTimer > .13f)
                {
                    FireMethod(spawnPositions, shadowShot);
                    fireCount++;
                    if (!canFireShadowShot)
                    {
                        canFireShadowShot = ShadowShotCalculation();
                    }
                }
            }

            if (canFireShadowShot)
            {
                shadowShotTimer += Time.deltaTime;
                if (!(shadowShotTimer >= .15f)) return;
                FireMethod(spawnPositions, true);
                shadowShotTimer = 0;
                canFireShadowShot = false;
            }
        }

        public void FireMethod(Transform[] spawnPositions, bool shadowShot)
        {
            var poolProjectile = ObjectPooling.GetAvailableObject(!shadowShot
                ? currentWeapon.projectile[0]
                : currentWeapon.shadowProjectile[0]);
            poolProjectile.transform.position = spawnPositions[0].position;
            poolProjectile.transform.rotation = spawnPositions[0].rotation;
            poolProjectile.SetActive(true);

            ResetFireTimer();
            DecreaseAmmoByOne();

            PlaySoundEffect(SoundManager.instance.RedLaserSfx);
        }
    }
    
    #endregion
}
