namespace Smart.AspNetCore.Authentication;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Authorization;

internal static class AttributeAuthorizeDataCache
{
    private static readonly ConcurrentDictionary<Type, IAuthorizeData[]?> Cache = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAuthorizeData[]? GetAuthorizeDataForType(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type) =>
        Cache.GetOrAdd(type, ComputeAuthorizeDataForType);

    private static IAuthorizeData[]? ComputeAuthorizeDataForType(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
    {
        var allAttributes = type.GetCustomAttributes(inherit: true);
        List<IAuthorizeData>? list = null;
        foreach (var attribute in allAttributes)
        {
            if (attribute is IAllowAnonymous)
            {
                return null;
            }

            if (attribute is IAuthorizeData authorizeData)
            {
                list ??= [];
                list.Add(authorizeData);
            }
        }

        return list?.ToArray();
    }
}
