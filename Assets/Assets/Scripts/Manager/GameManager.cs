
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
    //public List<MissionType> lstAllMission = new List<MissionType>();
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
    //  public List<Unit> lstAllGems = new List<Unit>();
    //  public bool isNotEnoughGems;
    public CamFollow _camFollow;
    public Image gPanelWin;
    //  public GameObject gPanelLose;

    [HideInInspector] public GameObject gTargetFollow;
    public void LoseDisplay()
    {
        btnTabNext.SetActive(false);
        btnx3Coin.SetActive(false);
        phaohoa.SetActive(false);
        btnReplay2.SetActive(true);
        btnSkipLevelLose.SetActive(true);
        gPanelWin.sprite = loseSp;
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
        txtCoin.text = Utils.currentCoin.ToString(/*"00,#"*/);
        //  txtCoinWin.text = Utils.currentCoin.ToString("00,#");
        Utils.SaveCoin();
    }
    public int coinTemp;
    void Start()
    {
        AdManager.Instance.LoadAd();
        Utils.LoadGameData();
        levelTextGameOver.text = txtLevel.text = "LEVEL " + (Utils.LEVEL_INDEX + 1).ToString(/*"00,#"*/);
        txtCoinWin.text = Utils.currentCoin.ToString(/*"00,#"*/);
        coinTemp = Utils.currentCoin;
        OnUpdateCoin();

        if (!isTest)
        {
            if (Utils.RealLevelIndex != 0)
            {
                LoadLevelToPlay(Utils.RealLevelIndex);
            }
            else
            {
                LoadLevelToPlay(Utils.LEVEL_INDEX);
            }
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBackgroundMusic();
        }
    
        CheckDisplayWarningAchievement();

       // MyAnalytic.EventLevelStart(Utils.LEVEL_INDEX + 1);
    }
    private void OnChange(Sprite _spr, string _text)
    {
        imgQuestImage.sprite = _spr;
        txtQuestText.text = "<color=#FFBC01> LEVEL " + (Utils.LEVEL_INDEX + 1).ToString("0#") + "</color> " + _text.ToUpper();
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
        MapLevelManager mapInstall = mapInstall = levelConfig.lstAllLevel[realLevelIndex];
      
        mapLevel = Instantiate(mapInstall, Vector3.zero, Quaternion.identity);
        Debug.LogError("wtf:" + mapLevel.loadListStick);
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
        StartCoroutine(IEWaitToShowWinLose(true));
    }
    public int enemyKill;
    static int countpasslevel;
    private IEnumerator IEWaitToShowWinLose(bool isWin)
    {
        yield return new WaitForSeconds(0.5f);
    
        if (isWin)
        {

            if (!gPanelWin.gameObject.activeSelf)
            {
                ActiveCamEff();
                Utils.currentCoin += Utils.BASE_COIN;

                OnUpdateCoin();
                gPanelWin.gameObject.SetActive(true);

                BtnReplay.SetActive(false);
                effectCamera.SetActive(false);
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.acWin);
                }

                if (DataController.instance != null)
                    DataController.instance.DoAchievment(0, 1);

                if (mapLevel.questType == MapLevelManager.QUEST_TYPE.SAVE_HOSTAGE)
                {
                    if (DataController.instance != null)
                        DataController.instance.DoAchievment(2, 1);
                }
                else if (mapLevel.questType == MapLevelManager.QUEST_TYPE.OPEN_CHEST)
                {
                    if (DataController.instance != null)
                        DataController.instance.DoAchievment(3, 1);
                }
                else if (mapLevel.questType == MapLevelManager.QUEST_TYPE.COLLECT)
                {
                    if (DataController.instance != null)
                        DataController.instance.DoAchievment(1, 1);
                }
                if (DataController.instance != null)
                    DataController.instance.DoAchievment(4, enemyKill);

                if (DataParam.firsttime == 0)
                {
                    if (Utils.LEVEL_INDEX >= DataParam.levelpassshowad)
                    {
                      
                        DataParam.firsttime = 1;
                        Debug.LogError("========show ads TH 1");
                    }
                }
                else
                {
                    countpasslevel++;
                    if (countpasslevel >= DataParam.delayshowAds && (System.DateTime.Now - DataParam.oldTimeShowAds).TotalSeconds >= DataParam.timedelayShowAds)
                    {
                        countpasslevel = 0;
                        DataParam.oldTimeShowAds = System.DateTime.Now;
                       
                    }
                    Debug.LogError("========show ads TH 2");
                }
              //  MyAnalytic.EventLevelCompleted(Utils.LEVEL_INDEX + 1);
            }
        }
        else
        {

            if (!gPanelWin.gameObject.activeSelf)
            {
                ActiveCamEff();
                gPanelWin.gameObject.SetActive(true);
                effectCamera.SetActive(false);
                LoseDisplay();
             //   countpasslevel = 0;
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.acLose);
                }
                //MyAnalytic.EventLevelFailed(Utils.LEVEL_INDEX + 1);
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
    public void OnNextLevel()
    {
        Utils.SaveGameData(); // Add this line
        Utils.LEVEL_INDEX += 1;
        Utils.SaveLevel();

        int levelIndex = Utils.LEVEL_INDEX;
        if (levelIndex > levelConfig.lstAllLevel.Count - 1)
        {
            List<int> tempResult = new List<int>();
            for (int i = 0; i < levelConfig.lstAllLevel.Count; i++)
            {
                if (!levelConfig.levelSkips.Contains(i))
                {
                    tempResult.Add(i);
                }
            }
            var index = Random.Range(0, tempResult.Count);
            Utils.RealLevelIndex = tempResult[index];
        }
        else
        {
            Utils.RealLevelIndex = levelIndex;
        }

        /*        if (Utils.LEVEL_INDEX < levelConfig.lstAllLevel.Count - 1)
                {
                    Utils.LEVEL_INDEX += 1;
                    Utils.SaveLevel();
                }
                else
                {
                    Utils.LEVEL_INDEX = 0;
                    Utils.SaveLevel();
                }*/

        ObjectPoolerManager.Instance.ClearAllPool();
        SceneManager.LoadSceneAsync("MainGame");
    }

    public void On3xCoin()
    {
#if UNITY_EDITOR
        Utils.currentCoin *= 3 /** Utils.BASE_COIN*/;
        OnUpdateCoin();
        OnNextLevel();
#else
                    Utils.currentCoin += 3 * Utils.BASE_COIN;
                    OnUpdateCoin();
                    OnNextLevel();
#endif
        //    Debug.LogError("X2 Coin");
    }
    public void OnX2Coin()
    {
#if UNITY_EDITOR
        Utils.currentCoin *= 3 /** Utils.BASE_COIN*/;
        OnUpdateCoin();
        OnNextLevel();
#else
                    Utils.currentCoin += 3 * Utils.BASE_COIN;
                    OnUpdateCoin();
                    OnNextLevel();
#endif
        //    Debug.LogError("X2 Coin");
        // // Check if AdManager exists
        // if (AdManager.Instance != null)
        // {
        //     // Show rewarded ad first, then multiply coins
        //     AdManager.Instance.ShowRewardedAdFor3xCoins(0, () =>
        //     {
        //         // // This runs AFTER user watches the ad
        //         // MultiplyCoins();
        //         // OnUpdateCoin();
        //         // OnNextLevel();
        //     });
        // }
        // else
        // {
        //     // // No ads available, just multiply coins
        //     // MultiplyCoins();
        //     // OnUpdateCoin();
        //     // OnNextLevel();
        //     On3xCoin();
        // }
    }

    // Simple method to multiply coins
    private void MultiplyCoins()
    {
    #if UNITY_EDITOR
        Utils.currentCoin *= 3;  // Multiply by 3 in editor
    #else
        Utils.currentCoin += 3 * Utils.BASE_COIN;  // Add bonus in build
    #endif
        Debug.Log("Coins multiplied! New total: " + Utils.currentCoin);
    }
    public void OnSkipByVideo()
    {
        if (Utils.currentCoin >= 100)
        {
            Utils.currentCoin -= 100;
            OnUpdateCoin();
            OnNextLevel();
        }
        else
        {
            Debug.LogError("Not enough coins to skip level. Need 100 coins.");
        }

// #if UNITY_EDITOR
//         OnNextLevel();
// #else

//                     OnNextLevel();
// #endif

    }

    public void AddCoins(int amount)
    {
        Utils.currentCoin += amount;
        OnUpdateCoin();
        Debug.LogError("Add " + amount + " coins. Total: " + Utils.currentCoin);
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.acClick);
        }
    }

    // Add this to your GameManager class
    public void AddCoinsWithAd(int coinAmount)
    {
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowRewardedAdForCoins(coinAmount, () =>
            {
                Debug.Log("Successfully added coins after watching ad!");
                // Optional: Show success message to player
            });
        }
        else
        {
            Debug.LogError("AdManager not found!");
            // Fallback: Add coins directly if no ads
            AddCoins(coinAmount);
        }
    }


    public void OnReplay()
    {
        Utils.SaveGameData(); // Add this line
        if (ObjectPoolerManager.Instance != null)
        {
            ObjectPoolerManager.Instance.ClearAllPool();
        }

        SceneManager.LoadSceneAsync("MainGame");
    }
    public void GoToMenu()
    {
        Utils.SaveGameData(); // Add this line
        if (ObjectPoolerManager.Instance != null)
        {
            ObjectPoolerManager.Instance.ClearAllPool();
        }
        SceneManager.LoadSceneAsync("MainMenu");
    }
    public void BtnAchievement()
    {
        if (ObjectPoolerManager.Instance != null)
        {
            ObjectPoolerManager.Instance.ClearAllPool();
        }
        MenuController.openAchievement = true;
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
        Debug.LogError("Buy Remove Ads");
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
