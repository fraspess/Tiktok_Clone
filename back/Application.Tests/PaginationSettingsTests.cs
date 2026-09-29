using Application.Features.User.Login;
using Application.Pagination;
using Xunit;

namespace Application.Tests;

public class PaginationSettingsTests
{
    [Fact]
    public void PageSize_WhenAboveMaximum_IsClampedToTwenty()
    {
        var settings = new PaginationSettings
        {
            PageSize = 100
        };

        Assert.Equal(50, settings.PageSize);
    }

    [Fact]
    public void PageSize_WhenBelowMinimum_IsClampedToOne()
    {
        var settings = new PaginationSettings
        {
            PageSize = -1
        };
        Assert.Equal(1, settings.PageSize);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    [InlineData(20, 20)]
    [InlineData(999, 50)]
    public void PageSize_IsKeptWithinAllowedRange(int input, int expected)
    {
        var settings = new PaginationSettings
        {
            PageSize = input
        };
        Assert.Equal(expected, settings.PageSize);
    }
}






