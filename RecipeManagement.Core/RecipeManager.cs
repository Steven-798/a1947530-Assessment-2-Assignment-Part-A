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

    // 作业要求的字段
    private int jsoncount;

    // ===== 构造函数 =====
    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        // 验证参数不为 null
        if (recipes == null)
            throw new ArgumentNullException(nameof(recipes));

        // 初始化五种数据结构
        _recipes = new Dictionary<int, Recipe>();
        _shoppingList = new List<string>();
        _cookingPlan = new LinkedList<int>();
        _removedRecipes = new Stack<int>();
        _instructionQueue = new Queue<string>();

        // 遍历传入的 recipes，验证后加入字典
        foreach (Recipe recipe in recipes)
        {
            // 验证 ID 为正数
            if (recipe.Id <= 0)
                throw new ArgumentException("Recipe ID must be positive.");
            
            // 验证 Title 不为空
            if (string.IsNullOrWhiteSpace(recipe.Title))
                throw new ArgumentException("Recipe title cannot be blank.");
            
            // 验证 ID 不重复
            if (_recipes.ContainsKey(recipe.Id))
                throw new ArgumentException($"Duplicate recipe ID: {recipe.Id}");

            _recipes.Add(recipe.Id, recipe);
        }

        jsoncount = _recipes.Count;
    }

    // ===== 属性（返回各集合的元素数量）=====
    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _instructionQueue.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;

    // ===== 食谱目录方法 =====

    // 添加食谱到字典
    public bool AddRecipe(Recipe recipe)
    {
        if (recipe == null)
            throw new ArgumentNullException(nameof(recipe));

        // 验证 ID 为正数
        if (recipe.Id <= 0)
            return false;
        
        // 验证 Title 不为空
        if (string.IsNullOrWhiteSpace(recipe.Title))
            return false;
        
        // 验证 ID 不重复
        if (_recipes.ContainsKey(recipe.Id))
            return false;

        _recipes.Add(recipe.Id, recipe);
        return true;
    }

    // 按 ID 查找食谱，找不到返回 null
    public Recipe? FindRecipe(int recipeId)
    {
        _recipes.TryGetValue(recipeId, out Recipe? recipe);
        return recipe;
    }

    // 按 ID 删除食谱
    public bool RemoveRecipe(int recipeId)
    {
        // 如果食谱不存在，返回 false
        if (!_recipes.ContainsKey(recipeId))
            return false;

        // 如果食谱正在烹饪计划中，不能删除，返回 false
        if (_cookingPlan.Contains(recipeId))
            return false;

        _recipes.Remove(recipeId);
        return true;
    }

    // ===== 以下方法先留空，后面逐步实现 =====

    public int AddIngredientsToShoppingList(int recipeId) =>
        throw new NotImplementedException();

    public IReadOnlyList<string> GetShoppingList() =>
        throw new NotImplementedException();

    public void ClearShoppingList() =>
        throw new NotImplementedException();

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

    public bool StartCooking(int recipeId) =>
        throw new NotImplementedException();

    public string? PeekNextInstruction() =>
        throw new NotImplementedException();

    public string? CompleteNextInstruction() =>
        throw new NotImplementedException();

    // Part B 方法（暂时不用实现）
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
