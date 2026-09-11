using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeManagement.Core;

public sealed class RecipeManager : IRecipeManager
{
    // ===== 五种数据结构（私有字段）=====

    private readonly Dictionary<int, Recipe> _recipes;
    private readonly List<string> _shoppingList;
    private readonly LinkedList<int> _cookingPlan;
    private readonly Stack<int> _removedRecipes;
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

    // 添加食谱到烹饪计划末尾
    public bool AddRecipeToCookingPlan(int recipeId)
    {
        // 食谱必须存在
        if (!_recipes.ContainsKey(recipeId))
            return false;

        // 不能重复添加
        if (_cookingPlan.Contains(recipeId))
            return false;

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    // 从烹饪计划删除，成功则 push 到 Stack
    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        if (!_cookingPlan.Contains(recipeId))
            return false;

        _cookingPlan.Remove(recipeId);
        _removedRecipes.Push(recipeId);
        return true;
    }

    // 恢复最近删除的食谱到烹饪计划末尾
    public bool RestoreLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
            return false;

        int recipeId = _removedRecipes.Peek();

        // 食谱必须还存在，且不在烹饪计划中
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
            return false;

        _removedRecipes.Pop();
        _cookingPlan.AddLast(recipeId);
        return true;
    }

    // 查看最近删除的食谱 ID，不移除
    public int? PeekLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
            return null;

        return _removedRecipes.Peek();
    }

    // 返回烹饪计划列表
    public IReadOnlyList<int> GetCookingPlan()
    {
        return new List<int>(_cookingPlan);
    }

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
