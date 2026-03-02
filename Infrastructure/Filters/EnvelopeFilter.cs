using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NoWasteOfMoney.Models.Dtos;
using NoWasteOfMoney.Models.Responses;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Infrastructure.Filters
{
    public class EnvelopeFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult)
            {
                // Only wrap successful responses (2xx). 
                // ProblemDetails (4xx, 5xx) will generally not be wrapped, or they might not have a StatusCode set here but are usually handled by other formatting.
                if (objectResult.StatusCode == null || (objectResult.StatusCode >= 200 && objectResult.StatusCode < 300))
                {
                    // Check if it's a PagedResult
                    if (objectResult.Value != null && objectResult.Value.GetType().IsGenericType &&
                        objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(PagedResult<>))
                    {
                        var pagedResult = objectResult.Value;
                        var itemsProperty = pagedResult.GetType().GetProperty("Items");
                        var totalCountProperty = pagedResult.GetType().GetProperty("TotalCount");
                        var pageNumberProperty = pagedResult.GetType().GetProperty("PageNumber");
                        var pageSizeProperty = pagedResult.GetType().GetProperty("PageSize");

                        if (itemsProperty != null && totalCountProperty != null && pageNumberProperty != null && pageSizeProperty != null)
                        {
                            var items = itemsProperty.GetValue(pagedResult);
                            var totalCount = (int)totalCountProperty.GetValue(pagedResult)!;
                            var pageNumber = (int)pageNumberProperty.GetValue(pagedResult)!;
                            var pageSize = (int)pageSizeProperty.GetValue(pagedResult)!;

                            var envelope = new EnvelopeResponse<object>
                            {
                                Data = items,
                                Meta = new EnvelopeMeta
                                {
                                    Pagination = new PaginationMeta
                                    {
                                        Limit = pageSize,
                                        Offset = (pageNumber - 1) * pageSize,
                                        Total = totalCount
                                    }
                                }
                            };
                            objectResult.Value = envelope;
                        }
                    }
                    else
                    {
                        // Wrap regular object
                        var envelope = new EnvelopeResponse<object>
                        {
                            Data = objectResult.Value,
                            Meta = null // No pagination for single items or standard lists
                        };
                        objectResult.Value = envelope;
                    }
                }
            }

            await next();
        }
    }
}
