// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Utils
// {
//     public enum QUAL_IMAGE { VERY_LOW = 0, LOW = 1, MEDIUM = 2, HEIGH = 3, VERY_HEIGH = 4, ULTRA = 5 }
//     public QUAL_IMAGE _quality;
//     public const string QUAL_VERY_HEIGHT = "Very High";
//     public const string QUAL_HEIGHT = "High";
//     public const string QUAL_MEDIUM = "Medium";
//     public const string QUAL_LOW = "Low";

//     public const string INAPP_REMOVE_ADS = "com.hero.rescue.pull.the.pin.puzzle.free.action";
//     public const string APP_ID = "ca-app-pub-3422892225046579~5148518811";
//     public const string BANNER_ID = "ca-app-pub-3422892225046579/5665928525";
//     public const string INTERS_ID = "ca-app-pub-3422892225046579/1017702114";
//     public const string VIDEO_ID = "ca-app-pub-3422892225046579/4367604393";
//     public const int BASE_COIN = 100;
//     private const string GAME_KEY = "com.hero.rescue.pull.the.pin.puzzle.free.action";
//     public const string COIN_KEY = GAME_KEY + ".coin";
//     public const string LEVEL_KEY = GAME_KEY + ".level";
//     public const string QUALITY_IMAGE = GAME_KEY + ".quality.image";
//     public const string CHANGE_SOUND = GAME_KEY + ".change.sound";
//     public const string CHANGE_MUSIC = GAME_KEY + ".change.music";
//     public const string CHANGE_VIBRATE = GAME_KEY + ".change.vibrate";
//     public const string HAS_REMOVE_ADS = GAME_KEY + ".removeads";
//     public const string KEY_DAILY_REWARD = GAME_KEY + ".KEY_DAILY_REWARD";
//     public const string KEY_CURRENT_DAILY_GIFT = GAME_KEY + ".KEY_CURRENT_DAILY_GIFT";
//     public const string KEY_PLAYER_SKIN = GAME_KEY + ".player.skin";
//     public const string KEY_HERO_SELECTED = GAME_KEY + ".hero.selected";
//     public const string KEY_SKIN_NORMAL = GAME_KEY + ".skin.hero.normal";
//     public const string KEY_SKIN_SWORD = GAME_KEY + ".skin.hero.sword";
//     public const string SELECTED_LEVEL_KEY = GAME_KEY + ".selected.level";

//     public const string TAG_STICKBARRIE = "StickBarrie";
//     public const string TAG_LAVA = "Trap_Lava";
//     public const string TAG_GAS = "Trap_Gas";
//     public const string TAG_WIN = "Tag_Win";
//     public const string TAG_STONE = "Tag_Stone";
//     public const string TAG_CHEST = "Chest";
//     public const string TAG_WALL_BOTTOM = "Wall_Bottom";
//     public const string TAG_SWORD = "Sword";

//     public static int LEVEL_INDEX = 0;
//     public static int currentCoin = 0;

//     public const string REAL_INDEX_LEVEL_PLAY = "real_index_level_play";

//     public static int RealLevelIndex 
//     { 
//         get => PlayerPrefs.GetInt(REAL_INDEX_LEVEL_PLAY, 0);
//         set => PlayerPrefs.SetInt(REAL_INDEX_LEVEL_PLAY, value);
//     }
      
//     public static void SaveCoin()
//     {
//         PlayerPrefs.SetInt(COIN_KEY, currentCoin);
//         PlayerPrefs.Save();
//         Debug.Log("Coins saved: " + currentCoin);
//     }
    
//     public static void SaveLevel()
//     {
//         PlayerPrefs.SetInt(LEVEL_KEY, LEVEL_INDEX);
//         PlayerPrefs.Save();
//         Debug.Log("Level saved: " + LEVEL_INDEX);
//     }
    
//     public static void SaveGameData()
//     {
//         SaveCoin();
//         SaveLevel();
//     }

//     public static void LoadCoin()
//     {
//         currentCoin = PlayerPrefs.GetInt(COIN_KEY, 0);
//         Debug.Log("Coins loaded: " + currentCoin);
//     }
    
//     public static void LoadGameData()
//     {
//         LEVEL_INDEX = PlayerPrefs.GetInt(LEVEL_KEY, 0);
//         currentCoin = PlayerPrefs.GetInt(COIN_KEY, 0);
//         useMediumImage = PlayerPrefs.GetInt(QUALITY_IMAGE, 0) == 0 ? false : true;
//         isSoundOn = PlayerPrefs.GetInt(CHANGE_SOUND, 1) == 0 ? false : true;
//         isMusicOn = PlayerPrefs.GetInt(CHANGE_MUSIC, 1) == 0 ? false : true;
//         isVibrateOn = PlayerPrefs.GetInt(CHANGE_VIBRATE, 0) == 0 ? false : true;
//         isRemoveAds = PlayerPrefs.GetInt(HAS_REMOVE_ADS, 0) == 0 ? false : true;

//         skinSword = GetCurSkinSword();

//         curDailyGift = PlayerPrefs.GetInt(KEY_CURRENT_DAILY_GIFT, 1);
//         if (curDailyGift > 7)
//         {
//             curDailyGift = 1;
//         }
//     }

//     #region Level Selection
//     /// <summary>
//     /// Gets the level index that should be loaded for gameplay.
//     /// This checks if player selected a specific level from the level panel,
//     /// otherwise returns the current progress level (LEVEL_INDEX).
//     /// </summary>
//     public static int GetLevelToPlay()
//     {
//         // Check if player selected a specific level from the level panel
//         if (PlayerPrefs.HasKey(SELECTED_LEVEL_KEY))
//         {
//             int selectedLevel = PlayerPrefs.GetInt(SELECTED_LEVEL_KEY, LEVEL_INDEX);
//             Debug.Log($"Loading selected level: {selectedLevel + 1}");
            
//             // Clear the selection after reading it (one-time use)
//             PlayerPrefs.DeleteKey(SELECTED_LEVEL_KEY);
//             PlayerPrefs.Save();
            
//             return selectedLevel;
//         }
        
//         // Default: load the current progress level
//         Debug.Log($"Loading progress level: {LEVEL_INDEX + 1}");
//         return LEVEL_INDEX;
//     }
    
//     /// <summary>
//     /// Sets the specific level the player wants to play next.
//     /// This is used by the level selection panel.
//     /// </summary>
//     public static void SetLevelToPlay(int levelIndex)
//     {
//         PlayerPrefs.SetInt(SELECTED_LEVEL_KEY, levelIndex);
//         PlayerPrefs.Save();
//         Debug.Log($"Set level to play: {levelIndex + 1}");
//     }
//     #endregion

//     public static bool useMediumImage;
//     public static void SaveImageSeting()
//     {
//         PlayerPrefs.SetInt(QUALITY_IMAGE, useMediumImage ? 1 : 0);
//         PlayerPrefs.Save();
//     }
    
//     public static bool isSoundOn;
//     public static void ChangeSound()
//     {
//         PlayerPrefs.SetInt(CHANGE_SOUND, isSoundOn ? 1 : 0);
//         PlayerPrefs.Save();
//     }
    
//     public static bool isMusicOn;
//     public static void ChangeMusic()
//     {
//         PlayerPrefs.SetInt(CHANGE_MUSIC, isMusicOn ? 1 : 0);
//         PlayerPrefs.Save();
//     }
    
//     public static bool isVibrateOn;
//     public static void ChangeVibrate()
//     {
//         PlayerPrefs.SetInt(CHANGE_VIBRATE, isVibrateOn ? 1 : 0);
//         PlayerPrefs.Save();
//     }

//     public static bool isRemoveAds;
//     public static void SaveRemoveAds()
//     {
//         PlayerPrefs.SetInt(HAS_REMOVE_ADS, isRemoveAds ? 1 : 0);
//         PlayerPrefs.Save();
//     }

//     #region Daily reward
//     public static bool IsClaimReward()
//     {
//         string _key = System.DateTime.Now.Day + "_" + System.DateTime.Now.Month;
//         return _key.Equals(SReward());
//     }
    
//     public static string SReward()
//     {
//         return PlayerPrefs.GetString(KEY_DAILY_REWARD, "");
//     }
    
//     public static void HasClaimReward()
//     {
//         string _key = System.DateTime.Now.Day + "_" + System.DateTime.Now.Month;
//         PlayerPrefs.SetString(KEY_DAILY_REWARD, _key);
//         PlayerPrefs.Save();
//     }
    
//     public static int curDailyGift;
//     public static bool cantakegiftdaily;
    
//     public static void SaveDailyGift()
//     {
//         PlayerPrefs.SetInt(KEY_CURRENT_DAILY_GIFT, curDailyGift);
//         PlayerPrefs.Save();
//     }
//     #endregion

//     #region Player Skin
//     public static string skinSword = "";

//     public static void SetSkinNormal(string skinName)
//     {
//         PlayerPrefs.SetString(KEY_SKIN_NORMAL, skinName);
//         PlayerPrefs.Save();
//     }
    
//     public static string GetCurSkinSword()
//     {
//         return PlayerPrefs.GetString(KEY_SKIN_SWORD, "kiem");
//     }
    
//     public static void SetSkinSword(string skinName)
//     {
//         PlayerPrefs.SetString(KEY_SKIN_SWORD, skinName);
//         PlayerPrefs.Save();
//     }
//     #endregion
// }






using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public enum QUAL_IMAGE { VERY_LOW = 0, LOW = 1, MEDIUM = 2, HEIGH = 3, VERY_HEIGH = 4, ULTRA = 5 }
    public QUAL_IMAGE _quality;
    public const string QUAL_VERY_HEIGHT = "Very High";
    public const string QUAL_HEIGHT = "High";
    public const string QUAL_MEDIUM = "Medium";
    public const string QUAL_LOW = "Low";

    public const string INAPP_REMOVE_ADS = "com.hero.rescue.pull.the.pin.puzzle.free.action";
    public const string APP_ID = "ca-app-pub-3422892225046579~5148518811";
    public const string BANNER_ID = "ca-app-pub-3422892225046579/5665928525";
    public const string INTERS_ID = "ca-app-pub-3422892225046579/1017702114";
    public const string VIDEO_ID = "ca-app-pub-3422892225046579/4367604393";
    public const int BASE_COIN = 100;
    private const string GAME_KEY = "com.hero.rescue.pull.the.pin.puzzle.free.action";
    public const string COIN_KEY = GAME_KEY + ".coin";
    public const string LEVEL_KEY = GAME_KEY + ".level";
    public const string QUALITY_IMAGE = GAME_KEY + ".quality.image";
    public const string CHANGE_SOUND = GAME_KEY + ".change.sound";
    public const string CHANGE_MUSIC = GAME_KEY + ".change.music";
    public const string CHANGE_VIBRATE = GAME_KEY + ".change.vibrate";
    public const string HAS_REMOVE_ADS = GAME_KEY + ".removeads";
    public const string KEY_DAILY_REWARD = GAME_KEY + ".KEY_DAILY_REWARD";
    public const string KEY_CURRENT_DAILY_GIFT = GAME_KEY + ".KEY_CURRENT_DAILY_GIFT";
    public const string KEY_PLAYER_SKIN = GAME_KEY + ".player.skin";
    public const string KEY_HERO_SELECTED = GAME_KEY + ".hero.selected";
    public const string KEY_SKIN_NORMAL = GAME_KEY + ".skin.hero.normal";
    public const string KEY_SKIN_SWORD = GAME_KEY + ".skin.hero.sword";
    public const string SELECTED_LEVEL_KEY = GAME_KEY + ".selected.level";

    public const string TAG_STICKBARRIE = "StickBarrie";
    public const string TAG_LAVA = "Trap_Lava";
    public const string TAG_GAS = "Trap_Gas";
    public const string TAG_WIN = "Tag_Win";
    public const string TAG_STONE = "Tag_Stone";
    public const string TAG_CHEST = "Chest";
    public const string TAG_WALL_BOTTOM = "Wall_Bottom";
    public const string TAG_SWORD = "Sword";

    public static int LEVEL_INDEX = 0;
    public static int currentCoin = 0;

    public const string REAL_INDEX_LEVEL_PLAY = "real_index_level_play";

    public static int RealLevelIndex 
    { 
        get => PlayerPrefs.GetInt(REAL_INDEX_LEVEL_PLAY, 0);
        set => PlayerPrefs.SetInt(REAL_INDEX_LEVEL_PLAY, value);
    }
      
    public static void SaveCoin()
    {
        PlayerPrefs.SetInt(COIN_KEY, currentCoin);
        PlayerPrefs.Save();
        Debug.Log("Coins saved: " + currentCoin);
    }
    
    public static void SaveLevel()
    {
        PlayerPrefs.SetInt(LEVEL_KEY, LEVEL_INDEX);
        PlayerPrefs.Save();
        Debug.Log("Level saved: " + LEVEL_INDEX);
    }
    
    public static void SaveGameData()
    {
        SaveCoin();
        SaveLevel();
    }

    public static void LoadCoin()
    {
        currentCoin = PlayerPrefs.GetInt(COIN_KEY, 0);
        Debug.Log("Coins loaded: " + currentCoin);
    }
    
    public static void LoadGameData()
    {
        LEVEL_INDEX = PlayerPrefs.GetInt(LEVEL_KEY, 0);
        currentCoin = PlayerPrefs.GetInt(COIN_KEY, 0);
        useMediumImage = PlayerPrefs.GetInt(QUALITY_IMAGE, 0) == 0 ? false : true;
        isSoundOn = PlayerPrefs.GetInt(CHANGE_SOUND, 1) == 0 ? false : true;
        isMusicOn = PlayerPrefs.GetInt(CHANGE_MUSIC, 1) == 0 ? false : true;
        isVibrateOn = PlayerPrefs.GetInt(CHANGE_VIBRATE, 0) == 0 ? false : true;
        isRemoveAds = PlayerPrefs.GetInt(HAS_REMOVE_ADS, 0) == 0 ? false : true;

        skinSword = GetCurSkinSword();

        curDailyGift = PlayerPrefs.GetInt(KEY_CURRENT_DAILY_GIFT, 1);
        if (curDailyGift > 7)
        {
            curDailyGift = 1;
        }
    }

    #region Level Selection
    /// <summary>
    /// Gets the level index that should be loaded for gameplay.
    /// This checks if player selected a specific level from the level panel,
    /// otherwise returns the current progress level (LEVEL_INDEX).
    /// </summary>
    public static int GetLevelToPlay()
    {
        // Check if player selected a specific level from the level panel
        if (PlayerPrefs.HasKey(SELECTED_LEVEL_KEY))
        {
            int selectedLevel = PlayerPrefs.GetInt(SELECTED_LEVEL_KEY, LEVEL_INDEX);
            Debug.Log($"Loading selected level: {selectedLevel + 1}");
            
            // DON'T delete here - let GameManager decide when to clear it
            return selectedLevel;
        }
        
        // Default: load the current progress level
        Debug.Log($"Loading progress level: {LEVEL_INDEX + 1}");
        return LEVEL_INDEX;
    }
    
    /// <summary>
    /// Sets the specific level the player wants to play next.
    /// This is used by the level selection panel.
    /// </summary>
    public static void SetLevelToPlay(int levelIndex)
    {
        PlayerPrefs.SetInt(SELECTED_LEVEL_KEY, levelIndex);
        PlayerPrefs.Save();
        Debug.Log($"Set level to play: {levelIndex + 1}");
    }
    
    /// <summary>
    /// Clears the selected level, causing the game to load progression level next time.
    /// </summary>
    public static void ClearSelectedLevel()
    {
        if (PlayerPrefs.HasKey(SELECTED_LEVEL_KEY))
        {
            PlayerPrefs.DeleteKey(SELECTED_LEVEL_KEY);
            PlayerPrefs.Save();
            Debug.Log("Cleared selected level");
        }
    }
    #endregion

    public static bool useMediumImage;
    public static void SaveImageSeting()
    {
        PlayerPrefs.SetInt(QUALITY_IMAGE, useMediumImage ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public static bool isSoundOn;
    public static void ChangeSound()
    {
        PlayerPrefs.SetInt(CHANGE_SOUND, isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public static bool isMusicOn;
    public static void ChangeMusic()
    {
        PlayerPrefs.SetInt(CHANGE_MUSIC, isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public static bool isVibrateOn;
    public static void ChangeVibrate()
    {
        PlayerPrefs.SetInt(CHANGE_VIBRATE, isVibrateOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool isRemoveAds;
    public static void SaveRemoveAds()
    {
        PlayerPrefs.SetInt(HAS_REMOVE_ADS, isRemoveAds ? 1 : 0);
        PlayerPrefs.Save();
    }

    #region Daily reward
    public static bool IsClaimReward()
    {
        string _key = System.DateTime.Now.Day + "_" + System.DateTime.Now.Month;
        return _key.Equals(SReward());
    }
    
    public static string SReward()
    {
        return PlayerPrefs.GetString(KEY_DAILY_REWARD, "");
    }
    
    public static void HasClaimReward()
    {
        string _key = System.DateTime.Now.Day + "_" + System.DateTime.Now.Month;
        PlayerPrefs.SetString(KEY_DAILY_REWARD, _key);
        PlayerPrefs.Save();
    }
    
    public static int curDailyGift;
    public static bool cantakegiftdaily;
    
    public static void SaveDailyGift()
    {
        PlayerPrefs.SetInt(KEY_CURRENT_DAILY_GIFT, curDailyGift);
        PlayerPrefs.Save();
    }
    #endregion

    #region Player Skin
    public static string skinSword = "";

    public static void SetSkinNormal(string skinName)
    {
        PlayerPrefs.SetString(KEY_SKIN_NORMAL, skinName);
        PlayerPrefs.Save();
    }
    
    public static string GetCurSkinSword()
    {
        return PlayerPrefs.GetString(KEY_SKIN_SWORD, "kiem");
    }
    
    public static void SetSkinSword(string skinName)
    {
        PlayerPrefs.SetString(KEY_SKIN_SWORD, skinName);
        PlayerPrefs.Save();
    }
    #endregion
}