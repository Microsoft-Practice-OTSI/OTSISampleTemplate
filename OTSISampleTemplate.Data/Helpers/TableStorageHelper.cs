using Azure.Data.Tables;
using System.Linq.Expressions;

namespace OTSISampleTemplate.Data.Helpers;

public static class TableStorageHelper
{
    public static async Task<List<TBase>> QueryEntitiesAsync<TDerived, TBase>(
       TableClient tableClient,
       string filter = "")
       where TDerived : class, TBase, ITableEntity, new()
       where TBase : class
    {
        var results = new List<TBase>();

        await foreach (var entity in tableClient.QueryAsync<TDerived>(filter))
        {
            results.Add(entity); 
        }

        return results;
    }

    public static async Task<List<TDerived>> QueryEntitiesAsync<TDerived>(
       TableClient tableClient,
       string filter = "")
       where TDerived : class, ITableEntity, new()
    {
        var results = new List<TDerived>();

        await foreach (var entity in tableClient.QueryAsync<TDerived>(filter))
        {
            results.Add(entity);
        }

        return results;
    }

    public static async Task<TValue?> GetMaxValueAsync<TEntity, TValue>(
    TableClient tableClient,
    Expression<Func<TEntity, bool>> filterExpression,
    Func<TEntity, TValue> selector)
    where TEntity : class, ITableEntity, new()
    {
        if (tableClient == null) throw new ArgumentNullException(nameof(tableClient));
        if (filterExpression == null) throw new ArgumentNullException(nameof(filterExpression));
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        var filter = TableClient.CreateQueryFilter(filterExpression);
        var queryResults = tableClient.QueryAsync<TEntity>(filter: filter);

        TValue? maxValue = default;

        await foreach (var entity in queryResults)
        {
            var currentValue = selector(entity);
            if (EqualityComparer<TValue>.Default.Equals(maxValue, default) || Comparer<TValue>.Default.Compare(currentValue, maxValue) > 0)
            {
                maxValue = currentValue;
            }
        }

        return maxValue;
    }
}
