using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;
using UnityEngine.SceneManagement;

/// GeneralSharkBehavior Explanation
/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
/// This instance gathers information from the scripts that require inputs
/// and sends the information to the GameManager for use in the other 
/// scripts that require this information
/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

public class GeneralSharkBehavior : AbstractBehavior
{
    #region Inspector
    [Header("Animation Holders")]
    [SerializeField]
    GameObject oil;
    [SerializeField]
    GameObject stun, death, minioil;

    [Header("Particle Systems")]
    [SerializeField]
    GameObject frozenSystem;
    public GameObject chargeStage1;
    public GameObject chargeStage2, addHealth,
        addAmmoShotty, addAmmoBurst, addAmmoCharge,
        addAmmoAll, dashRight, dashLeft, moveReminder, poisoned;

    [Header("Times For Timers")]
    [SerializeField]
    float freezeTime;
    [SerializeField]
    float poisonTime;
    [SerializeField]
    float stunTime, respawnTime;

    [Header("Bools")]
    [ReadOnly] public bool isDeadSharky = false;
    [SerializeField]
    [ReadOnly] bool inCurrent;

    public LayerMask mask;

    //Speeds for everything
    [Header("Speeds For Everything")]
    [SerializeField]
    [ReadOnly] float speed;
    [SerializeField] [ReadOnly] float normSpeed, dashSpeed, slowSpeedPercent, tempSpeed, stopSpeed;
    #endregion

    //Scripts that this instance will get information from
    #region Scripts
    FaceDirection face;
    SharkMovement movement;
    SharkDash dash;
    ShootingInput shooting;
    Wehking_CameraShake camShake;
    Item_PlayerModifier itemModifers;
    #endregion

    #region Other Components
    [HideInInspector]
    public SpriteRenderer weaponHolder;
    #endregion

    #region Floats
    //Original Speeds for stat effects
    float origNorm, origDash, origSlowPercent, origAgainTime, origLinearDrag , originalGrav;

    //Timers
    float timer, oilTimer, miniOilTimer, stunTimer, healthTimer, ammoTimer, freezeTimer, poisonTimer;
    float frozenSoundTimer = 1f;

    //Floats for variables from other scripts
    float lookAngle, againTime;

    //Modifers, make sure they are set because errors won't happen.
    float fireAdjustTime = 1, moveAdjustSpeed = 1, dashForceAdjust = 1, linearDragAdjust = 1,
        angularDragAdjust = 1;
    #endregion

    #region Bools
    [HideInInspector]
    public bool weaponWheel, hookRight, hookLeft, hookUp, hookDown;
    bool oiled, slowed, stunned, frozen, miniOiled, damaging;
    bool dashTut, isDashing;

    //Pro-Gen Bools - Haven't Been Added to the set functions
    bool scrunchShoot, playerCollisionDamage, flameDash, ammoRegen, noShootNoAmbientDamage, oneHit, scrunchHealthRegen,
        scrunchNoAmbient, dashDontUseAmmo;
    #endregion

    #region Get And Set Functions
    #region Get Bools
    public bool GetIsDashing()
    {
        return isDashing;
    }
    public bool TutDashCheck()
    {
        return dashTut;
    }
    public bool isRight()
    {
        if (face.GetLookAngle() <= 90f && face.GetLookAngle() >= -90)
        {
            return true;
        }
        else return false;
    }
    public bool IsOiled()
    {
        return oiled;
    }
    public bool IsMiniOiled()
    {
        return miniOiled;
    }
    public bool IsStunned()
    {
        return stunned;
    }
    public bool GetStickReset()
    {
        return face.GetStickReset();
    }
    public bool IsFiring()
    {
        return shooting.GetFireCheck();
    }
    public bool ReloadCheck()
    {
        return shooting.ReloadCheck();
    }
    public bool IsInCurrent()
    {
        return inCurrent;
    }
    public bool IsSlowed()
    {
        return slowed;
    }
    public bool GetIsMoving()
    {
        return movement.GetIsMoving();
    }
    #endregion

    #region Set Bools
    public void SetOiled(bool newOil)
    {
        oiled = newOil;
    }
    public void SetminiOiled(bool newminioil)
    {
        miniOiled = newminioil;
    }
    #endregion

    #region Get Floats
    public float GetLookAngle()
    {
        return face.GetLookAngle();
    }
    public float GetAgainTime()
    {
        return againTime;
    }
    public float GetOrigAgainTime()
    {
        return origAgainTime;
    }
    public float GetFireAdjustTime()
    {
        return fireAdjustTime;
    }
    public float GetDashAdjustForce()
    {
        return dashForceAdjust;
    }
    public float GetSpeed()
    {
        return speed;
    }
    public float GetNormSpeed()
    {
        return normSpeed;
    }

    public float GetFireTimer()
    {
        return shooting.GetFireTimer;
    }
    public float GetChargeTimer()
    {
        return shooting.GetChargeTimer;
    }

    public float GetOrigNormSpeed()
    {
        return origNorm;
    }
    public float GetSlowPercentage()
    {
        if (IsSlowed())
        {
            return slowSpeedPercent;
        }
        return 1;
    }
    #endregion

    #region Set Floats
    void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void SetNormSpeed(float newNorm)
    {
        normSpeed = newNorm;
    }
    public void SetSlowSpeedPercentage(float newSlow)
    {
        slowSpeedPercent = newSlow;
    }
    public void SetAgainTime(float newTime)
    {
        dash.SetAgainTime(newTime);
    }
    public void SetDashTime(float newTime)
    {
        dash.SetDashTime(newTime);
    }

    #region Reset Float Functions
    public void ResetNormSpeed()
    {
        normSpeed = origNorm;
    }
    public void ResetFireAdjust()
    {
        fireAdjustTime = 1;
    }
    public void ResetMoveAdjust()
    {
        moveAdjustSpeed = 1;
    }
    public void ResetDashForceAdjust()
    {
        dashForceAdjust = 1;
    }
    #endregion

    #endregion

    #region Get Vector2s
    public Vector2 getMoveVector()
    {
        return movement.getVector();
    }
    public Vector2 getLookVector()
    {
        return face.LookVector();
    }
    #endregion

    #region Set Ints
    public void SetDirectionIndex(int index)
    {
        movement.directionIndex = index;
    }
    #endregion

    #region Random Gets
    public Scene GetTheSceneName()
    {
        return SceneManager.GetActiveScene();
    }

    public Animator GetAnim()
    {
        return anim;
    }
    #endregion
    #endregion

    protected override void Awake()
    {
        base.Awake();

        weaponHolder = weaponHolderSprite;

        #region Get other Scripts from frikin
        face = GetComponent<FaceDirection>();
        movement = GetComponent<SharkMovement>();
        dash = GetComponent<SharkDash>();
        shooting = GetComponent<ShootingInput>();
        camShake = camRelated.GetComponent<Wehking_CameraShake>();
        itemModifers = GetComponent<Item_PlayerModifier>();
        #endregion

        #region Get Particle Systems
        chargeStage1.SetActive(false);
        chargeStage2.SetActive(false);
        addHealth.SetActive(false);
        addAmmoShotty.SetActive(false);
        addAmmoBurst.SetActive(false);
        addAmmoCharge.SetActive(false);
        addAmmoAll.SetActive(false);
        frozenSystem.SetActive(false);
        dashRight.SetActive(false);
        dashLeft.SetActive(false);
        poisoned.SetActive(false);
        #endregion

        #region Anim Bools
        anim.SetBool("isStunned", false);
        anim.SetBool("isOiled", false);
        anim.SetBool("isMiniOil", false);
        #endregion

        itemModifers.enabled = JP_GameManager.instance.inProGen;
    }

    private void Start()
    {
        #region Setting floats to static values
        normSpeed = movement.getNormSpeed();
        origNorm = normSpeed;
        dashSpeed = dash.getDashSpeed();
        origDash = dashSpeed;
        slowSpeedPercent = movement.getSlowSpeed();
        origSlowPercent = slowSpeedPercent;
        lookAngle = face.GetLookAngle();
        origAgainTime = dash.GetAgainTime();
        origLinearDrag = rb2d.drag;
        originalGrav = rb2d.gravityScale;

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region Pro-Gen Adjustments/Bool sets
        if (StatManager.instance != null)
        {
            #region Pro-Gen Bools

            #endregion

            #region Pro-Gen Floats
            fireAdjustTime = StatManager.instance.GetFireAdjustTime();
            moveAdjustSpeed = StatManager.instance.GetMoveSpeedAdjust();
            dashForceAdjust = StatManager.instance.GetDashForceAdjust();
            linearDragAdjust = StatManager.instance.GetLinearDragAdjust();
            angularDragAdjust = StatManager.instance.GetAngularDragAdjust();
            SetAgainTime(StatManager.instance.GetDashCooldown());
            #endregion
        }
        #endregion

        weaponWheel = control.GetButton("Weapon Wheel");

        isDeadSharky = JP_GameManager.instance.respawnScript.getIsDead();

        rb2d.gravityScale = JP_InGameMenu.instance.loadingTime ? 0 : originalGrav;

        /*
        Speed is not being set to normSpeed in Update anymore
         if you want to change speed SetSpeed(GetNormSpeed)
        */
        SetSpeed(normSpeed * moveAdjustSpeed);
        tempSpeed = speed * GetSlowPercentage();
        movement.speed = tempSpeed;

        isDashing = dash.getIsDashing();

        #region Particle System Timers
        if (addHealth.activeSelf)
        {
            healthTimer += Time.deltaTime;

            if (healthTimer >= .5f)
            {
                addHealth.SetActive(false);
                healthTimer = 0f;
            }
        }

        if (poisoned.activeSelf)
        {
            poisonTimer += Time.deltaTime;
            if (poisonTimer >= poisonTime)
            {
                poisoned.SetActive(false);
                poisonTimer = 0f;
            }

        }
        if (addAmmoAll.activeSelf)
        {
            ammoTimer += Time.deltaTime;
            if (ammoTimer >= 1.0f)
            {
                addAmmoAll.SetActive(false);
                ammoTimer = 0;
            }
        }
        if (addAmmoBurst.activeSelf)
        {
            ammoTimer += Time.deltaTime;
            if (ammoTimer >= .5f)
            {
                addAmmoBurst.SetActive(false);
                ammoTimer = 0;
            }
        }
        if (addAmmoCharge.activeSelf)
        {
            ammoTimer += Time.deltaTime;
            if (ammoTimer >= .5f)
            {
                addAmmoCharge.SetActive(false);
                ammoTimer = 0;
            }
        }
        if (addAmmoShotty.activeSelf)
        {
            ammoTimer += Time.deltaTime;
            if (ammoTimer >= .5f)
            {
                addAmmoShotty.SetActive(false);
                ammoTimer = 0;
            }
        }
        #endregion

        #region Status Effect Controls
        if (stunned)
        {
            stunTimer += Time.deltaTime;
            if (stunTimer >= stunTime)
            {
                anim.SetBool("isStunned", false);
                stun.GetComponent<SpriteRenderer>();
                camShake.shakeCamera = false;
                camShake.shakeTime = 1.5f;
                stunTime = 0;
                SetNormSpeed(origNorm);
                stunned = false;
                Wehking_SoundManagerV1.instance.RandomizeSfx(Wehking_SoundManagerV1.instance.EelStunnedSfx);
            }
        }

        if (frozen)
        {
            frozenSystem.SetActive(true);
            freezeTimer += Time.deltaTime;
            if (freezeTimer >= freezeTime)
            {
                frozenSystem.SetActive(false);
                frozen = false;
                slowed = false;
                freezeTimer = 0;
                frozenSoundTimer = 1f;
            }
        }
        #endregion

        if (GetTheSceneName().name == "Hub_Main Menu")
        {
            Physics2D.gravity = Vector2.zero;
        }
    }

    #region Collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        #region Particle Systems
        if (collision.gameObject.CompareTag("PickUp") && JP_GameManager.instance.frikinHealth.getHealth() < JP_GameManager.instance.frikinHealth.getMaxHealth())
        {
            addHealth.SetActive(true);
        }

        if (collision.gameObject.CompareTag("AmmoBurst") && JP_GameManager.instance.laser.currentWeapon.weaponName != "Normal Laser" && JP_GameManager.instance.laser.currAmmo[JP_GameManager.instance.laser.weaponIndex] < JP_GameManager.instance.laser.maxAmmo[JP_GameManager.instance.laser.weaponIndex])
        {
            addAmmoBurst.SetActive(true);
        }

        if (collision.gameObject.CompareTag("AmmoAll") && JP_GameManager.instance.laser.currentWeapon.weaponName != "Normal Laser" && JP_GameManager.instance.laser.currAmmo[JP_GameManager.instance.laser.weaponIndex] < JP_GameManager.instance.laser.maxAmmo[JP_GameManager.instance.laser.weaponIndex])
        {
            addAmmoAll.SetActive(true);
        }

        if (collision.gameObject.CompareTag("DamagingSeaweed"))
        {

            poisoned.SetActive(true);

        }
        #endregion //ammo partical systems on/off
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Slowing"))
        {
            slowed = true;
        }

        if (col.gameObject.CompareTag("Current"))
        {
            inCurrent = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Slowing"))
        {
            slowed = false;
        }

        if (col.gameObject.CompareTag("Current"))
        {
            inCurrent = false;
        }
    }

    #endregion

    #region Public Status Effect Functions 
    public void Stun(float newTime)
    {
        camShake.shakeCamera = true;
        SetNormSpeed(stopSpeed);
        shooting.CancelBurstOnStun();
        stunned = true;
        stunTime = newTime;
        anim.SetBool("isStunned", true);
        Wehking_SoundManagerV1.instance.RandomizeSfx(Wehking_SoundManagerV1.instance.EelStunnedSfx);
        //stunSFX.Play();
    }

    public void Freeze()
    {
        slowed = true;
        freezeTime = 2f;
        frozen = true;
        //frozenSFX.Play();
        frozenSoundTimer += Time.deltaTime;
        if (frozenSoundTimer >= 1f)
        {
            Wehking_SoundManagerV1.instance.RandomizeSfx(Wehking_SoundManagerV1.instance.FrozenSfx);
            frozenSoundTimer = 0;
        }
    }

    public void LockPlayerToPosition(Transform lockPosition)
    {
        transform.position = lockPosition.position;
    }

    void healthSystem()
    {
        GameObject.FindGameObjectWithTag("PickUp");
        healthTimer = 2;
    }

    public void SetInCurrent(bool change)
    {
        inCurrent = change;
    }

    //Adding Oiled
    public void Oil(float oilTime)
    {
        //OiledSFX.Play();
        Wehking_SoundManagerV1.instance.RandomizeSfx(Wehking_SoundManagerV1.instance.SquidOiledSfx);
        oiled = true;
        oilTimer = oilTime;
        anim.SetBool("isOiled", true);
    }
    public void miniOil(float miniOilTime)
    {
        miniOiled = true;
        miniOilTimer = miniOilTime;
        anim.SetBool("isMiniOil", true);
    }
    #endregion
}
