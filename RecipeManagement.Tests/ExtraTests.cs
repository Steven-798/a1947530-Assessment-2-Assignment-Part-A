using RecipeManagement.Core;
using Xunit;

namespace RecipeManagement.Tests;

public class ExtraTests
{
    private static RecipeManager MakeManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe { Id = 1, Title = "Pasta",
                Ingredients = new() { "noodles", "sauce" },
                Instructions = new() { "boil", "mix" } },
            new Recipe { Id = 2, Title = "Salad",
                Ingredients = new() { "lettuce" },
                Instructions = new() { "chop", "toss" } }
        });
    }

    // AddRecipe 边界测试
    [Fact]
    public void AddRecipe_InvalidId_ReturnsFalse()
    {
        var m = MakeManager();
        Assert.False(m.AddRecipe(new Recipe { Id = 0, Title = "Bad" }));
        Assert.False(m.AddRecipe(new Recipe { Id = -5, Title = "Bad" }));
    }

    [Fact]
    public void AddRecipe_BlankTitle_ReturnsFalse()
    {
        var m = MakeManager();
        Assert.False(m.AddRecipe(new Recipe { Id = 10, Title = "" }));
        Assert.False(m.AddRecipe(new Recipe { Id = 11, Title = "   " }));
    }

    [Fact]
    public void AddRecipe_Null_Throws()
    {
        var m = MakeManager();
        Assert.Throws<ArgumentNullException>(() => m.AddRecipe(null!));
    }

    // 构造函数边界测试
    [Fact]
    public void Constructor_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    public void Constructor_DuplicateId_Throws()
    {
        Assert.Throws<ArgumentException>(() => new RecipeManager(new[]
        {
            new Recipe { Id = 1, Title = "A" },
            new Recipe { Id = 1, Title = "B" }
        }));
    }

    // Queue 边界测试
    [Fact]
    public void CompleteAllInstructions_ThenNull()
    {
        var m = MakeManager();
        m.StartCooking(1);

        Assert.Equal("boil", m.CompleteNextInstruction());
        Assert.Equal("mix", m.CompleteNextInstruction());
        Assert.Null(m.CompleteNextInstruction());
    }

    [Fact]
    public void StartCooking_SwitchesRecipe_ClearsQueue()
    {
        var m = MakeManager();
        m.StartCooking(1);
        Assert.Equal(2, m.PendingInstructionCount);

        m.StartCooking(2);
        Assert.Equal(2, m.PendingInstructionCount);
        Assert.Equal("chop", m.PeekNextInstruction());
    }

    // Stack 边界测试
    [Fact]
    public void RemoveFromPlan_NotPlanned_DoesNotTouchStack()
    {
        var m = MakeManager();
        m.AddRecipeToCookingPlan(1);
        m.RemoveRecipeFromCookingPlan(999);

        Assert.Equal(0, m.RemovedRecipeCount);
        Assert.Null(m.PeekLastRemovedRecipe());
    }

    // 购物清单顺序测试
    [Fact]
    public void ShoppingList_MultipleRecipes_InOrder()
    {
        var m = MakeManager();
        m.AddIngredientsToShoppingList(1);
        m.AddIngredientsToShoppingList(2);

        var list = m.GetShoppingList();
        Assert.Equal(new[] { "noodles", "sauce", "lettuce" }, list);
    }

    // RemoveRecipe 边界测试
    [Fact]
    public void RemoveRecipe_NotExists_ReturnsFalse()
    {
        var m = MakeManager();
        Assert.False(m.RemoveRecipe(999));
    }
}
