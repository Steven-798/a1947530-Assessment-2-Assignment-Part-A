using RecipeManagement.Core;
using Xunit;

namespace RecipeManagement.Tests;

public class MyTests
{
    private static RecipeManager MakeManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe { Id = 1, Title = "Pasta",
                Ingredients = new() { "noodles", "sauce", "cheese" },
                Instructions = new() { "boil water", "cook pasta", "add sauce" } },
            new Recipe { Id = 2, Title = "Salad",
                Ingredients = new() { "lettuce", "tomato", "dressing" },
                Instructions = new() { "chop", "mix" } },
            new Recipe { Id = 3, Title = "Soup",
                Ingredients = new() { "broth", "veggies", "salt" },
                Instructions = new() { "heat", "add veggies", "simmer" } }
        });
    }

    // Dictionary 测试
    [Fact]
    public void AddDuplicateId_Fails()
    {
        var m = MakeManager();
        var result = m.AddRecipe(new Recipe { Id = 1, Title = "Dup" });
        Assert.False(result);
        Assert.Equal(3, m.RecipeCount);
    }

    [Fact]
    public void FindMissingId_ReturnsNull()
    {
        var m = MakeManager();
        Assert.Null(m.FindRecipe(999));
    }

    [Fact]
    public void RemoveRecipeInPlan_Fails()
    {
        var m = MakeManager();
        m.AddRecipeToCookingPlan(1);
        Assert.False(m.RemoveRecipe(1));
    }

    // List 购物清单测试
    [Fact]
    public void AddIngredientsNotFound_ReturnsZero()
    {
        var m = MakeManager();
        Assert.Equal(0, m.AddIngredientsToShoppingList(999));
    }

    [Fact]
    public void AddIngredientsTwoRecipes_Accumulates()
    {
        var m = MakeManager();
        m.AddIngredientsToShoppingList(1);
        m.AddIngredientsToShoppingList(2);
        Assert.Equal(6, m.ShoppingItemCount);
    }

    [Fact]
    public void ClearShoppingList_Empty()
    {
        var m = MakeManager();
        m.AddIngredientsToShoppingList(1);
        m.ClearShoppingList();
        Assert.Equal(0, m.ShoppingItemCount);
    }

    // LinkedList 烹饪计划测试
    [Fact]
    public void AddDuplicateToPlan_Fails()
    {
        var m = MakeManager();
        m.AddRecipeToCookingPlan(1);
        Assert.False(m.AddRecipeToCookingPlan(1));
        Assert.Equal(1, m.CookingPlanCount);
    }

    [Fact]
    public void AddMissingRecipeToPlan_Fails()
    {
        var m = MakeManager();
        Assert.False(m.AddRecipeToCookingPlan(999));
    }

    [Fact]
    public void GetPlan_InOrder()
    {
        var m = MakeManager();
        m.AddRecipeToCookingPlan(3);
        m.AddRecipeToCookingPlan(1);
        m.AddRecipeToCookingPlan(2);
        Assert.Equal(new[] { 3, 1, 2 }, m.GetCookingPlan());
    }

    // Stack 删除历史测试
    [Fact]
    public void PeekEmptyStack_Null()
    {
        var m = MakeManager();
        Assert.Null(m.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RemoveTwo_LastInFirstOut()
    {
        var m = MakeManager();
        m.AddRecipeToCookingPlan(1);
        m.AddRecipeToCookingPlan(2);
        m.RemoveRecipeFromCookingPlan(1);
        m.RemoveRecipeFromCookingPlan(2);
        Assert.Equal(2, m.PeekLastRemovedRecipe());
        Assert.Equal(2, m.RemovedRecipeCount);
    }

    [Fact]
    public void RestoreEmptyStack_Fails()
    {
        var m = MakeManager();
        Assert.False(m.RestoreLastRemovedRecipe());
    }

    // Queue 烹饪指令测试
    [Fact]
    public void PeekEmptyQueue_Null()
    {
        var m = MakeManager();
        Assert.Null(m.PeekNextInstruction());
    }

    [Fact]
    public void CompleteEmptyQueue_Null()
    {
        var m = MakeManager();
        Assert.Null(m.CompleteNextInstruction());
    }

    [Fact]
    public void StartCookingNoInstructions_Fails()
    {
        var m = new RecipeManager(new[] { new Recipe { Id = 1, Title = "Empty" } });
        Assert.False(m.StartCooking(1));
    }

    [Fact]
    public void StartCooking_LoadsInOrder()
    {
        var m = MakeManager();
        m.StartCooking(1);
        Assert.Equal(3, m.PendingInstructionCount);
        Assert.Equal("boil water", m.PeekNextInstruction());
    }

    // 组件交互测试
    [Fact]
    public void FullWorkflow_Works()
    {
        var m = MakeManager();

        var pasta = m.FindRecipe(1);
        Assert.NotNull(pasta);

        var count = m.AddIngredientsToShoppingList(1);
        Assert.Equal(3, count);

        Assert.True(m.AddRecipeToCookingPlan(1));
        Assert.True(m.StartCooking(1));

        var first = m.CompleteNextInstruction();
        Assert.Equal("boil water", first);
        Assert.Equal(2, m.PendingInstructionCount);
    }

    [Fact]
    public void RemoveThenRestore_Works()
    {
        var m = MakeManager();
        m.AddRecipeToCookingPlan(1);
        m.AddRecipeToCookingPlan(2);

        m.RemoveRecipeFromCookingPlan(1);
        Assert.Equal(1, m.CookingPlanCount);

        Assert.True(m.RestoreLastRemovedRecipe());
        Assert.Equal(2, m.CookingPlanCount);
        Assert.Equal(new[] { 2, 1 }, m.GetCookingPlan());
    }
}
