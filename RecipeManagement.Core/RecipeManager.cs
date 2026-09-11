using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeManagement.Core;

public sealed class RecipeManager : IRecipeManager
{
    // ===== 五种数据结构（私有字段）=====

    // Dictionary：食谱目录，key 是 recipe ID，value 是 Recipe 对象
    private readonly Dictionary<int, Recipe> _recipes;

    // List：购物清单，存储食材字符串
    private readonly List<string> _shoppingList;

    // LinkedList：烹饪计划，存储 recipe ID，按顺序排列
    private readonly LinkedList<int> _cookingPlan;

    // Stack：最近删除的烹饪计划食谱 ID（后进先出）
    private readonly Stack<int> _removedRecipes;

    // Queue：当前烹饪的指令队列（先进先出）
    private readonly Queue<string> _instructionQueue;

    private int jsoncount;

    // ===== 构造函数 =====
    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        if (recipes == null)
            throw new ArgumentNullException(nameof(recipes));

        _recipes = new Dictionary<int, Recipe>();
        _shoppingList = new List<string>();
        _cookingPlan = new LinkedList<int>();
        _removedRecipes = new Stack<int>();
        _instructionQueue = new Queue<string>();

        foreach (Recipe recipe in recipes)
        {
            if (recipe.Id <= 0)
                throw new ArgumentException("Recipe ID must be positive.");

            if (string.IsNullOrWhiteSpace(recipe.Title))
                throw new ArgumentException("Recipe title cannot be blank.");

            if (_recipes.ContainsKey(recipe.Id))
                throw new ArgumentException($"Duplicate recipe ID: {recipe.Id}");

            _recipes.Add(recipe.Id, recipe);
        }

        jsoncount = _recipes.Count;
    }

    // ===== 属性 =====
    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _instructionQueue.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;

    // ===== 食谱目录（Dictionary）=====

    public bool AddRecipe(Recipe recipe)
    {
        if (recipe == null)
            throw new ArgumentNullException(nameof(recipe));

        if (recipe.Id <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(recipe.Title))
            return false;

        if (_recipes.ContainsKey(recipe.Id))
            return false;

        _recipes.Add(recipe.Id, recipe);
        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        _recipes.TryGetValue(recipeId, out Recipe? recipe);
        return recipe;
    }

    public bool RemoveRecipe(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId))
            return false;

        if (_cookingPlan.Contains(recipeId))
            return false;

        _recipes.Remove(recipeId);
        return true;
    }

    // ===== 购物清单（List<string>）=====

    public int AddIngredientsToShoppingList(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if (recipe == null)
            return 0;

        foreach (string ingredient in recipe.Ingredients)
        {
            _shoppingList.Add(ingredient);
        }

        return recipe.Ingredients.Count;
    }

    public IReadOnlyList<string> GetShoppingList()
    {
        return _shoppingList.AsReadOnly();
    }

    public void ClearShoppingList()
    {
        _shoppingList.Clear();
    }

    // ===== 烹饪计划（LinkedList<int>）+ 删除历史（Stack<int>）=====

    public bool AddRecipeToCookingPlan(int recipeId) =>
        throw new NotImplementedException();

    public bool RemoveRecipeFromCookingPlan(int recipeId) =>
        throw new NotImplementedException();

    public bool RestoreLastRemovedRecipe() =>
        throw new NotImplementedException();

    public int? PeekLastRemovedRecipe() =>
        throw new NotImplementedException();

    public IReadOnlyList<int> GetCookingPlan() =>
        throw new NotImplementedException();

    // ===== 烹饪指令（Queue<string>）=====

    public bool StartCooking(int recipeId) =>
        throw new NotImplementedException();

    public string? PeekNextInstruction() =>
        throw new NotImplementedException();

    public string? CompleteNextInstruction() =>
        throw new NotImplementedException();

    // ===== Part B（暂时不用实现）=====

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException();

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException();

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException();

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException();

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException();

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException();

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException();
}

