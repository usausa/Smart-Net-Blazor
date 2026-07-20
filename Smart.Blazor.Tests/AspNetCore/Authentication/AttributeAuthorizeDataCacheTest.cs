namespace Smart.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authorization;

public sealed class AttributeAuthorizeDataCacheTest
{
    [Fact]
    public void ReturnsAuthorizeData_ForAuthorizedType()
    {
        var result = AttributeAuthorizeDataCache.GetAuthorizeDataForType(typeof(AuthorizedType));

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public void ReturnsNull_ForAllowAnonymousType()
    {
        var result = AttributeAuthorizeDataCache.GetAuthorizeDataForType(typeof(AnonymousType));

        Assert.Null(result);
    }

    [Fact]
    public void ReturnsNull_ForTypeWithoutAuthorizeData()
    {
        var result = AttributeAuthorizeDataCache.GetAuthorizeDataForType(typeof(PlainType));

        Assert.Null(result);
    }

    [Fact]
    public void CachesResult_ReturnsSameInstance()
    {
        var first = AttributeAuthorizeDataCache.GetAuthorizeDataForType(typeof(AuthorizedType));
        var second = AttributeAuthorizeDataCache.GetAuthorizeDataForType(typeof(AuthorizedType));

        Assert.Same(first, second);
    }

    [Authorize]
    public sealed class AuthorizedType;

    [AllowAnonymous]
    public sealed class AnonymousType;

    public sealed class PlainType;
}
