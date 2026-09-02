namespace Smart.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authorization;

public sealed class AttributeAuthorizeDataCacheTest
{
    [Fact]
    public void ReturnsAuthorizeDataForAuthorizedType()
    {
        var result = AttributeAuthorizeDataCache.GetAuthorizeDataForType(typeof(AuthorizedType));

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public void ReturnsNullForAllowAnonymousType()
    {
        var result = AttributeAuthorizeDataCache.GetAuthorizeDataForType(typeof(AnonymousType));

        Assert.Null(result);
    }

    [Fact]
    public void ReturnsNullForTypeWithoutAuthorizeData()
    {
        var result = AttributeAuthorizeDataCache.GetAuthorizeDataForType(typeof(PlainType));

        Assert.Null(result);
    }

    [Fact]
    public void CachesResultReturnsSameInstance()
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
