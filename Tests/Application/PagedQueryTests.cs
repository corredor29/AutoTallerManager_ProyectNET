using Application.Requests.Customers;

namespace AutoTallerManager.Tests.Application;

public sealed class PagedQueryTests
{
    [Fact]
    public void NormalizedPageNumber_ReturnsOne_WhenPageNumberIsLessThanOne()
    {
        var request = new GetCustomersRequest
        {
            PageNumber = 0
        };

        Assert.Equal(1, request.NormalizedPageNumber);
    }

    [Fact]
    public void NormalizedPageSize_ReturnsDefault_WhenPageSizeIsLessThanOne()
    {
        var request = new GetCustomersRequest
        {
            PageSize = 0
        };

        Assert.Equal(10, request.NormalizedPageSize);
    }

    [Fact]
    public void NormalizedPageSize_ReturnsMax_WhenPageSizeExceedsLimit()
    {
        var request = new GetCustomersRequest
        {
            PageSize = 500
        };

        Assert.Equal(100, request.NormalizedPageSize);
    }
}
