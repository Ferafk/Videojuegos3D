using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public enum ItemType
{
    Flour,
    Milk,
    Eggs,
    Chocolate,
    Sugar,
    Butter,
    Pastel,
    Galleta
}

[System.Serializable]
public class ItemPickupData
{
    public ItemType type;
    public int maxCarryAmount;
    public GameObject projectilePrefab;
}

public class ItemManager : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRadius = 3f;
    public LayerMask itemLayer;

    [Header("Inventory Configuration")]
    public List<ItemPickupData> pickupableItems = new List<ItemPickupData>
    {
        new ItemPickupData { type = ItemType.Flour, maxCarryAmount = 5, projectilePrefab = null },
        new ItemPickupData { type = ItemType.Milk, maxCarryAmount = 5, projectilePrefab = null },
        new ItemPickupData { type = ItemType.Eggs, maxCarryAmount = 12, projectilePrefab = null },
        new ItemPickupData { type = ItemType.Chocolate, maxCarryAmount = 7, projectilePrefab = null },
        new ItemPickupData { type = ItemType.Sugar, maxCarryAmount = 5, projectilePrefab = null },
        new ItemPickupData { type = ItemType.Butter, maxCarryAmount = 7, projectilePrefab = null },
        new ItemPickupData { type = ItemType.Pastel, maxCarryAmount = 8, projectilePrefab = null },
        new ItemPickupData { type = ItemType.Galleta, maxCarryAmount = 12, projectilePrefab = null }
    };

    [Header("Current Inventory")]
    public ItemType? currentItemType = null;
    public int currentItemAmount = 0;
    private ItemPickupData currentItemData;
    public ObjectPool objectPool;
    public bool EsReceta;

    public GameObject currentProjectilPrefab
    {
        get
        {
            return currentItemData != null ? currentItemData.projectilePrefab : null;
        }
    }

    public string currentProjectileName
    {
        get
        {
            return currentItemData.type.ToString();
        }
    }

    void Update()
    {
        CheckNearbyItems();
    }

    private void CheckNearbyItems()
    {
        if (currentItemType.HasValue) return;

        Collider[] nearbyItems = Physics.OverlapSphere(transform.position, pickupRadius, itemLayer);

        if (nearbyItems.Length > 0)
        {
            // Encuentra el item más cercano
            Collider closestItem = nearbyItems[0];
            float closestDistance = Vector3.Distance(transform.position, closestItem.transform.position);

            foreach (Collider item in nearbyItems)
            {
                float distance = Vector3.Distance(transform.position, item.transform.position);
                if (distance < closestDistance)
                {
                    closestItem = item;
                    closestDistance = distance;
                }
            }

            // Intenta recoger el item
            Item itemBehavior = closestItem.GetComponent<Item>();
            if (itemBehavior != null)
            {
                PickupItem(itemBehavior.ingredientType, itemBehavior.isReceta, itemBehavior.gameObject);
                Debug.Log("Recogiendo item");
            }
        }
    }

    private void PickupItem(ItemType itemType, bool receta, GameObject objeto)
    {
        // Busca la configuración de este item
        ItemPickupData itemData = pickupableItems.Find(x => x.type == itemType);

        if (itemData != null)
        {
            currentItemType = itemType;
            currentItemAmount = itemData.maxCarryAmount;
            currentItemData = itemData;
            objectPool.UpdateProjectile();

            Debug.Log($"Picked up {currentItemType} - Amount: {currentItemAmount}");

            if (receta == false)
            {
                Debug.Log("Ingrediente encontrado");
            }
            else
            {
                Destroy(objeto);
                EsReceta = true;
                Debug.Log("Receta encontrada");
            }
        }
    }

    public void ClearItem()
    {
        currentItemType = null;
        currentItemAmount = 0;
        currentItemData = null;
        objectPool.UpdateProjectile();
        EsReceta = false;
        Debug.Log("Item Acabado");
    }

    public GameObject GetProjectileForCurrentItem()
    {
        if (currentItemType.HasValue)
        {
            ItemPickupData itemData = pickupableItems.Find(x => x.type == currentItemType.Value);
            return itemData?.projectilePrefab;
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

}
