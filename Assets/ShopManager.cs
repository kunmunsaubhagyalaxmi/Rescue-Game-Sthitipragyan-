// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class ShopManager : MonoBehaviour
// {
//     public static ShopManager Instance { get; private set; }

//     [Header("Banana Text")]
//     [SerializeField] private TextMeshProUGUI bananaText;

//     [Header("Buy Buttons")]
//     [SerializeField] private Button banana2500Button;
//     [SerializeField] private Button banana7500Button;

//     [Header("Buy Button Texts")]
//     [SerializeField] private TextMeshProUGUI banana2500PriceText;
//     [SerializeField] private TextMeshProUGUI banana7500PriceText;

//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//         DontDestroyOnLoad(gameObject);
//     }

//     private void Start()
//     {
//         Utils.LoadCoin();
//         bananaText.text = Utils.currentCoin.ToString();
//     }

//     public void UpdateButtonPrice(string productId, string price)
//     {
//         if (productId == IAPManager.Instance.banana2500)
//         {
//             banana2500PriceText.text = price;
//         }
//         else if (productId == IAPManager.Instance.banana7500)
//         {
//             banana7500PriceText.text = price;
//         }
//     }
    
//     public void Banana2500Button()
//     {
//         IAPManager.Instance.BuyProduct(IAPProductKey.Banana2500);
//     }

//     public void Banana7500Button()
//     {
//         IAPManager.Instance.BuyProduct(IAPProductKey.Banana7500);
//     }

//     public void Purchasebananas(int bananaAmount)
//     {
//         Utils.currentCoin += bananaAmount;
//         Utils.SaveCoin();
//         bananaText.text = Utils.currentCoin.ToString();
//         Debug.Log($"Purchased {bananaAmount} bananas. Total: {Utils.currentCoin}");
//     }
// }



using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Banana Text")]
    [SerializeField] private TextMeshProUGUI bananaText;

    [Header("Buy Buttons")]
    [SerializeField] private Button banana2500Button;
    [SerializeField] private Button banana7500Button;

    [Header("Buy Button Texts")]
    [SerializeField] private TextMeshProUGUI banana2500PriceText;
    [SerializeField] private TextMeshProUGUI banana7500PriceText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Utils.LoadCoin();
        bananaText.text = Utils.currentCoin.ToString();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void UpdateButtonPrice(string productId, string price)
    {
        if (productId == IAPManager.Instance.banana2500)
        {
            banana2500PriceText.text = price;
        }
        else if (productId == IAPManager.Instance.banana7500)
        {
            banana7500PriceText.text = price;
        }
    }
    
    public void Banana2500Button()
    {
        IAPManager.Instance.BuyProduct(IAPProductKey.Banana2500);
    }

    public void Banana7500Button()
    {
        IAPManager.Instance.BuyProduct(IAPProductKey.Banana7500);
    }

    public void Purchasebananas(int bananaAmount)
    {
        Utils.currentCoin += bananaAmount;
        Utils.SaveCoin();
        bananaText.text = Utils.currentCoin.ToString();
        Debug.Log($"Purchased {bananaAmount} bananas. Total: {Utils.currentCoin}");
    }
}