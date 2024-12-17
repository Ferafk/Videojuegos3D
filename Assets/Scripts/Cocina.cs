using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cocina : MonoBehaviour
{

    public enum RecipeType
    {
        Cookie,
        Cake
    }

    [Header("Inventory Settings")]
    public Dictionary<ItemType, int> storedItems = new Dictionary<ItemType, int>();

    [Header("Recipe Configurations")]
    public RecipeConfiguration[] recipes = new RecipeConfiguration[]
    {
        new RecipeConfiguration
        {
            type = RecipeType.Cookie,
            requiredIngredients = new ItemType[]
            {
                ItemType.Flour,
                ItemType.Sugar,
                ItemType.Chocolate
            },
            recipePrefav = null
        },
        new RecipeConfiguration
        {
            type = RecipeType.Cake,
            requiredIngredients = new ItemType[]
            {
                ItemType.Flour,
                ItemType.Milk,
                ItemType.Sugar,
                ItemType.Chocolate,
                ItemType.Eggs
            },
            recipePrefav = null
        }
    };

    [System.Serializable]
    public class RecipeConfiguration
    {
        public RecipeType type;
        public ItemType[] requiredIngredients;
        public GameObject recipePrefav;
    }

    [Header("Ingredient Indicators")]
    public List<IngredientIndicator> ingredientIndicators = new List<IngredientIndicator>();

    [System.Serializable]
    public class IngredientIndicator
    {
        public ItemType ingredientType;
        public GameObject indicatorObject;
    }

    public Transform pastelSpawnPoint;
    public Transform cookieSpawnPoint;
    public GameObject pastelPrefav;
    public GameObject cookiePrefav;

    private void OnCollisionEnter(Collision collision)
    {
        Bala bullet = collision.gameObject.GetComponent<Bala>();

        if (bullet != null)
        {
            // Recoger el item
            AddItemToInventory(bullet.itemType);

            // Destruir el proyectil
            collision.gameObject.SetActive(false);

            // Intentar crear recetas
            TryCreateRecipes();
        }
    }

    private void AddItemToInventory(ItemType ingredientType)
    {
        // Si el item no existe en el inventario, inicializarlo
        if (!storedItems.ContainsKey(ingredientType))
        {
            storedItems[ingredientType] = 0;
        }

        // Incrementar cantidad
        storedItems[ingredientType]++;

        UpdateIngredientIndicators();

        // Log de recolección de item
        Debug.Log($"Collected {ingredientType}. Current Inventory: {storedItems[ingredientType]}");
    }

    private void TryCreateRecipes()
    {
        // Primero intentar crear pastel (receta más compleja)
        foreach (var recipe in recipes)
        {
            if (recipe.type == RecipeType.Cake && CanCreateRecipe(recipe))
            {
                CreateRecipe(recipe);
                return; // Salir después de crear el pastel
            }
        }

        // Si no se puede crear pastel, intentar crear galletas
        foreach (var recipe in recipes)
        {
            if (recipe.type == RecipeType.Cookie && CanCreateRecipe(recipe))
            {
                CreateRecipe(recipe);
                return; // Salir después de crear galletas
            }
        }
    }

    private bool CanCreateRecipe(RecipeConfiguration recipe)
    {
        // Verificación específica para cada tipo de receta
        switch (recipe.type)
        {
            case RecipeType.Cookie:
                // Verificar exactamente los 3 ingredientes para galletas
                return storedItems.ContainsKey(ItemType.Flour) && storedItems[ItemType.Flour] > 0 &&
                       storedItems.ContainsKey(ItemType.Sugar) && storedItems[ItemType.Sugar] > 0 &&
                       storedItems.ContainsKey(ItemType.Chocolate) && storedItems[ItemType.Chocolate] > 0;

            case RecipeType.Cake:
                // Verificar exactamente los 5 ingredientes para pastel
                return storedItems.ContainsKey(ItemType.Flour) && storedItems[ItemType.Flour] > 0 &&
                       storedItems.ContainsKey(ItemType.Milk) && storedItems[ItemType.Milk] > 0 &&
                       storedItems.ContainsKey(ItemType.Sugar) && storedItems[ItemType.Sugar] > 0 &&
                       storedItems.ContainsKey(ItemType.Chocolate) && storedItems[ItemType.Chocolate] > 0 &&
                       storedItems.ContainsKey(ItemType.Eggs) && storedItems[ItemType.Eggs] > 0;

            default:
                return false;
        }
    }

    private void CreateRecipe(RecipeConfiguration recipe)
    {
        // Consumir ingredientes
        foreach (var ingredient in recipe.requiredIngredients)
        {
            storedItems[ingredient]--;
        }

        // Actualizar indicadores
        UpdateIngredientIndicators();

        // Log de creación de receta
        Debug.Log($"Created {recipe.type} recipe!");

        // Aquí podrías añadir lógica para instanciar el prefab de la receta

        SpawnRecipeItem(recipe.type);
    }

    private void SpawnRecipeItem(RecipeType receta)
    {
        GameObject postre = null;

        if (receta == RecipeType.Cookie)
        {
            postre = Instantiate(cookiePrefav, cookieSpawnPoint.position, cookieSpawnPoint.rotation);
            Debug.Log("Saaalen unas Galletaaas");
        }
        else if (receta == RecipeType.Cake)
        {
            postre = Instantiate(pastelPrefav, pastelSpawnPoint.position, pastelSpawnPoint.rotation);
            Debug.Log("Saaale un Pasteeeeeeel");
        }
    }

    private void UpdateIngredientIndicators()
    {
        foreach (var indicator in ingredientIndicators)
        {
            // Activar el indicador si el ingrediente está en el inventario
            if (storedItems.ContainsKey(indicator.ingredientType) && storedItems[indicator.ingredientType] > 0)
            {
                indicator.indicatorObject.SetActive(true);
            }
            else
            {
                indicator.indicatorObject.SetActive(false);
            }
        }
    }

    // Método opcional para depuración de inventario
    public void PrintInventory()
    {
        Debug.Log("Current Kitchen Inventory:");
        foreach (var item in storedItems)
        {
            Debug.Log($"{item.Key}: {item.Value}");
        }
    }
}
