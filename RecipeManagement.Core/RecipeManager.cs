using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeManagement.Core;

public sealed class RecipeManager : IRecipeManager
{
    private readonly Dictionary<int, Recipe> _recipes;
    private readonly List<string> _shoppingList;
    private readonly LinkedList<int> _cookingPlan;
    private readonly Stack<int> _removedRecipes;
    private readonly Queue<string> _instructionQueue;

    private int jsoncount;

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        if (recipes == null)
            throw new ArgumentNullException(nameof(recipes));

        _recipes = new Dictionary<int, Recipe>();
        _shoppingList = new List<string>();
        _cookingPlan = new LinkedList<int>();
        _removedRecipes = new Stack<int>();
        _instructionQueue = new Queue<string>();

        foreach (var r in recipes)
        {
            if (r.Id <= 0)
                throw new ArgumentException("Recipe ID must be positive.");
            if (string.IsNullOrWhiteSpace(r.Title))
                throw new ArgumentException("Recipe title cannot be blank.");
            if (_recipes.ContainsKey(r.Id))
                throw new ArgumentException($"Duplicate recipe ID: {r.Id}");

            _recipes.Add(r.Id, r);
        }

        jsoncount = _recipes.Count;
    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _instructionQueue.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;

    // 食谱目录
    public bool AddRecipe(Recipe recipe)
    {
        if (recipe == null)
            throw new ArgumentNullException(nameof(recipe));

        if (recipe.Id <= 0 || string.IsNullOrWhiteSpace(recipe.Title))
            return false;
        if (_recipes.ContainsKey(recipe.Id))
            return false;

        _recipes.Add(recipe.Id, recipe);
        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        _recipes.TryGetValue(recipeId, out var recipe);
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

    // 购物清单
    public int AddIngredientsToShoppingList(int recipeId)
    {
        var recipe = FindRecipe(recipeId);
        if (recipe == null)
            return 0;

        foreach (var ing in recipe.Ingredients)
            _shoppingList.Add(ing);

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

    // 烹饪计划 + 删除历史
    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId))
            return false;
        if (_cookingPlan.Contains(recipeId))
            return false;

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        if (!_cookingPlan.Contains(recipeId))
            return false;

        _cookingPlan.Remove(recipeId);
        _removedRecipes.Push(recipeId);
        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
            return false;

        var id = _removedRecipes.Peek();

        if (!_recipes.ContainsKey(id) || _cookingPlan.Contains(id))
            return false;

        _removedRecipes.Pop();
        _cookingPlan.AddLast(id);
        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
            return null;
        return _removedRecipes.Peek();
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        return new List<int>(_cookingPlan);
    }

    // 烹饪指令队列
    public bool StartCooking(int recipeId)
    {
        var recipe = FindRecipe(recipeId);
        if (recipe == null || recipe.Instructions.Count == 0)
            return false;

        _instructionQueue.Clear();

        foreach (var step in recipe.Instructions)
            _instructionQueue.Enqueue(step);

        return true;
    }

    public string? PeekNextInstruction()
    {
        if (_instructionQueue.Count == 0)
            return null;
        return _instructionQueue.Peek();
    }

    public string? CompleteNextInstruction()
    {
        if (_instructionQueue.Count == 0)
            return null;
        return _instructionQueue.Dequeue();
    }

    public int indexer()
    {
        var last = 0;
        foreach (var r in _recipes)
        {
            if (r.Key > last)
                last = r.Key;
        }
        return last;
    }

    // Part B 暂时不用做
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
