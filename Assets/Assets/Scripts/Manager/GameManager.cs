// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
// using TMPro;
// using System.Linq;

// public class GameManager : MonoBehaviour
// {
    
//     public GameObject effectCamera, BtnReplay, bouderCoinFly, btnx3Coin, btnTabNext, phaohoa, btnReplay2, btnSkipLevelLose, warningAchievment;
//     public Sprite winSp, loseSp;
//     public PhysicsMaterial2D matStone;
//     public bool playerMove;
//     public int counthatwater;
//     public enum GAMESTATE { BEGIN, PLAYING, WIN, LOSE }
//     public static GameManager Instance;
//     public MissionType mSavePrincess, mCollect, mOpenChest, mKill;
//     public Image imgQuestImage;
//     public TextMeshProUGUI txtQuestText;

//     public Text txtLevel, levelTextGameOver;
//     public Text txtCoin;
//     public Text txtCoinWin;
//     public bool isTest;
//     [HideInInspector] public bool canUseTrail;
//     public GAMESTATE gameState;
//     [SerializeField] public LevelConfig levelConfig;
//     public MapLevelManager mapLevel;
//     public int totalGems;
//     public CamFollow _camFollow;
//     public Image gPanelWin;

//     [HideInInspector] public GameObject gTargetFollow;
    
//     // Store the actual level being played for correct display
//     private int currentPlayingLevel;
    
//     public void LoseDisplay()
//     {
//         btnTabNext.SetActive(false);
//         btnx3Coin.SetActive(false);
//         phaohoa.SetActive(false);
//         btnReplay2.SetActive(true);
//         btnSkipLevelLose.SetActive(true);
//         gPanelWin.sprite = loseSp;
//     }
    
//     public void CheckDisplayWarningAchievement()
//     {
//         if (DataController.instance != null)
//             warningAchievment.SetActive(DataController.instance.CheckWarningAchievement());
//     }
    
//     private void Awake()
//     {
//         Instance = this;
//     }
    
//     private void OnUpdateCoin()
//     {
//         txtCoin.text = Utils.currentCoin.ToString();
//         Utils.SaveCoin();
//     }
    
//     public int coinTemp;
    
//     void Start()
//     {
//         AdManager.Instance.LoadAd();
//         Utils.LoadGameData();
        
//         // Get the level to play (either selected or current progress)
//         int levelToPlay = Utils.GetLevelToPlay();
//         currentPlayingLevel = levelToPlay;
        
//         // Clear the selected level key after reading it
//         Utils.ClearSelectedLevel();
        
//         // Display the correct level number
//         levelTextGameOver.text = txtLevel.text = "LEVEL " + (currentPlayingLevel + 1).ToString();
//         txtCoinWin.text = Utils.currentCoin.ToString();
//         coinTemp = Utils.currentCoin;
//         OnUpdateCoin();

//         if (!isTest)
//         {
//             LoadLevelToPlay(levelToPlay);
//             Utils.RealLevelIndex = levelToPlay;
//         }
        
//         if (SoundManager.Instance != null)
//         {
//             SoundManager.Instance.PlayBackgroundMusic();
//         }
    
//         CheckDisplayWarningAchievement();
        
//         // Verify buttons are assigned
//         if (btnSkipLevelLose != null)
//         {
//             Debug.Log("Skip button is assigned");
//         }
//         else
//         {
//             Debug.LogError("Skip button is NOT assigned in Inspector!");
//         }
        
//         if (warningAchievment != null)
//         {
//             Debug.Log("Achievement warning is assigned");
//         }
//         else
//         {
//             Debug.LogError("Achievement warning is NOT assigned in Inspector!");
//         }
//     }

//     private void OnApplicationQuit()
//     {
//         Utils.SaveCoin();
//     }

//     private void OnApplicationPause(bool pause)
//     {
//         if (pause)
//         {
//             Utils.SaveCoin();
//         }
//     }

//     private void OnChange(Sprite _spr, string _text)
//     {
//         imgQuestImage.sprite = _spr;
//         txtQuestText.text = "<color=#FFBC01> LEVEL " + (currentPlayingLevel + 1).ToString("0#") + "</color> " + _text.ToUpper();
//     }
    
//     public void OnInitQuestText(MapLevelManager.QUEST_TYPE _questType)
//     {
//         switch (_questType)
//         {
//             case MapLevelManager.QUEST_TYPE.COLLECT:
//                 OnChange(mCollect.spr_, mCollect.strQuest);
//                 break;
//             case MapLevelManager.QUEST_TYPE.KILL:
//                 OnChange(mKill.spr_, mKill.strQuest);
//                 break;
//             case MapLevelManager.QUEST_TYPE.OPEN_CHEST:
//                 OnChange(mOpenChest.spr_, mOpenChest.strQuest);
//                 break;
//             case MapLevelManager.QUEST_TYPE.SAVE_HOSTAGE:
//                 OnChange(mSavePrincess.spr_, mSavePrincess.strQuest);
//                 break;
//         }
//     }

//     private void LoadLevelToPlay(int realLevelIndex)
//     {
//         if (realLevelIndex < 0 || realLevelIndex >= levelConfig.lstAllLevel.Count)
//         {
//             Debug.LogError($"Invalid level index: {realLevelIndex}. Loading level 0 instead.");
//             realLevelIndex = 0;
//             currentPlayingLevel = 0;
//         }
        
//         MapLevelManager mapInstall = levelConfig.lstAllLevel[realLevelIndex];
//         mapLevel = Instantiate(mapInstall, Vector3.zero, Quaternion.identity);
        
//         Debug.Log($"Loading level: {realLevelIndex + 1}");
        
//         if (mapLevel.lstAllStick.Count > 0)
//             playerMove = true;
//         if (mapLevel.waterObj != null)
//             counthatwater = mapLevel.waterObj.gGems.Count;
//     }

//     private void ActiveCamEff()
//     {
//         _camFollow.objectToFollow = gTargetFollow;
//         _camFollow.beginFollow = true;
//     }
    
//     public void ShowWinPanel()
//     {
//         Debug.Log("ShowWinPanel called!");
//         StartCoroutine(IEWaitToShowWinLose(true));
//     }
    
//     public int enemyKill;
//     static int countpasslevel;
    
//     private IEnumerator IEWaitToShowWinLose(bool isWin)
//     {
//         yield return new WaitForSeconds(0.5f);
        
//         Debug.Log($"IEWaitToShowWinLose: isWin={isWin}, panel active={gPanelWin.gameObject.activeSelf}");
    
//         if (isWin)
//         {
//             if (!gPanelWin.gameObject.activeSelf)
//             {
//                 Debug.Log("Showing win panel now!");
                
//                 ActiveCamEff();
//                 Utils.currentCoin += Utils.BASE_COIN;

//                 OnUpdateCoin();
//                 gPanelWin.gameObject.SetActive(true);

//                 BtnReplay.SetActive(false);
//                 effectCamera.SetActive(false);
                
//                 if (SoundManager.Instance != null)
//                 {
//                     SoundManager.Instance.PlaySound(SoundManager.Instance.acWin);
//                 }

//                 if (DataController.instance != null)
//                 {
//                     DataController.instance.DoAchievment(0, 1);

//                     if (mapLevel.questType == MapLevelManager.QUEST_TYPE.SAVE_HOSTAGE)
//                         DataController.instance.DoAchievment(2, 1);
//                     else if (mapLevel.questType == MapLevelManager.QUEST_TYPE.OPEN_CHEST)
//                         DataController.instance.DoAchievment(3, 1);
//                     else if (mapLevel.questType == MapLevelManager.QUEST_TYPE.COLLECT)
//                         DataController.instance.DoAchievment(1, 1);
                    
//                     DataController.instance.DoAchievment(4, enemyKill);
//                 }

//                 if (DataParam.firsttime == 0)
//                 {
//                     if (Utils.LEVEL_INDEX >= DataParam.levelpassshowad)
//                     {
//                         DataParam.firsttime = 1;
//                         Debug.Log("Show ads TH 1");
//                     }
//                 }
//                 else
//                 {
//                     countpasslevel++;
//                     if (countpasslevel >= DataParam.delayshowAds && 
//                         (System.DateTime.Now - DataParam.oldTimeShowAds).TotalSeconds >= DataParam.timedelayShowAds)
//                     {
//                         countpasslevel = 0;
//                         DataParam.oldTimeShowAds = System.DateTime.Now;
//                     }
//                     Debug.Log("Show ads TH 2");
//                 }
//             }
//             else
//             {
//                 Debug.LogWarning("Win panel already active!");
//             }
//         }
//         else
//         {
//             if (!gPanelWin.gameObject.activeSelf)
//             {
//                 Debug.Log("Showing lose panel now!");
                
//                 ActiveCamEff();
//                 gPanelWin.gameObject.SetActive(true);
//                 effectCamera.SetActive(false);
//                 LoseDisplay();
                
//                 if (SoundManager.Instance != null)
//                 {
//                     SoundManager.Instance.PlaySound(SoundManager.Instance.acLose);
//                 }
//             }
//             else
//             {
//                 Debug.LogWarning("Lose panel already active!");
//             }
//         }
//     }
    
//     private bool playingSoundLava = false;
    
//     public void PlaySoundLavaOnWater()
//     {
//         if (SoundManager.Instance != null)
//         {
//             SoundManager.Instance.PlaySound(SoundManager.Instance.acStoneApear);
//             if (!playingSoundLava)
//             {
//                 SoundManager.Instance.PlaySound(SoundManager.Instance.acLavaOnWater);
//             }
//             playingSoundLava = true;
//         }
//     }
    
//     public void ShowLosePanel()
//     {
//         StartCoroutine(IEWaitToShowWinLose(false));
//     }
    
//     public void OnNextLevel()
//     {
//         // Check if player completed a level ahead of their progress
//         // If they completed a level that's higher than their current progress, update progress
//         if (currentPlayingLevel >= Utils.LEVEL_INDEX)
//         {
//             // Player completed current progress level or a future level
//             Utils.LEVEL_INDEX = currentPlayingLevel + 1;
//             Utils.SaveLevel();
//         }
        
//         Utils.SaveGameData();

//         int levelIndex = Utils.LEVEL_INDEX;
//         if (levelIndex > levelConfig.lstAllLevel.Count - 1)
//         {
//             List<int> tempResult = new List<int>();
//             for (int i = 0; i < levelConfig.lstAllLevel.Count; i++)
//             {
//                 if (!levelConfig.levelSkips.Contains(i))
//                 {
//                     tempResult.Add(i);
//                 }
//             }
//             var index = UnityEngine.Random.Range(0, tempResult.Count);
//             Utils.RealLevelIndex = tempResult[index];
//         }
//         else
//         {
//             Utils.RealLevelIndex = levelIndex;
//         }

//         ObjectPoolerManager.Instance.ClearAllPool();
//         SceneManager.LoadSceneAsync("MainGame");
//     }

//     public void On3xCoin()
//     {
// #if UNITY_EDITOR
//         Utils.currentCoin *= 3;
//         OnUpdateCoin();
//         OnNextLevel();
// #else
//         Utils.currentCoin += 3 * Utils.BASE_COIN;
//         OnUpdateCoin();
//         OnNextLevel();
// #endif
//     }
    
//     public void OnX2Coin()
//     {
// #if UNITY_EDITOR
//         Utils.currentCoin *= 3;
//         OnUpdateCoin();
//         OnNextLevel();
// #else
//         Utils.currentCoin += 3 * Utils.BASE_COIN;
//         OnUpdateCoin();
//         OnNextLevel();
// #endif
//     }

//     private void MultiplyCoins()
//     {
// #if UNITY_EDITOR
//         Utils.currentCoin *= 3;
// #else
//         Utils.currentCoin += 3 * Utils.BASE_COIN;
// #endif
//         Debug.Log("Coins multiplied! New total: " + Utils.currentCoin);
//     }
    
//     public void OnSkipByVideo()
//     {
//         Debug.Log("OnSkipByVideo clicked!");
        
//         if (Utils.currentCoin >= 100)
//         {
//             Debug.Log($"Skipping level. Coins before: {Utils.currentCoin}");
//             Utils.currentCoin -= 100;
//             OnUpdateCoin();
//             Debug.Log($"Coins after: {Utils.currentCoin}");
//             OnNextLevel();
//         }
//         else
//         {
//             Debug.LogError($"Not enough coins to skip level. Have {Utils.currentCoin}, need 100 coins.");
//         }
//     }

//     public void AddCoins(int amount)
//     {
//         Utils.currentCoin += amount;
//         OnUpdateCoin();
//         Debug.Log("Add " + amount + " coins. Total: " + Utils.currentCoin);
        
//         if (SoundManager.Instance != null)
//         {
//             SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
//         }
//     }

//     public void AddCoinsWithAd(int coinAmount)
//     {
//         if (AdManager.Instance != null)
//         {
//             AdManager.Instance.ShowRewardedAdForCoins(coinAmount, () =>
//             {
//                 Debug.Log("Successfully added coins after watching ad!");
//             });
//         }
//         else
//         {
//             Debug.LogError("AdManager not found!");
//             AddCoins(coinAmount);
//         }
//     }

//     public void OnReplay()
//     {
//         Utils.SaveGameData();
        
//         // Keep the same level - set it as selected level for replay
//         Utils.SetLevelToPlay(currentPlayingLevel);
        
//         if (ObjectPoolerManager.Instance != null)
//         {
//             ObjectPoolerManager.Instance.ClearAllPool();
//         }

//         SceneManager.LoadSceneAsync("MainGame");
//     }
    
//     public void GoToMenu()
//     {
//         Utils.SaveGameData();
        
//         if (ObjectPoolerManager.Instance != null)
//         {
//             ObjectPoolerManager.Instance.ClearAllPool();
//         }
        
//         SceneManager.LoadSceneAsync("MainMenu");
//     }
    
//     public void BtnAchievement()
//     {
//         Debug.Log("BtnAchievement clicked!");
        
//         // Don't clear pool or change scene - just set flag and go to menu
//         MenuController.openAchievement = true;
        
//         Utils.SaveGameData();
        
//         if (ObjectPoolerManager.Instance != null)
//         {
//             ObjectPoolerManager.Instance.ClearAllPool();
//         }
        
//         SceneManager.LoadSceneAsync("MainMenu");
//     }
    
//     public void BtnCastle()
//     {
//         if (ObjectPoolerManager.Instance != null)
//         {
//             ObjectPoolerManager.Instance.ClearAllPool();
//         }
        
//         MenuController.openCastle = true;
//         SceneManager.LoadSceneAsync("MainMenu");
//     }
    
//     public void BuyRemoveAds()
//     {
//         Debug.Log("Buy Remove Ads");
//     }

//     private void OnApplicationFocus(bool focus)
//     {
//         if (focus)
//         {
//             Utils.LoadGameData();
//             OnUpdateCoin();
//         }
//         else
//         {
//             Utils.SaveGameData();
//         }
//     }

//     public void SoundClickButton()
//     {
//         if (SoundManager.Instance != null)
//         {
//             SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
//         }
//     }
// }

// [System.Serializable]
// public class MissionType
// {
//     public MapLevelManager.QUEST_TYPE questType;
//     public Sprite spr_;
//     public string strQuest;
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    
    public GameObject effectCamera, BtnReplay, bouderCoinFly, btnx3Coin, btnTabNext, phaohoa, btnReplay2, btnSkipLevelLose, warningAchievment;
    public Sprite winSp, loseSp;
    public PhysicsMaterial2D matStone;
    public bool playerMove;
    public int counthatwater;
    public enum GAMESTATE { BEGIN, PLAYING, WIN, LOSE }
    public static GameManager Instance;
    public MissionType mSavePrincess, mCollect, mOpenChest, mKill;
    public Image imgQuestImage;
    public TextMeshProUGUI txtQuestText;

    public Text txtLevel, levelTextGameOver;
    public Text txtCoin;
    public Text txtCoinWin;
    public bool isTest;
    [HideInInspector] public bool canUseTrail;
    public GAMESTATE gameState;
    [SerializeField] public LevelConfig levelConfig;
    public MapLevelManager mapLevel;
    public int totalGems;
    public CamFollow _camFollow;
    public GameObject gPanelWin; // Changed from Image to GameObject

    [HideInInspector] public GameObject gTargetFollow;
    
    // Store the actual level being played for correct display
    private int currentPlayingLevel;
    
    public void LoseDisplay()
    {
        btnTabNext.SetActive(false);
        btnx3Coin.SetActive(false);
        phaohoa.SetActive(false);
        btnReplay2.SetActive(true);
        btnSkipLevelLose.SetActive(true);
        
        // Get the Image component and change sprite
        Image panelImage = gPanelWin.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.sprite = loseSp;
        }
    }
    
    public void CheckDisplayWarningAchievement()
    {
        if (DataController.instance != null)
            warningAchievment.SetActive(DataController.instance.CheckWarningAchievement());
    }
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void OnUpdateCoin()
    {
        txtCoin.text = Utils.currentCoin.ToString();
        Utils.SaveCoin();
    }
    
    public int coinTemp;
    
    void Start()
    {
        AdManager.Instance.LoadAd();
        Utils.LoadGameData();
        
        // Get the level to play (either selected or current progress)
        int levelToPlay = Utils.GetLevelToPlay();
        currentPlayingLevel = levelToPlay;
        
        // Clear the selected level key after reading it
        Utils.ClearSelectedLevel();
        
        // Display the correct level number
        levelTextGameOver.text = txtLevel.text = "LEVEL " + (currentPlayingLevel + 1).ToString();
        txtCoinWin.text = Utils.currentCoin.ToString();
        coinTemp = Utils.currentCoin;
        OnUpdateCoin();

        if (!isTest)
        {
            LoadLevelToPlay(levelToPlay);
            Utils.RealLevelIndex = levelToPlay;
        }
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBackgroundMusic();
        }
    
        CheckDisplayWarningAchievement();
        
        // Verify buttons are assigned
        if (btnSkipLevelLose != null)
        {
            Debug.Log("Skip button is assigned");
        }
        else
        {
            Debug.LogError("Skip button is NOT assigned in Inspector!");
        }
        
        if (warningAchievment != null)
        {
            Debug.Log("Achievement warning is assigned");
        }
        else
        {
            Debug.LogError("Achievement warning is NOT assigned in Inspector!");
        }
    }

    private void OnApplicationQuit()
    {
        Utils.SaveCoin();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Utils.SaveCoin();
        }
    }

    private void OnChange(Sprite _spr, string _text)
    {
        imgQuestImage.sprite = _spr;
        txtQuestText.text = "<color=#FFBC01> LEVEL " + (currentPlayingLevel + 1).ToString("0#") + "</color> " + _text.ToUpper();
    }
    
    public void OnInitQuestText(MapLevelManager.QUEST_TYPE _questType)
    {
        switch (_questType)
        {
            case MapLevelManager.QUEST_TYPE.COLLECT:
                OnChange(mCollect.spr_, mCollect.strQuest);
                break;
            case MapLevelManager.QUEST_TYPE.KILL:
                OnChange(mKill.spr_, mKill.strQuest);
                break;
            case MapLevelManager.QUEST_TYPE.OPEN_CHEST:
                OnChange(mOpenChest.spr_, mOpenChest.strQuest);
                break;
            case MapLevelManager.QUEST_TYPE.SAVE_HOSTAGE:
                OnChange(mSavePrincess.spr_, mSavePrincess.strQuest);
                break;
        }
    }

    private void LoadLevelToPlay(int realLevelIndex)
    {
        if (realLevelIndex < 0 || realLevelIndex >= levelConfig.lstAllLevel.Count)
        {
            Debug.LogError($"Invalid level index: {realLevelIndex}. Loading level 0 instead.");
            realLevelIndex = 0;
            currentPlayingLevel = 0;
        }
        
        MapLevelManager mapInstall = levelConfig.lstAllLevel[realLevelIndex];
        mapLevel = Instantiate(mapInstall, Vector3.zero, Quaternion.identity);
        
        Debug.Log($"Loading level: {realLevelIndex + 1}");
        
        if (mapLevel.lstAllStick.Count > 0)
            playerMove = true;
        if (mapLevel.waterObj != null)
            counthatwater = mapLevel.waterObj.gGems.Count;
    }

    private void ActiveCamEff()
    {
        _camFollow.objectToFollow = gTargetFollow;
        _camFollow.beginFollow = true;
    }
    
    public void ShowWinPanel()
    {
        Debug.Log("ShowWinPanel called!");
        StartCoroutine(IEWaitToShowWinLose(true));
    }
    
    public int enemyKill;
    static int countpasslevel;
    
    private IEnumerator IEWaitToShowWinLose(bool isWin)
    {
        yield return new WaitForSeconds(0.5f);
        
        Debug.Log($"IEWaitToShowWinLose: isWin={isWin}, panel active={gPanelWin.activeSelf}");
    
        if (isWin)
        {
            if (!gPanelWin.activeSelf)
            {
                Debug.Log("Showing win panel now!");
                
                ActiveCamEff();
                Utils.currentCoin += Utils.BASE_COIN;

                OnUpdateCoin();
                
                // Change sprite to win sprite
                Image panelImage = gPanelWin.GetComponent<Image>();
                if (panelImage != null)
                {
                    panelImage.sprite = winSp;
                }
                
                gPanelWin.SetActive(true);

                BtnReplay.SetActive(false);
                effectCamera.SetActive(false);
                
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.acWin);
                }

                if (DataController.instance != null)
                {
                    DataController.instance.DoAchievment(0, 1);

                    if (mapLevel.questType == MapLevelManager.QUEST_TYPE.SAVE_HOSTAGE)
                        DataController.instance.DoAchievment(2, 1);
                    else if (mapLevel.questType == MapLevelManager.QUEST_TYPE.OPEN_CHEST)
                        DataController.instance.DoAchievment(3, 1);
                    else if (mapLevel.questType == MapLevelManager.QUEST_TYPE.COLLECT)
                        DataController.instance.DoAchievment(1, 1);
                    
                    DataController.instance.DoAchievment(4, enemyKill);
                }

                if (DataParam.firsttime == 0)
                {
                    if (Utils.LEVEL_INDEX >= DataParam.levelpassshowad)
                    {
                        DataParam.firsttime = 1;
                        Debug.Log("Show ads TH 1");
                    }
                }
                else
                {
                    countpasslevel++;
                    if (countpasslevel >= DataParam.delayshowAds && 
                        (System.DateTime.Now - DataParam.oldTimeShowAds).TotalSeconds >= DataParam.timedelayShowAds)
                    {
                        countpasslevel = 0;
                        DataParam.oldTimeShowAds = System.DateTime.Now;
                    }
                    Debug.Log("Show ads TH 2");
                }
            }
            else
            {
                Debug.LogWarning("Win panel already active!");
            }
        }
        else
        {
            if (!gPanelWin.activeSelf)
            {
                Debug.Log("Showing lose panel now!");
                
                ActiveCamEff();
                gPanelWin.SetActive(true);
                effectCamera.SetActive(false);
                LoseDisplay();
                
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.acLose);
                }
            }
            else
            {
                Debug.LogWarning("Lose panel already active!");
            }
        }
    }
    
    private bool playingSoundLava = false;
    
    public void PlaySoundLavaOnWater()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.acStoneApear);
            if (!playingSoundLava)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.acLavaOnWater);
            }
            playingSoundLava = true;
        }
    }

    public void ShowLosePanel()
    {
        StartCoroutine(IEWaitToShowWinLose(false));
    }
    
    // public void OnNextLevel()
    // {
    //     Debug.Log($"OnNextLevel: Playing level {currentPlayingLevel + 1}, Progress level {Utils.LEVEL_INDEX + 1}");
        
    //     // Calculate next level (always go to the next sequential level)
    //     int nextLevel = currentPlayingLevel + 1;
        
    //     // Update progress if we're advancing beyond current progress
    //     if (nextLevel > Utils.LEVEL_INDEX)
    //     {
    //         Utils.LEVEL_INDEX = nextLevel;
    //         Utils.SaveLevel();
    //         Debug.Log($"Updated progress to level {Utils.LEVEL_INDEX + 1}");
    //     }
        
    //     Utils.SaveGameData();

    //     // Handle level wrap-around if we've completed all levels
    //     if (nextLevel > levelConfig.lstAllLevel.Count - 1)
    //     {
    //         List<int> tempResult = new List<int>();
    //         for (int i = 0; i < levelConfig.lstAllLevel.Count; i++)
    //         {
    //             if (!levelConfig.levelSkips.Contains(i))
    //             {
    //                 tempResult.Add(i);
    //             }
    //         }
    //         var index = UnityEngine.Random.Range(0, tempResult.Count);
    //         Utils.RealLevelIndex = tempResult[index];
    //     }
    //     else
    //     {
    //         Utils.RealLevelIndex = nextLevel;
    //     }

    //     ObjectPoolerManager.Instance.ClearAllPool();
    //     SceneManager.LoadSceneAsync("MainGame");
    // }
    
    
    public void OnNextLevel()
    {
        Debug.Log($"OnNextLevel: Playing level {currentPlayingLevel + 1}, Progress level {Utils.LEVEL_INDEX + 1}");
        
        // Calculate next level (always go to the next sequential level)
        int nextLevel = currentPlayingLevel + 1;
        
        // Update progress if we're advancing beyond current progress
        if (nextLevel > Utils.LEVEL_INDEX)
        {
            Utils.LEVEL_INDEX = nextLevel;
            Utils.SaveLevel();
            Debug.Log($"Updated progress to level {Utils.LEVEL_INDEX + 1}");
        }
        
        Utils.SaveGameData();
    
        // Handle level wrap-around if we've completed all levels
        if (nextLevel > levelConfig.lstAllLevel.Count - 1)
        {
            List<int> tempResult = new List<int>();
            for (int i = 0; i < levelConfig.lstAllLevel.Count; i++)
            {
                if (!levelConfig.levelSkips.Contains(i))
                {
                    tempResult.Add(i);
                }
            }
            var index = UnityEngine.Random.Range(0, tempResult.Count);
            nextLevel = tempResult[index];
        }
    
        // Set the level to play before loading the scene
        Utils.SetLevelToPlay(nextLevel);
    
        ObjectPoolerManager.Instance.ClearAllPool();
        SceneManager.LoadSceneAsync("MainGame");
    }

    public void On3xCoin()
    {
#if UNITY_EDITOR
        Utils.currentCoin *= 3;
        OnUpdateCoin();
        OnNextLevel();
#else
        Utils.currentCoin += 3 * Utils.BASE_COIN;
        OnUpdateCoin();
        OnNextLevel();
#endif
    }
    
    public void OnX2Coin()
    {
#if UNITY_EDITOR
        Utils.currentCoin *= 3;
        OnUpdateCoin();
        OnNextLevel();
#else
        Utils.currentCoin += 3 * Utils.BASE_COIN;
        OnUpdateCoin();
        OnNextLevel();
#endif
    }

    private void MultiplyCoins()
    {
#if UNITY_EDITOR
        Utils.currentCoin *= 3;
#else
        Utils.currentCoin += 3 * Utils.BASE_COIN;
#endif
        Debug.Log("Coins multiplied! New total: " + Utils.currentCoin);
    }
    
    public void OnSkipByVideo()
    {
        Debug.Log("OnSkipByVideo clicked!");
        
        if (Utils.currentCoin >= 100)
        {
            Debug.Log($"Skipping level. Coins before: {Utils.currentCoin}");
            Utils.currentCoin -= 100;
            OnUpdateCoin();
            Debug.Log($"Coins after: {Utils.currentCoin}");
            OnNextLevel();
        }
        else
        {
            Debug.LogError($"Not enough coins to skip level. Have {Utils.currentCoin}, need 100 coins.");
        }
    }

    public void AddCoins(int amount)
    {
        Utils.currentCoin += amount;
        OnUpdateCoin();
        Debug.Log("Add " + amount + " coins. Total: " + Utils.currentCoin);
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
        }
    }

    public void AddCoinsWithAd(int coinAmount)
    {
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowRewardedAdForCoins(coinAmount, () =>
            {
                Debug.Log("Successfully added coins after watching ad!");
            });
        }
        else
        {
            Debug.LogError("AdManager not found!");
            AddCoins(coinAmount);
        }
    }

    public void OnReplay()
    {
        Utils.SaveGameData();
        
        // Keep the same level - set it as selected level for replay
        Utils.SetLevelToPlay(currentPlayingLevel);
        
        if (ObjectPoolerManager.Instance != null)
        {
            ObjectPoolerManager.Instance.ClearAllPool();
        }

        SceneManager.LoadSceneAsync("MainGame");
    }
    
    public void GoToMenu()
    {
        Utils.SaveGameData();
        
        if (ObjectPoolerManager.Instance != null)
        {
            ObjectPoolerManager.Instance.ClearAllPool();
        }
        
        SceneManager.LoadSceneAsync("MainMenu");
    }
    
    public void BtnAchievement()
    {
        Debug.Log("BtnAchievement clicked!");
        
        // Don't clear pool or change scene - just set flag and go to menu
        MenuController.openAchievement = true;
        
        Utils.SaveGameData();
        
        if (ObjectPoolerManager.Instance != null)
        {
            ObjectPoolerManager.Instance.ClearAllPool();
        }
        
        SceneManager.LoadSceneAsync("MainMenu");
    }
    
    public void BtnCastle()
    {
        if (ObjectPoolerManager.Instance != null)
        {
            ObjectPoolerManager.Instance.ClearAllPool();
        }
        
        MenuController.openCastle = true;
        SceneManager.LoadSceneAsync("MainMenu");
    }
    
    public void BuyRemoveAds()
    {
        Debug.Log("Buy Remove Ads");
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Utils.LoadGameData();
            OnUpdateCoin();
        }
        else
        {
            Utils.SaveGameData();
        }
    }

    public void SoundClickButton()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
        }
    }
}

[System.Serializable]
public class MissionType
{
    public MapLevelManager.QUEST_TYPE questType;
    public Sprite spr_;
    public string strQuest;
}