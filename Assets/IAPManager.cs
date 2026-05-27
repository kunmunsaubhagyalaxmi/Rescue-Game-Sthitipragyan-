// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Unity.Services.Core;
// using Unity.Services.Core.Environments;
// using UnityEngine;
// using UnityEngine.Purchasing;
// using UnityEngine.UI;

// public class IAPManager : MonoBehaviour
// {
//     public static IAPManager Instance { get; private set; }
//     public string banana2500 = "banana_2500";
//     public string banana7500 = "banana_7500";
//     public static bool IsInitialized { get; private set; } = false;
//     private static StoreController storeController;
//     [SerializeField] private ShopManager shopManager;

//     private async void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//         DontDestroyOnLoad(gameObject);

//         await InitIAP();
//     }

//     private async Task InitIAP()
//     {
//         try
//         {
//             var option = new InitializationOptions().SetEnvironmentName("production");
//             await UnityServices.InitializeAsync(option);

//             storeController = UnityIAPServices.StoreController();

//             storeController.OnStoreDisconnected += OnStoreDisconnected;
//             storeController.OnProductsFetched += OnProductsFetched;
//             storeController.OnProductsFetchFailed += OnProductsFetchFailed;
//             storeController.OnPurchasesFetched += OnPurchasesFetched;
//             storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
//             storeController.OnPurchasePending += OnPurchasePending;
//             storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
//             storeController.OnPurchaseFailed += OnPurchaseFailed;
//             storeController.OnPurchaseDeferred += OnPurchaseDeferred;

//             RegisterEntitlementCallbacks();

//             await storeController.Connect();

//             var initialProductToFetch = BuildProductDefinitions();
//             storeController.FetchProducts(initialProductToFetch);
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"IAP Initialization failed: {e.Message}");
//         }
//     }

//     private void RegisterEntitlementCallbacks()
//     {
//         storeController.OnCheckEntitlement += (result) =>
//         {
//             Product product = result.Product;
//             var status = result.Status;

//             Debug.Log($"Entitlement check for product {product.definition.id} returned status: {status}");

//             // Only for non-consumables
//         };
//     }

//     private List<ProductDefinition> BuildProductDefinitions()
//     {
//         var initialProductToFetch = new List<ProductDefinition>();

//         initialProductToFetch.Add(new ProductDefinition(banana2500, ProductType.Consumable));
//         initialProductToFetch.Add(new ProductDefinition(banana7500, ProductType.Consumable));
//         return initialProductToFetch;
//     }

//     private void OnProductsFetched(List<Product> products)
//     {
//         storeController.FetchPurchases();

//         foreach (var product in products)
//         {
//             string price = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
//             // Pass price in ShopManager to update UI

//             ShopManager.Instance.UpdateButtonPrice(product.definition.id, price);
//         }
//     }

//     private void OnProductsFetchFailed(ProductFetchFailed reason)
//     {
//         Debug.Log($"Product fetch failed: {reason}");
//     }

//     private void OnPurchasesFetched(Orders orders)
//     {
//         IsInitialized = true;
//         foreach (var product in storeController.GetProducts())
//         {
//             storeController.CheckEntitlement(product);
//         }
//     }

//     private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription reason)
//     {
//         Debug.Log($"Purchase fetch failed: {reason}");
//     }

//     private void OnStoreDisconnected(StoreConnectionFailureDescription description)
//     {
//         Debug.Log($"Initialization/Connection Failed: {description.message}");
//     }

//     public void BuyProduct(IAPProductKey productKey)
//     {
//         if (!IsInitialized)
//         {
//             Debug.LogWarning("IAP Models is not initialized.");
//             return;
//         }

//         if (productKey == IAPProductKey.Banana2500)
//         {
//             storeController.PurchaseProduct(banana2500);
//         }
//         else if (productKey == IAPProductKey.Banana7500)
//         {
//             storeController.PurchaseProduct(banana7500);
//         }
//     }

//     private void OnPurchasePending(PendingOrder order)
//     {
//         Debug.Log($"Pending order: {order}");
//         storeController.ConfirmPurchase(order);
//     }

//     private void OnPurchaseDeferred(DeferredOrder deferredOrder)
//     {
//         Debug.Log($"Purchase Deferred for product: {deferredOrder?.Info}");
//         //Show Pending Purchase UI
//     }

//     private void OnPurchaseConfirmed(Order order)
//     {
//         Debug.Log($"Purchase Confirmed: {order}");
//         //Reward the player
//         if (order?.Info?.PurchasedProductInfo != null && order.Info.PurchasedProductInfo.Count > 0)
//         {
//             int quantity = GetPurchaseQuantity(order);
//             string productId = order.Info.PurchasedProductInfo[0].productId;
//             if (productId == banana2500)
//             {
//                 shopManager.Purchasebananas(2500 * quantity);
//             }
//             else if (productId == banana7500)
//             {
//                 shopManager.Purchasebananas(7500 * quantity);
//             }
//         }
//     }

//     private int GetPurchaseQuantity(Order order)
//     {
//         int quantity = 1; // Default quantity

//         string receipt = order.Info.Receipt;
//         if (!string.IsNullOrEmpty(receipt))
//         {
//             IAPPayData payData = JsonUtility.FromJson<IAPPayData>(receipt);
//             if (payData.Store != "fake")
//             {
//                 IAPPayload payload = JsonUtility.FromJson<IAPPayload>(payData.Payload);
//                 IAPPayloadData payloadData = JsonUtility.FromJson<IAPPayloadData>(payload.json);
//                 quantity = payloadData.quantity;
//             }
//         }
//         return quantity;
//     }

//     private void OnPurchaseFailed(FailedOrder failedOrder)
//     {
//         if (failedOrder?.Info?.PurchasedProductInfo == null || failedOrder.Info.PurchasedProductInfo.Count == 0)
//         {
//             Debug.LogError("Purchase failed but no product info available.");
//             return;
//         }
//         var productId = failedOrder.Info.PurchasedProductInfo[0];
//         var reason = failedOrder.FailureReason;
//         var message = failedOrder.Details;

//         Debug.LogError($"Purchase failed for product {productId}. Reason: {reason}, Message: {message}");
//     }
    
//     // Only for iOS
//     public void RestorePurchases()
//     {
//         storeController.RestoreTransactions((success, error) =>
//         {
//             if (success)
//             {
//                 Debug.Log("All previous purchases restored successfully.");
//             }
//             else
//             {
//                 Debug.LogWarning($"Restore purchases failed: {error}");
//             }
//         });
//     }
// }





using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance { get; private set; }
    public string banana2500 = "banana_2500";
    public string banana7500 = "banana_7500";
    public static bool IsInitialized { get; private set; } = false;
    private static StoreController storeController;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        await InitIAP();
    }

    private void OnDestroy()
    {
        if (storeController != null)
        {
            storeController.OnStoreDisconnected -= OnStoreDisconnected;
            storeController.OnProductsFetched -= OnProductsFetched;
            storeController.OnProductsFetchFailed -= OnProductsFetchFailed;
            storeController.OnPurchasesFetched -= OnPurchasesFetched;
            storeController.OnPurchasesFetchFailed -= OnPurchasesFetchFailed;
            storeController.OnPurchasePending -= OnPurchasePending;
            storeController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
            storeController.OnPurchaseFailed -= OnPurchaseFailed;
            storeController.OnPurchaseDeferred -= OnPurchaseDeferred;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private async Task InitIAP()
    {
        try
        {
            var option = new InitializationOptions().SetEnvironmentName("production");
            await UnityServices.InitializeAsync(option);

            storeController = UnityIAPServices.StoreController();

            storeController.OnStoreDisconnected += OnStoreDisconnected;
            storeController.OnProductsFetched += OnProductsFetched;
            storeController.OnProductsFetchFailed += OnProductsFetchFailed;
            storeController.OnPurchasesFetched += OnPurchasesFetched;
            storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
            storeController.OnPurchasePending += OnPurchasePending;
            storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            storeController.OnPurchaseFailed += OnPurchaseFailed;
            storeController.OnPurchaseDeferred += OnPurchaseDeferred;

            RegisterEntitlementCallbacks();

            await storeController.Connect();

            var initialProductToFetch = BuildProductDefinitions();
            storeController.FetchProducts(initialProductToFetch);
        }
        catch (Exception e)
        {
            Debug.LogError($"IAP Initialization failed: {e.Message}");
        }
    }

    private void RegisterEntitlementCallbacks()
    {
        storeController.OnCheckEntitlement += (result) =>
        {
            Product product = result.Product;
            var status = result.Status;

            Debug.Log($"Entitlement check for product {product.definition.id} returned status: {status}");
        };
    }

    private List<ProductDefinition> BuildProductDefinitions()
    {
        var initialProductToFetch = new List<ProductDefinition>();

        initialProductToFetch.Add(new ProductDefinition(banana2500, ProductType.Consumable));
        initialProductToFetch.Add(new ProductDefinition(banana7500, ProductType.Consumable));
        return initialProductToFetch;
    }

    private void OnProductsFetched(List<Product> products)
    {
        storeController.FetchPurchases();

        foreach (var product in products)
        {
            string price = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
            
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.UpdateButtonPrice(product.definition.id, price);
            }
        }
    }

    private void OnProductsFetchFailed(ProductFetchFailed reason)
    {
        Debug.Log($"Product fetch failed: {reason}");
    }

    private void OnPurchasesFetched(Orders orders)
    {
        IsInitialized = true;
        foreach (var product in storeController.GetProducts())
        {
            storeController.CheckEntitlement(product);
        }
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription reason)
    {
        Debug.Log($"Purchase fetch failed: {reason}");
    }

    private void OnStoreDisconnected(StoreConnectionFailureDescription description)
    {
        Debug.Log($"Initialization/Connection Failed: {description.message}");
    }

    public void BuyProduct(IAPProductKey productKey)
    {
        if (!IsInitialized)
        {
            Debug.LogWarning("IAP Models is not initialized.");
            return;
        }

        if (productKey == IAPProductKey.Banana2500)
        {
            storeController.PurchaseProduct(banana2500);
        }
        else if (productKey == IAPProductKey.Banana7500)
        {
            storeController.PurchaseProduct(banana7500);
        }
    }

    private void OnPurchasePending(PendingOrder order)
    {
        Debug.Log($"Pending order: {order}");
        storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseDeferred(DeferredOrder deferredOrder)
    {
        Debug.Log($"Purchase Deferred for product: {deferredOrder?.Info}");
    }

    private void OnPurchaseConfirmed(Order order)
    {
        Debug.Log($"Purchase Confirmed: {order}");
        //Reward the player
        if (order?.Info?.PurchasedProductInfo != null && order.Info.PurchasedProductInfo.Count > 0)
        {
            int quantity = GetPurchaseQuantity(order);
            string productId = order.Info.PurchasedProductInfo[0].productId;
            
            if (ShopManager.Instance != null)
            {
                if (productId == banana2500)
                {
                    ShopManager.Instance.Purchasebananas(2500 * quantity);
                }
                else if (productId == banana7500)
                {
                    ShopManager.Instance.Purchasebananas(7500 * quantity);
                }
            }
            else
            {
                Debug.LogWarning("ShopManager not found, but purchase was successful. Saving coins directly.");
                if (productId == banana2500)
                {
                    Utils.currentCoin += 2500 * quantity;
                }
                else if (productId == banana7500)
                {
                    Utils.currentCoin += 7500 * quantity;
                }
                Utils.SaveCoin();
            }
        }
    }

    private int GetPurchaseQuantity(Order order)
    {
        int quantity = 1;

        string receipt = order.Info.Receipt;
        if (!string.IsNullOrEmpty(receipt))
        {
            IAPPayData payData = JsonUtility.FromJson<IAPPayData>(receipt);
            if (payData.Store != "fake")
            {
                IAPPayload payload = JsonUtility.FromJson<IAPPayload>(payData.Payload);
                IAPPayloadData payloadData = JsonUtility.FromJson<IAPPayloadData>(payload.json);
                quantity = payloadData.quantity;
            }
        }
        return quantity;
    }

    private void OnPurchaseFailed(FailedOrder failedOrder)
    {
        if (failedOrder?.Info?.PurchasedProductInfo == null || failedOrder.Info.PurchasedProductInfo.Count == 0)
        {
            Debug.LogError("Purchase failed but no product info available.");
            return;
        }
        var productId = failedOrder.Info.PurchasedProductInfo[0];
        var reason = failedOrder.FailureReason;
        var message = failedOrder.Details;

        Debug.LogError($"Purchase failed for product {productId}. Reason: {reason}, Message: {message}");
    }
    
    // Only for iOS
    public void RestorePurchases()
    {
        storeController.RestoreTransactions((success, error) =>
        {
            if (success)
            {
                Debug.Log("All previous purchases restored successfully.");
            }
            else
            {
                Debug.LogWarning($"Restore purchases failed: {error}");
            }
        });
    }
}