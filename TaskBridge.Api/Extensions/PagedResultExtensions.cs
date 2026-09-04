using AutoMapper;

using TaskBridge.Application.Common;
using TaskBridge.Contracts.Common;

namespace TaskBridge.Api.Extensions;

public static class PagedResultExtensions
{
    public static PagedResponse<TDestination> ToPagedResponse<TSource, TDestination>(
        this PagedResult<TSource> source,
        IMapper mapper)
    {
        var items = mapper.Map<IReadOnlyList<TDestination>>(source.Items);

        return new PagedResponse<TDestination>(
            items,
            source.Page,
            source.PageSize,
            source.TotalCount);
    }
}
