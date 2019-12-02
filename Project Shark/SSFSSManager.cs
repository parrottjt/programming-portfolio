using System;
using System.Collections;
using System.Collections.Generic;
using ProBuilder2.Common;
using UnityEngine;
using Random = UnityEngine.Random;

/// SSFSSManager Explanation
/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
/// This instance handles everything that happens in the Final Phase of the Frakin Boss Fight
/// it uses two enums to switch between damage states which control not just he damage but the
/// attacks that are allowed to happen.
///
/// During the <see cref="DamagePhases.MustacheAway"/>
/// - The mustache moves around and is damageable. It tethers a line to it's start position for
///     the player to find it if the player loses sight of it.
/// - SSFSS Frakin will attack and switch between his 5 weapons. Randomly choosing a weapon once
///     the current weapon is empty.
/// - SSFSS Frakin will slam the wall 1 - 2 times while in this face which will cause rocks to
///     fall and deal damage to the player.
/// 
/// During the <see cref="DamagePhases.MustacheHome"/>
/// -The mustache is back on Frakin and Frakin is damageable but takes a fraction of the normal
///     damage.
///
/// Overall During both Phases <see cref="DoubleJetFire"/> is active and will start on a timer
/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

/// SSFSSManager Possible Improvements
/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
/// *Add Damage Aura Attack if player moves too close during <see cref="DamagePhases.MustacheAway"/>
/// - Trigger if player moves within a set distance
/// - Firing of projectiles would stop while the charge would happen.
///     - Try to ensure the player couldn't outsmart the system
///
/// *Add ability for enough damage to Mustache to activate wall smash
/// - Along with the random time and timer have.
/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

public class SSFSSManager : MonoBehaviour
{
    #region Variables
    //Inspector Variables
    [Header("Health Info")]
    [SerializeField] float[] healthPhase;
    public bool cantHitMe;
    [SerializeField]
    float damageAdjustment;

    [Header("Mustache Info")]
    [SerializeField]
    float speed;
    [SerializeField]
    float wallCheckDistance;
    [SerializeField] GameObject mustache;
    [SerializeField] Transform mustacheStart;

    [Header("Weapon Info")]
    [SerializeField] FrakinWeapon[] weapons;
    [SerializeField] Transform projSpawn, laserHitPos;
    [SerializeField] LineRenderer energyBeam;
    [SerializeField] float laserDamage, reloadTime;
    [Space]
    [ReadOnly]public int weaponIndex;
    [ReadOnly] public GameObject currProj;
    [ReadOnly] public int currMaxAmmo;
    [ReadOnly] public float currFireTime;
    [ReadOnly] public bool currProjIsTracking;


    [Header("Random Times Min/Max")]
    [SerializeField]
    float minRandomActiveTime = 0;
    [SerializeField]
    float maxRandomActiveTime = 0, minRandMoveTime = 0, maxRandMoveTime = 0;

    [Header("Jet Info")]
    [SerializeField] GameObject bottomParticleHolder, sideParticleHolder;
    [SerializeField] float jetTime;

    [Header("Wall Slam")]
    [SerializeField] GameObject boulder;
    [SerializeField] BoulderPosition[] boulderDropLocations;
    BoulderPosition[] masterCopyBoulderPositions;
    [SerializeField] [ReadOnly] float wallSmashTime;
    [SerializeField] float boulderSpawnTime, boulderSwitchTime;

    FrackinHealth health;
    Wehking_CameraShake shake;

    FinderForBottomWallParticle[] bottomWallParticleSystem;
    FinderScriptForSideWall[] sideWallParticleSystem;

    Transform player;

    Animator anim;
    LineRenderer mustacheLine;

    Vector2 closestPoint;

    //Floats
    float reactivateTime = 5;
    float randMoveTime, randActiveTime, randX, randY;
    float moveTimer, activeTimer, reactivateTimer, wallSmashTimer, fireTimer, reloadTimer, 
        boulderSpawnTimer, boulderSwitchTimer;
    float damageNumber;
    float lookRotate;

    float jetTimer, jetAliveTimer, jetActiveTimer;

    //Bools
    bool hasWallSmash, hasSetRandomBoulderTime, fireDoubleJets;
    bool mustacheAwayFromFrakin;
    bool runRandom = true, nearWall;
    bool fireTheJet, playerPosFound;
    bool reloadWeapon, getNewWeaponIndex = true;
    bool arrayBeenRandomized;
    bool rotScale;

    //Ints
    int phaseNum, checkSideWallIndex, checkBottomWallIndex;
    int currAmmo, lastWeaponIndex, randomIndexForBlockWall;
    int boulderLocIndex, dropNumber, totalDropping, randomizedIndex;
    [HideInInspector] public static int randomBDPatternIndex1, randomBDPatternIndex2, randomBDPatternIndex3;

    public ContactFilter2D contactFilter;
    #endregion

    public enum DamagePhases
    {
        MustacheAway,
        MustacheHome
    }
    DamagePhases damagePhase;

    public enum Weapons
    {
        ProjectileWall, //Done
        ProjectileBlockWallWithHole, //Done
        EnergyBlast, //Done
        RippleWithExpandingWheel, //Done
        BeamCannon //Done : Need to finish the animations, currently not active for weapon switch;
    }
    Weapons weapon;

    void Awake()
    {
        health = GetComponent<FrackinHealth>();
        mustacheLine = mustache.GetComponentInChildren<LineRenderer>();
        shake = FindObjectOfType<Wehking_CameraShake>();
        anim = GetComponent<Animator>();
        masterCopyBoulderPositions = boulderDropLocations;
        randomIndexForBlockWall = Random.Range(0, weapons[1].multiProj.Length);
    }

    // Use this for initialization
    void Start()
    {
        healthPhase[0] = health.getMaxHealth();
        bottomWallParticleSystem = bottomParticleHolder.GetComponentsInChildren<FinderForBottomWallParticle>();
        sideWallParticleSystem = sideParticleHolder.GetComponentsInChildren<FinderScriptForSideWall>();
        foreach (var damageParticle in bottomWallParticleSystem)
        {
            damageParticle.GetComponent<Wehking_DamageOverTime>().damage = 10;
        }

        foreach (var damageParticle in sideWallParticleSystem)
        {
            damageParticle.GetComponent<Wehking_DamageOverTime>().damage = 10;
        }
        ResetArray();
        weaponIndex = 3;
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health.cantHitMe = cantHitMe;

        #region Check Health Function
        for (int i = 0; i < healthPhase.Length; i++)
        {
            if (i != healthPhase.Length - 1)
            {
                if (healthPhase[i + 1] < health.getHealth() && health.getHealth() <= healthPhase[i])
                {
                    phaseNum = i + 1;
                    break;
                }
            }
            if (i == healthPhase.Length - 1)
            {
                if (health.getHealth() <= healthPhase.Length - 1)
                {
                    phaseNum = i + 1;
                }
            }
        }
        #endregion

        damagePhase = mustacheAwayFromFrakin ? DamagePhases.MustacheAway : DamagePhases.MustacheHome;

        #region Jet Control
        if (!fireTheJet)
        {
            if (jetTimer <= jetTime)
            {
                jetTimer += Time.deltaTime;
            }
            if (jetTimer >= jetTime)
            {
                DoubleJetFire();
                fireTheJet = true;
            }
        }

        if (fireTheJet)
        {
            if (bottomWallParticleSystem[checkBottomWallIndex].SSFSS_Particle1.isStopped
                && sideWallParticleSystem[checkSideWallIndex].SSFSS_Particle1.isStopped)
            {
                jetActiveTimer += Time.deltaTime;
            }
            if (jetActiveTimer >= 2f)
            {
                jetAliveTimer += Time.deltaTime;
                if (bottomWallParticleSystem[checkBottomWallIndex].SSFSS_Particle1.isStopped
                    && sideWallParticleSystem[checkSideWallIndex].SSFSS_Particle1.isStopped)
                {
                    bottomWallParticleSystem[checkBottomWallIndex].SSFSS_Particle1.Play();
                    bottomWallParticleSystem[checkBottomWallIndex].GetComponent<Animator>().SetBool("ToggleOn", true);
                    sideWallParticleSystem[checkSideWallIndex].SSFSS_Particle1.Play();
                    sideWallParticleSystem[checkSideWallIndex].GetComponent<Animator>().SetBool("ToggleOn", true);
                }

                if (jetAliveTimer >= 2)
                {

                    bottomWallParticleSystem[checkBottomWallIndex].GetComponent<Animator>().SetBool("ToggleOn", false);
                    sideWallParticleSystem[checkSideWallIndex].GetComponent<Animator>().SetBool("ToggleOn", false);
                    jetAliveTimer = 0;
                    jetActiveTimer = 0;
                    jetTimer = 0;
                    fireTheJet = false;
                }
            }
        }
        #endregion

        switch (damagePhase)
        {
            case DamagePhases.MustacheAway:

                mustache.GetComponent<Collider2D>().enabled = true;
                cantHitMe = true;
                damageAdjustment = 1;
                mustacheLine.SetPosition(0, mustache.transform.position);
                mustacheLine.SetPosition(1, mustacheStart.position);

                #region Movement
                Collider2D[] test = GameObject.FindGameObjectsWithTag("enemyBorder").GetComponents<Collider2D>();

                for (int i = 0; i < test.Length; i++)
                {
                    if (Mathf.Abs(Vector2.Distance(test[i].GetComponent<Collider2D>().bounds.ClosestPoint(mustache.transform.position),
                            mustache.transform.position))
                        <= mustache.GetComponent<SpriteRenderer>().bounds.extents.magnitude + wallCheckDistance && !nearWall)
                    {
                        closestPoint = test[i].GetComponent<Collider2D>().bounds.ClosestPoint(mustache.transform.position);
                        nearWall = true;
                        moveTimer = 0;
                        break;
                    }
                }

                if (activeTimer <= randActiveTime)
                {
                    if (!anim.GetBool("SlamWall") && !anim.GetBool("EnergyBeam"))
                    {
                        activeTimer += Time.deltaTime;
                    }
                    if (!nearWall)
                    {
                        if (runRandom)
                        {
                            randX = Random.Range(-1f, 1f);
                            randY = Random.Range(-1f, 1f);
                            randMoveTime = Random.Range(minRandMoveTime, maxRandMoveTime);
                            runRandom = false;
                        }
                        if (!runRandom)
                        {
                            moveTimer += Time.deltaTime;
                            if (moveTimer >= randMoveTime)
                            {
                                runRandom = true;
                                moveTimer = 0;
                            }
                        }
                        mustache.transform.Translate(new Vector2(randX, randY).normalized * speed * Time.deltaTime);
                    }
                    if (nearWall)
                    {
                        mustache.transform.Translate((new Vector2(mustache.transform.position.x, mustache.transform.position.y) - closestPoint).normalized * speed * Time.deltaTime);
                        moveTimer += Time.deltaTime;
                        if (moveTimer >= 1f)
                        {
                            nearWall = false;
                            runRandom = true;
                            moveTimer = 0;
                        }
                    }
                }
                if (activeTimer > randActiveTime)
                {
                    if (Mathf.Abs(Vector2.Distance(new Vector2(mustache.transform.position.x, mustache.transform.position.y), new Vector2(mustacheStart.position.x, mustacheStart.position.y))) <= .5f
                        && !anim.GetBool("Shrink"))
                    {
                        mustache.transform.position = mustacheStart.position;
                        anim.SetBool("Shrink", true);
                    }
                    if (Mathf.Abs(Vector2.Distance(new Vector2(mustache.transform.position.x, mustache.transform.position.y), new Vector2(mustacheStart.position.x, mustacheStart.position.y))) > .5f)
                    {
                        mustache.transform.position = Vector2.MoveTowards(mustache.transform.position, mustacheStart.position, speed * Time.deltaTime);
                    }
                }
                #endregion

                //Make Weapons change when the phase switches
                #region Weapon Control
                if (getNewWeaponIndex)
                {
                    weaponIndex = Random.Range(0, weapons.Length - 1);
                    if (weaponIndex != lastWeaponIndex)
                    {
                        lastWeaponIndex = weaponIndex;
                        getNewWeaponIndex = false;
                        reloadWeapon = true;
                    }
                }

                SelectWeapon(weaponIndex);
                weapons[1].projectile = weapons[1].multiProj[randomIndexForBlockWall];
                FrakinWeaponAttacks(weapon);

                #endregion

                //Boulders are dropping weird
                #region Wall_Smash
                anim.SetFloat("Which Slam", (float)phaseNum);

                if (hasWallSmash)
                {
                    switch (phaseNum)
                    {
                        case 1:
                            if (totalDropping == 0)
                            {
                                BDSwitch(randomBDPatternIndex1);
                            }

                            if (totalDropping == 1)
                            {
                                hasWallSmash = false;
                                totalDropping = 0;
                            }
                            break;

                        case 2:
                            if (totalDropping == 0)
                            {
                                BDSwitch(randomBDPatternIndex1);
                            }

                            if (totalDropping == 1)
                            {
                                BDSwitch(randomBDPatternIndex2);
                            }

                            if (totalDropping == 2)
                            {
                                hasWallSmash = false;
                                totalDropping = 0;
                            }
                            break;

                        case 3:
                            if (totalDropping == 0)
                            {
                                BDSwitch(randomBDPatternIndex1);
                            }

                            if (totalDropping == 1)
                            {
                                BDSwitch(randomBDPatternIndex2);
                            }

                            if (totalDropping == 2)
                            {
                                BDSwitch(randomBDPatternIndex3);
                            }

                            if (totalDropping == 3)
                            {
                                hasWallSmash = false;
                                totalDropping = 0;
                            }
                            break;
                    }
                    return;
                }
                if (!hasSetRandomBoulderTime)
                {
                    wallSmashTime = Random.Range(randActiveTime * .25f, randActiveTime * .75f);
                    hasSetRandomBoulderTime = true;
                }
                if (!anim.GetBool("EnergyBeam"))
                {
                    wallSmashTimer += Time.deltaTime;
                }
                //Possibility to add in after the mustache has taken enough damage
                if (wallSmashTimer >= wallSmashTime)
                {
                    anim.SetBool("SlamWall", true);
                    hasWallSmash = true;
                    hasSetRandomBoulderTime = false;
                    wallSmashTimer = 0;
                }

                #endregion
                break;

            case DamagePhases.MustacheHome:
                damageAdjustment = .25f;
                mustache.GetComponent<Collider2D>().enabled = false;
                cantHitMe = false;
                reactivateTimer += Time.deltaTime;
                if (reactivateTimer >= reactivateTime)
                {
                    anim.SetBool("Grow", true);
                }
                break;
        }
    }

    #region Animation Functions
    public void ShakeCamera()
    {
        shake.shakeCamera = true;
        shake.shakeTime = 1f;
        shake.shakeAmount = .5f;
    }

    #region Boulder Drop Functions

    public void CalculateDropNumber1()
    {
        SSFSSManager.randomBDPatternIndex1 = Random.Range(0, 4);
    }

    public void CalculateDropNumber2()
    {
        SSFSSManager.randomBDPatternIndex2 = Random.Range(0, 4);
    }

    public void CalculateDropNumber3()
    {
        SSFSSManager.randomBDPatternIndex3 = Random.Range(0, 4);
    }

    void BDSwitch(int BDNumb)
    {
        switch (BDNumb)
        {
            case 0:
                BD_Pattern_LeftToRight();
                break;

            case 1:
                BD_Pattern_RightToLeft();
                break;

            case 2:
                BD_Pattern_ReorderDrop();
                break;

            case 3:
                BD_Pattern_OutFromPlayer();
                break;
        }
    }

    public void BD_Pattern_OutFromPlayer()
    {
        if (!playerPosFound)
        {
            for (int i = 0; i < boulderDropLocations.Length; i++)
            {
                if ((player.position.x - boulderDropLocations[i].boulderPos.position.x) <= 1.45f && (player.position.x - boulderDropLocations[i].boulderPos.position.x) >= -1.45f)
                {
                    boulderLocIndex = i;
                    playerPosFound = true;
                    break;
                }
            }
        }

        if (playerPosFound)
        {
            switch (boulderLocIndex)
            {
                case 0:
                    ResetArray();
                    break;

                case 1:
                    boulderDropLocations[0] = masterCopyBoulderPositions[1];
                    boulderDropLocations[1] = masterCopyBoulderPositions[0];
                    boulderDropLocations[2] = masterCopyBoulderPositions[2];
                    boulderDropLocations[3] = masterCopyBoulderPositions[3];
                    boulderDropLocations[4] = masterCopyBoulderPositions[4];
                    boulderDropLocations[5] = masterCopyBoulderPositions[5];
                    boulderDropLocations[6] = masterCopyBoulderPositions[6];
                    break;

                case 2:
                    boulderDropLocations[0] = masterCopyBoulderPositions[2];
                    boulderDropLocations[1] = masterCopyBoulderPositions[1];
                    boulderDropLocations[2] = masterCopyBoulderPositions[3];
                    boulderDropLocations[3] = masterCopyBoulderPositions[0];
                    boulderDropLocations[4] = masterCopyBoulderPositions[4];
                    boulderDropLocations[5] = masterCopyBoulderPositions[5];
                    boulderDropLocations[6] = masterCopyBoulderPositions[6];
                    break;

                case 3:
                    boulderDropLocations[0] = masterCopyBoulderPositions[3];
                    boulderDropLocations[1] = masterCopyBoulderPositions[2];
                    boulderDropLocations[2] = masterCopyBoulderPositions[4];
                    boulderDropLocations[3] = masterCopyBoulderPositions[1];
                    boulderDropLocations[4] = masterCopyBoulderPositions[5];
                    boulderDropLocations[5] = masterCopyBoulderPositions[0];
                    boulderDropLocations[6] = masterCopyBoulderPositions[6];

                    break;

                case 4:
                    boulderDropLocations[0] = masterCopyBoulderPositions[4];
                    boulderDropLocations[1] = masterCopyBoulderPositions[5];
                    boulderDropLocations[2] = masterCopyBoulderPositions[3];
                    boulderDropLocations[3] = masterCopyBoulderPositions[6];
                    boulderDropLocations[4] = masterCopyBoulderPositions[2];
                    boulderDropLocations[5] = masterCopyBoulderPositions[1];
                    boulderDropLocations[6] = masterCopyBoulderPositions[0];
                    break;

                case 5:
                    boulderDropLocations[0] = masterCopyBoulderPositions[5];
                    boulderDropLocations[1] = masterCopyBoulderPositions[6];
                    boulderDropLocations[2] = masterCopyBoulderPositions[4];
                    boulderDropLocations[3] = masterCopyBoulderPositions[3];
                    boulderDropLocations[4] = masterCopyBoulderPositions[2];
                    boulderDropLocations[5] = masterCopyBoulderPositions[1];
                    boulderDropLocations[6] = masterCopyBoulderPositions[0];
                    break;

                case 6:

                    break;
            }

            if (boulderLocIndex == 0)
            {

                BD_Pattern_LeftToRight();
                boulderSpawnTimer = 0;

            }

            if (boulderLocIndex == 6)
            {

                BD_Pattern_RightToLeft();

            }

            if (boulderLocIndex != 0 && boulderLocIndex != 6)
            {
                if (dropNumber <= 6)
                {
                    boulderSpawnTimer += Time.deltaTime;
                    if (boulderSpawnTimer >= boulderSpawnTime)
                    {
                        if (dropNumber == 0)
                        {
                            Instantiate(boulder, boulderDropLocations[dropNumber].boulderPos.position, Quaternion.identity);
                            dropNumber += 2;
                            boulderSpawnTimer = 0;
                        }
                        else
                        {
                            if (dropNumber > 0 && dropNumber <= 6)
                            {
                                Instantiate(boulder, boulderDropLocations[dropNumber - 1].boulderPos.position, Quaternion.identity);
                                Instantiate(boulder, boulderDropLocations[dropNumber].boulderPos.position, Quaternion.identity);
                                dropNumber += 2;
                                boulderSpawnTimer = 0;
                            }
                        }
                    }
                }
                if (dropNumber > 6)
                {
                    boulderSwitchTimer += Time.deltaTime;
                    if (boulderSwitchTimer >= boulderSwitchTime)
                    {
                        totalDropping++;
                        ResetArray();
                        dropNumber = 0;
                    }
                }
            }
        }
    }

    public void BD_Pattern_LeftToRight()
    {
        if (dropNumber <= 6)
        {
            boulderSpawnTimer += Time.deltaTime;
            if (boulderSpawnTimer >= boulderSpawnTime)
            {
                Instantiate(boulder, boulderDropLocations[dropNumber].boulderPos.position, Quaternion.identity);
                dropNumber++;
                boulderSpawnTimer = 0;
            }
        }
        if (dropNumber > 6)
        {
            boulderSpawnTimer += Time.deltaTime;
            if (boulderSwitchTimer >= boulderSwitchTime)
            {
                totalDropping++;
                ResetArray();
                dropNumber = 0;
            }
        }
    }

    public void BD_Pattern_RightToLeft()
    {
        if (dropNumber <= 6)
        {
            if (boulderDropLocations == masterCopyBoulderPositions)
            {
                Array.Reverse(boulderDropLocations);
            }
            boulderSpawnTimer += Time.deltaTime;
            if (boulderSpawnTimer >= boulderSpawnTime)
            {
                Instantiate(boulder, boulderDropLocations[dropNumber].boulderPos.position, Quaternion.identity);
                dropNumber++;
                boulderSpawnTimer = 0;
            }
        }
        if (dropNumber > 6)
        {
            boulderSpawnTimer += Time.deltaTime;
            if (boulderSwitchTimer >= boulderSwitchTime)
            {
                totalDropping++;
                ResetArray();
                dropNumber = 0;
            }
        }
    }

    public void BD_Pattern_ReorderDrop()
    {
        if (!arrayBeenRandomized)
        {
            for (int i = 0; i < boulderDropLocations.Length; i++)
            {
                boulderDropLocations[i].sortNum = Random.Range(0f, 100f);
                randomizedIndex++;
            }

            if (randomizedIndex == boulderDropLocations.Length)
            {
                arrayBeenRandomized = true;
                Array.Sort(boulderDropLocations);
            }
        }
        else
        {
            BD_Pattern_LeftToRight();
        }
    }

    void ResetArray()
    {
        Array.Sort(boulderDropLocations, new IndexComparer());
    }
    #endregion

    public void ResetGrow()
    {
        anim.SetBool("Grow", false);
        randActiveTime = Random.Range(minRandomActiveTime, maxRandomActiveTime);
        reactivateTimer = 0;
        mustacheAwayFromFrakin = true;
    }

    public void ResetShrink()
    {
        anim.SetBool("Shrink", false);
        activeTimer = 0f;
        mustacheAwayFromFrakin = false;
        ResetWallSmash();
    }

    public void ResetWallSmash()
    {
        anim.SetBool("SlamWall", false);
        wallSmashTimer = 0;
    }

    #region Energy Beam Functions
    public void TurnEnergyBeamOn()
    {
        energyBeam.enabled = true;
        anim.SetBool("EnergyBeam", true);
    }

    public void TurnEnergyBeamOff()
    {
        energyBeam.enabled = false;
        anim.SetBool("EnergyBeam", false);
    }

    public void EnergyBeamEnd_ChangeWeapon()
    {
        anim.SetBool("ChangeWeapon_EB", true);
    }
    #endregion
    #endregion

    void DoubleJetFire()
    {
        for (int i = 0; i < bottomWallParticleSystem.Length; i++)
        {
            if ((player.position.x - bottomWallParticleSystem[i].gameObject.transform.position.x) <= 5 && (player.position.x - bottomWallParticleSystem[i].gameObject.transform.position.x) >= -5)
            {
                checkBottomWallIndex = i;
                bottomWallParticleSystem[checkBottomWallIndex].SSFSS_StartUo.Play();
                break;
            }
        }

        for (int i = 0; i < sideWallParticleSystem.Length; i++)
        {
            if ((player.position.y - sideWallParticleSystem[i].gameObject.transform.position.y) <= 3.5 && (player.position.y - sideWallParticleSystem[i].gameObject.transform.position.y) >= -3.5)
            {
                checkSideWallIndex = i;
                sideWallParticleSystem[checkSideWallIndex].SSFSS_StartUpParticle.Play();
                break;
            }
        }
    }

    #region Weapon Functions
    void SelectWeapon(int index)
    {
        switch (index)
        {
            case 0:
                weapon = (Weapons)index;
                break;

            case 1:
                weapon = (Weapons)index;
                break;

            case 2:
                weapon = (Weapons)index;
                break;

            case 3:
                weapon = (Weapons)index;
                break;

            case 4:
                weapon = (Weapons)index;
                break;
        }
    }

    void FrakinWeaponAttacks(Weapons weapon)
    {
        switch (weapon)
        {
            case Weapons.ProjectileWall:
                currProj = weapons[(int)weapon].projectile;
                currMaxAmmo = weapons[(int)weapon].maxAmmo;
                currFireTime = weapons[(int)weapon].fireTime;
                currProjIsTracking = weapons[(int)weapon].isTracking;
                break;

            case Weapons.ProjectileBlockWallWithHole:
                currProj = weapons[(int)weapon].projectile;
                currMaxAmmo = weapons[(int)weapon].maxAmmo;
                currFireTime = weapons[(int)weapon].fireTime;
                currProjIsTracking = weapons[(int)weapon].isTracking;
                break;

            case Weapons.EnergyBlast:
                currProj = weapons[(int)weapon].projectile;
                currMaxAmmo = weapons[(int)weapon].maxAmmo;
                currFireTime = weapons[(int)weapon].fireTime;
                currProjIsTracking = weapons[(int)weapon].isTracking;
                break;

            case Weapons.RippleWithExpandingWheel:
                currProj = weapons[(int)weapon].projectile;
                currMaxAmmo = weapons[(int)weapon].maxAmmo;
                currFireTime = weapons[(int)weapon].fireTime;
                currProjIsTracking = weapons[(int)weapon].isTracking;
                break;

            case Weapons.BeamCannon:
                currProj = weapons[(int)weapon].projectile;
                currMaxAmmo = weapons[(int)weapon].maxAmmo;
                currFireTime = weapons[(int)weapon].fireTime;
                currProjIsTracking = weapons[(int)weapon].isTracking;
                break;
        }
        if (reloadWeapon)
        {
            fireTimer = 0;
            currAmmo = currMaxAmmo;
            reloadWeapon = false;
        }
        if (!reloadWeapon)
        {
            if (weaponIndex != weapons.Length - 1)
            {
                Debug.Log(currAmmo);
                fireTimer += Time.deltaTime;
                if (currAmmo > 0)
                {
                    if (fireTimer >= currFireTime)
                    {
                        FireFromWeaponSlot(currProj, projSpawn, currProjIsTracking);
                        currAmmo--;
                        randomIndexForBlockWall = Random.Range(0, weapons[1].multiProj.Length);
                        fireTimer = 0;
                    }
                }

                if (currAmmo <= 0)
                {
                    reloadTimer += Time.deltaTime;
                    if (reloadTimer >= reloadTime)
                    {
                        reloadTimer = 0;
                        getNewWeaponIndex = true;
                    }
                }
            }

            if (weaponIndex == weapons.Length - 1)
            {
                if (!anim.GetBool("EnergyBeam"))
                {
                    anim.SetBool("EnergyBeam", true);
                }

                if (anim.GetBool("ChangeWeapon_EB"))
                {
                    reloadTimer = 0;
                    getNewWeaponIndex = true;
                }
                if (energyBeam.enabled)
                {
                    RaycastHit2D hit1 = Physics2D.Raycast(projSpawn.position, projSpawn.right);
                    laserHitPos.position = hit1.point;
                    energyBeam.SetPosition(1, projSpawn.position);
                    energyBeam.SetPosition(0, laserHitPos.position);
                    if (hit1.collider.gameObject.CompareTag("Player"))
                    {
                        JP_GameManager.instance.frikinHealth.removeHealth(laserDamage);
                        //Add any effect that happens to the player on hit with the laser here;
                    }
                }
            }
        }
    }

    void FireFromWeaponSlot(GameObject projectile, Transform slotSpawn, bool tracking)
    {
        if (tracking)
        {
            Vector2 diff = player.transform.position - slotSpawn.transform.position;
            lookRotate = Mathf.Atan2(-diff.x, diff.y) * Mathf.Rad2Deg;
            slotSpawn.rotation = Quaternion.Euler(0, 0, lookRotate);
        }
        else if (!tracking)
        {
            slotSpawn.localRotation = Quaternion.Euler(0, 0, -90);
        }
        //if (randomRapid)
        //{
        //    slotSpawn.localRotation = Quaternion.Euler(0, 0, randomRange);
        //}

        Instantiate(projectile, slotSpawn.position, slotSpawn.rotation);
    } 
    #endregion

    public class IndexComparer : IComparer
    {
        int IComparer.Compare(object a, object b)
        {
            BoulderPosition c1 = (BoulderPosition)a;
            BoulderPosition c2 = (BoulderPosition)b;
            return c1.indexNum.CompareTo(c2.indexNum);
        }
    }
}

[Serializable]
public partial class BoulderPosition
{
    public Transform boulderPos;
    public int indexNum;
    public float sortNum;
}

public partial class BoulderPosition : IComparable
{
    public int CompareTo(object obj)
    {
        var sortNumber = obj as BoulderPosition;
        if (sortNumber != null)
        {
            return sortNum.CompareTo(sortNumber.sortNum);  // compare user names
        }
        throw new ArgumentException("Object is not a User");
    }
}