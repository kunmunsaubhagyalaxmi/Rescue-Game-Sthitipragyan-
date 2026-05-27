// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using TMPro;

// public class LevelManager : MonoBehaviour
// {
//     [Header("Level Panel References")]
//     public GameObject levelPanel;
//     public Transform levelButtonContainer;
//     public GameObject levelButtonPrefab;

//     [Header("UI Elements")]
//     public Sprite lockedSprite;
//     public Color lockedColor = Color.gray;
//     public Color unlockedColor = Color.white;

//     private List<GameObject> levelButtons = new List<GameObject>();
//     private LevelConfig levelConfig;

//     public void Initialize(LevelConfig config)
//     {
//         levelConfig = config;
//         GenerateLevelButtons();
//     }

//     public void ShowLevelPanel()
//     {
//         if (MenuController.instance.animLoading)
//             return;

//         levelPanel.SetActive(true);
//         RefreshLevelButtons();
//         MenuController.instance.SoundClickButton();
//     }

//     public void HideLevelPanel()
//     {
//         levelPanel.SetActive(false);
//         MenuController.instance.SoundClickButton();
//     }

//     private void GenerateLevelButtons()
//     {
//         foreach (GameObject btn in levelButtons)
//         {
//             Destroy(btn);
//         }
//         levelButtons.Clear();

//         if (levelConfig == null || levelConfig.lstAllLevel == null)
//         {
//             Debug.LogError("Level config is not assigned or has no levels!");
//             return;
//         }

//         for (int i = 0; i < levelConfig.lstAllLevel.Count; i++)
//         {
//             GameObject btnObj = Instantiate(levelButtonPrefab, levelButtonContainer);
//             levelButtons.Add(btnObj);

//             SetupLevelButton(btnObj, i);
//         }
//         StartCoroutine(RebuildLayoutNextFrame());
//     }

//     private IEnumerator RebuildLayoutNextFrame()
//     {
//         yield return null; // Wait one frame
//         ForceRebuildLayout();
//     }

//     private void SetupLevelButton(GameObject btnObj, int levelIndex)
//     {
//         Button btn = btnObj.GetComponent<Button>();
//         TextMeshProUGUI levelText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
//         Image btnImage = btnObj.GetComponentInChildren<Image>();

//         GameObject lockIcon = FindLockIcon(btnObj.transform);

//         btn.onClick.RemoveAllListeners();

//         bool isUnlocked = levelIndex <= Utils.LEVEL_INDEX;

//         Debug.Log($"Setting up button {levelIndex + 1}, isUnlocked: {isUnlocked}, Utils.LEVEL_INDEX: {Utils.LEVEL_INDEX}");

//         if (isUnlocked)
//         {
//             if (levelText != null)
//             {
//                 levelText.text = (levelIndex + 1).ToString();
//                 levelText.gameObject.SetActive(true);
//             }
//             else
//             {
//                 Debug.LogWarning($"Level text component not found for button {levelIndex + 1}");
//             }

//             if (lockIcon != null)
//             {
//                 lockIcon.SetActive(false);
//             }

//             btnImage.color = unlockedColor;
//             btn.interactable = true;

//             int capturedIndex = levelIndex;
//             btn.onClick.AddListener(() => OnLevelButtonClicked(capturedIndex));
//         }
//         else
//         {
//             if (levelText != null)
//             {
//                 levelText.gameObject.SetActive(false);
//             }

//             if (lockIcon != null)
//             {
//                 lockIcon.SetActive(true);
//             }

//             if (lockedSprite != null)
//             {
//                 btnImage.sprite = lockedSprite;
//             }
//             btnImage.color = lockedColor;
//             btn.interactable = false;
//         }
//     }

//     private GameObject FindLockIcon(Transform parent)
//     {
//         string[] possibleNames = { "LockIcon", "lockicon", "Lock", "lock", "LockImage" };

//         foreach (string name in possibleNames)
//         {
//             Transform found = parent.Find(name);
//             if (found != null)
//             {
//                 return found.gameObject;
//             }
//         }

//         foreach (Transform child in parent)
//         {
//             if (child.name.ToLower().Contains("lock"))
//             {
//                 return child.gameObject;
//             }
//         }

//         return null;
//     }

//     private void RefreshLevelButtons()
//     {
//         for (int i = 0; i < levelButtons.Count; i++)
//         {
//             GameObject btnObj = levelButtons[i];
//             if (btnObj == null) continue;

//             SetupLevelButton(btnObj, i);
//         }
//     }

//     private void OnLevelButtonClicked(int levelIndex)
//     {
//         Debug.Log($"Level button clicked! Loading level: {levelIndex + 1}");

//         Utils.SetLevelToPlay(levelIndex);

//         MenuController.instance.LoadScenePlay();

//         HideLevelPanel();
//     }

//     public void OnBattleButtonClicked()
//     {
//         MenuController.instance.LoadScenePlay();
//     }
    
//     private void ForceRebuildLayout()
//     {
//         // Force the layout to rebuild
//         LayoutRebuilder.ForceRebuildLayoutImmediate(levelButtonContainer.GetComponent<RectTransform>());

//         // Also rebuild the scroll view
//         if (levelPanel != null)
//         {
//             ScrollRect scrollRect = levelPanel.GetComponentInChildren<ScrollRect>();
//             if (scrollRect != null)
//             {
//                 Canvas.ForceUpdateCanvases();
//                 scrollRect.verticalNormalizedPosition = 1f; // Scroll to top
//             }
//         }
//     }
// }




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Level Panel References")]
    public GameObject levelPanel;
    public Transform levelButtonContainer;
    public GameObject levelButtonPrefab;

    [Header("UI Elements")]
    public Sprite lockedSprite;
    
    [Header("Text Colors")]
    public Color lockedTextColor = new Color(0.5f, 0.5f, 0.5f, 0.7f); // Dimmed text for locked levels
    public Color unlockedTextColor = Color.white; // Bright text for unlocked levels

    private List<GameObject> levelButtons = new List<GameObject>();
    private LevelConfig levelConfig;

    public void Initialize(LevelConfig config)
    {
        levelConfig = config;
        GenerateLevelButtons();
    }

    public void ShowLevelPanel()
    {
        if (MenuController.instance.animLoading)
            return;

        levelPanel.SetActive(true);
        RefreshLevelButtons();
        MenuController.instance.SoundClickButton();
    }

    public void HideLevelPanel()
    {
        levelPanel.SetActive(false);
        MenuController.instance.SoundClickButton();
    }

    private void GenerateLevelButtons()
    {
        foreach (GameObject btn in levelButtons)
        {
            Destroy(btn);
        }
        levelButtons.Clear();

        if (levelConfig == null || levelConfig.lstAllLevel == null)
        {
            Debug.LogError("Level config is not assigned or has no levels!");
            return;
        }

        for (int i = 0; i < levelConfig.lstAllLevel.Count; i++)
        {
            GameObject btnObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            levelButtons.Add(btnObj);

            SetupLevelButton(btnObj, i);
        }
        
        // Force layout rebuild after generating buttons
        StartCoroutine(RebuildLayoutNextFrame());
    }

    private IEnumerator RebuildLayoutNextFrame()
    {
        yield return null; // Wait one frame
        ForceRebuildLayout();
    }

    private void SetupLevelButton(GameObject btnObj, int levelIndex)
    {
        Button btn = btnObj.GetComponent<Button>();
        TextMeshProUGUI levelText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        
        // Find the lock icon (child of the button)
        GameObject lockIcon = FindLockIcon(btnObj.transform);

        btn.onClick.RemoveAllListeners();

        bool isUnlocked = levelIndex <= Utils.LEVEL_INDEX;

        Debug.Log($"Setting up button {levelIndex + 1}, isUnlocked: {isUnlocked}, Utils.LEVEL_INDEX: {Utils.LEVEL_INDEX}");

        // ALWAYS show the level number text
        if (levelText != null)
        {
            levelText.text = (levelIndex + 1).ToString();
            levelText.gameObject.SetActive(true);
            
            // Change text color based on lock status
            levelText.color = isUnlocked ? unlockedTextColor : lockedTextColor;
        }
        else
        {
            Debug.LogWarning($"Level text component not found for button {levelIndex + 1}");
        }

        if (isUnlocked)
        {
            // Unlocked level - HIDE lock icon only
            if (lockIcon != null)
            {
                lockIcon.SetActive(false);
            }
            
            btn.interactable = true;

            int capturedIndex = levelIndex;
            btn.onClick.AddListener(() => OnLevelButtonClicked(capturedIndex));
        }
        else
        {
            // Locked level - SHOW lock icon on top
            if (lockIcon != null)
            {
                lockIcon.SetActive(true);
                
                // Make sure lock icon is rendered on top of everything
                lockIcon.transform.SetAsLastSibling();
            }
            else
            {
                Debug.LogWarning($"Lock icon not found for button {levelIndex + 1}");
            }
            
            btn.interactable = false;
        }
    }

    private GameObject FindLockIcon(Transform parent)
    {
        string[] possibleNames = { "LockIcon", "lockicon", "Lock", "lock", "LockImage", "lock image", "Lock Image" };

        foreach (string name in possibleNames)
        {
            Transform found = parent.Find(name);
            if (found != null)
            {
                return found.gameObject;
            }
        }

        // Search recursively in all children
        foreach (Transform child in parent)
        {
            if (child.name.ToLower().Contains("lock"))
            {
                return child.gameObject;
            }
        }

        return null;
    }

    private void RefreshLevelButtons()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            GameObject btnObj = levelButtons[i];
            if (btnObj == null) continue;

            SetupLevelButton(btnObj, i);
        }
    }

    private void OnLevelButtonClicked(int levelIndex)
    {
        Debug.Log($"Level button clicked! Loading level: {levelIndex + 1}");

        Utils.SetLevelToPlay(levelIndex);

        MenuController.instance.LoadScenePlay();

        HideLevelPanel();
    }

    public void OnBattleButtonClicked()
    {
        MenuController.instance.LoadScenePlay();
    }

    private void ForceRebuildLayout()
    {
        // Force the layout to rebuild
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelButtonContainer.GetComponent<RectTransform>());

        // Also rebuild the scroll view
        if (levelPanel != null)
        {
            ScrollRect scrollRect = levelPanel.GetComponentInChildren<ScrollRect>();
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 1f; // Scroll to top
            }
        }
    }
}