using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class JP_GameManager : MonoBehaviour
{
    public static JP_GameManager instance = null;

    #region Scripts
    [HideInInspector] public GeneralSharkBehavior playerCode;
    [HideInInspector] public PlayerHealthController frikinHealth;
    [HideInInspector] public Parker_Scoring scoringCode;
    [HideInInspector] public JP_InGameMenu pauseMenu;
    [HideInInspector] public PlayerWeaponInventory laser;
    [HideInInspector] public CS_SetAudioLevel setAudio;
    [HideInInspector] public RS_RespawnScript respawnScript;
    [HideInInspector] public LootManager loot;
    #endregion

    private Animator frikinAnim;

    float tickTimer;

    [HideInInspector]
    public Text weaponName, currAmmo, maxAmmo;
    [SerializeField]
    public int heartPieces;

    public int storySceneInt, progenSceneInt, menuSceneInt;

    public bool hasSpring;
    public bool devTools, maxHealth;
    public bool isBossScene;
    public bool invuln;
    public bool unlimitedDash;
    public bool unlimitedAmmo;
    public bool superShot;
    public bool kawaiiMode;
    public bool haveStartRestarted = false;
    public bool inMenu, inStory = true, inProGen, inMultiplayer;
    public bool miniBossActive;

    public GameObject dodgeTutTextbox;

    GameObject backgroundManager;

    [SerializeField]
    public string[] menuScenes, storyScenes, proGenScenes, bossScenes, multiplayerScenes;
    string lastScene = "";

    float timer;

    public int[] timeToFinishLevelsInMins;

    // Use this for initialization
    void Awake()
    {
        #region Instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        #endregion

        #region Set Code/Objects

        laser = GetComponent<PlayerWeaponInventory>();
        loot = GetComponent<LootManager>();
        frikinHealth = GetComponent<PlayerHealthController>();
        scoringCode = GetComponent<Parker_Scoring>();
        setAudio = GetComponent<CS_SetAudioLevel>();
        respawnScript = GetComponent<RS_RespawnScript>();


        #endregion

        tickTimer = 0f;
    }

    private void Start()
    {
        pauseMenu = JP_InGameMenu.instance;
        playerCode = FindObjectOfType<GeneralSharkBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBossScene)
        {
            JP_InGameMenu.instance.infoHolder.bossHealthBar.SetActive(miniBossActive);
        }
        if (SceneManager.GetActiveScene().name != lastScene)
        {
            haveStartRestarted = false;
            lastScene = SceneManager.GetActiveScene().name;
            LevelNameCheck();
            BossSceneCheck();
            laser.levelStartAmmo = laser.currAmmo;
            JP_InGameMenu.instance.infoHolder.backGroundManager.SetActive(true);
        }

        if (haveStartRestarted == false)
        {
            timer += Time.deltaTime;
            if (timer >= .5f)
            {
                haveStartRestarted = true;
                timer = 0;
            }
        }

        if (SceneManager.GetActiveScene().name == "Death" || SceneManager.GetActiveScene().name == "Menu" || SceneManager.GetActiveScene().name == "Winning_Hashtag")
        {
            miniBossActive = false;
            Cursor.lockState = CursorLockMode.Confined;
            laser.ReloadAllCurrentWeapons();
        }
        if (SceneManager.GetActiveScene().name == "Death" || SceneManager.GetActiveScene().name == "Winning_Hashtag")
        {
            Cursor.visible = true;
        }

        if (inStory || inProGen)
        {
            #region Find Code/Objects
            if (playerCode == null)
            {
                playerCode = FindObjectOfType<GeneralSharkBehavior>();
            }
            if (pauseMenu == null)
            {
                pauseMenu = JP_InGameMenu.instance;
            }
            if (playerCode != null)
            {
                if (frikinAnim == null)
                {
                    frikinAnim = playerCode.GetAnim();
                }
            }
            if (weaponName == null)
            {
                weaponName = JP_InGameMenu.instance.infoHolder.laserName;
            }
            #endregion

            //This timer needs to be removed when the Tick Function is removed from this script.
            tickTimer += Time.deltaTime;

            laser.enabled = TextboxManager.instance.stopShootingTheDamnLaser != true;

            //Region for the development tools that allow for debugging section of the game
            #region Dev Tools
            if (Input.GetKeyDown(KeyCode.I) || Input.GetButtonDown("Right Joystick Button"))
            {
                devTools = !devTools;
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                Debug.Log(TickTimeTimer.GetOnTickSubCount());
            }

            if (!devTools) return;
            if (invuln)
            {
                frikinHealth.setHealth(frikinHealth.getMaxHealth());
            }
            if (unlimitedAmmo)
            {
                laser.ReloadAllCurrentWeapons();
            }
            if (unlimitedDash)
            {
                playerCode.SetAgainTime(0f);
            }
            else if (!unlimitedDash)
            {
                playerCode.SetAgainTime(playerCode.GetOrigAgainTime());
            }

            if (maxHealth)
            {
                frikinHealth.setMaxHealth(20);
                frikinHealth.setHealth(frikinHealth.getMaxHealth());
            }

            if (!playerCode.gameObject.activeSelf) return;

            if (kawaiiMode)
            {
                GameObject.Find("KAWAII MODE").GetComponent<SpriteRenderer>().enabled = true;
            }
            if (!kawaiiMode)
            {
                GameObject.Find("KAWAII MODE").GetComponent<SpriteRenderer>().enabled = false;
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                superShot = !superShot;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                unlimitedDash = !unlimitedDash;
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                frikinHealth.setHealth(5f);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                invuln = !invuln;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                maxHealth = !maxHealth;
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                frikinHealth.setHealth(100);
                frikinHealth.setMaxHealth(100);
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                unlimitedAmmo = !unlimitedAmmo;
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Wehking_PlayerSavedStats.instance.saveLoadTest = !Wehking_PlayerSavedStats.instance.saveLoadTest;
                Debug.Log("save/load test");
            }

            if (Input.GetKeyDown((KeyCode.T)))
            {
                wehking_SteamAchievements.instance.DEBUG_FOR_STEAM_ACHIEVEMENTS = !wehking_SteamAchievements.instance.DEBUG_FOR_STEAM_ACHIEVEMENTS;
                Debug.Log("Achievement Test");
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                kawaiiMode = !kawaiiMode;
                GameObject.Find("KAWAII MODE").GetComponent<SpriteRenderer>().enabled = kawaiiMode;
            }
            if (Input.GetKeyDown((KeyCode.B)))
                JP_InGameMenu.instance.LevelSkip();
            if (Input.GetKeyDown(KeyCode.R))
            {
                Wehking_PlayerSavedStats.instance.ResetGameData(); ;
            }
                
            if(Input.GetKeyDown(KeyCode.Alpha0))
                Wehking_PlayerSavedStats.instance.levelsUnlocked ++;
            #endregion 
        }
    }

    /// <summary>
    /// This check runs checks through each of the arrays that hold the scene names
    /// to check which type of scene the player is currently in
    ///
    /// As soon as an array has found a true it breaks out of the array and sets the
    /// corresponding bool true;
    /// </summary>
    void LevelNameCheck()
    {
        for (int i = 0; i < menuScenes.Length; i++)
        {
            if (SceneManager.GetActiveScene().name == menuScenes[i])
            {
                menuSceneInt = i;
                inMenu = true;
                break;
            }
            else inMenu = false;
        }

        for (int i = 0; i < storyScenes.Length; i++)
        {
            if (SceneManager.GetActiveScene().isLoaded && SceneManager.GetActiveScene().name == storyScenes[i])
            {
                inStory = true;
                storySceneInt = i;
                if (i > 1)
                {
                    pauseMenu.worldNumber = (i - 2) / 4;
                }
                else pauseMenu.worldNumber = 0;

                loot.timeInMinsToCompleteTheLevel = timeToFinishLevelsInMins[i];

                break;
            }
            else inStory = false;
        }

        for (int i = 0; i < multiplayerScenes.Length; i++)
        {
            if (SceneManager.GetActiveScene().isLoaded && SceneManager.GetActiveScene().name == multiplayerScenes[i])
            {
                inMultiplayer = true;
                break;
            }
            else inMultiplayer = false;
        }

        //Commented as the ProGen mode is currently not working.

        //for (int i = 0; i < proGenScenes.Length; i++)
        //{
        //    if (SceneManager.GetActiveScene().name == proGenScenes[i])
        //    {
        //        progenSceneInt = i + 1;
        //        inProGen = true;
        //        break;
        //    }
        //    else inProGen = false;
        //}

        if (!inMenu && !inStory && !inProGen && !isBossScene)
        {
            Debug.Log("Current Scene has not been added to a scene array.");
            inStory = true; //This needs to be removed
        }

    }

    public void BossSceneCheck()
    {
        foreach (var t in bossScenes)
        {
            if (SceneManager.GetActiveScene().name == t)
            {
                isBossScene = true;
                break;
            }
            else isBossScene = false;
        }
    }

    //***UPDATE - THIS NEEDS TO BE CHANGED OVER TO TickTimeTimer***
    public bool TickDamage()
    {
        if (tickTimer >= 1.5)
        {
            tickTimer = 0;
            return true;
        }
        else return false;
    }

    #region HeartPieces
    public void IncreaseHeartPieces()
    {
        heartPieces++;
        Wehking_PlayerSavedStats.instance.heartPieces = heartPieces;
        if (heartPieces == 4)
        {
            //frikinHealth.HeartAnimation();
        }
    }

    public void ResetHeartPieces()
    {
        heartPieces = 0;
        Wehking_PlayerSavedStats.instance.heartPieces = heartPieces;
    }

    public int getHeartPieces()
    {
        return heartPieces;
    }
    #endregion

    public void DebugUpgradeThings()
    {
       //This is for the upgrade Debugs if we need to check values
    }
}
