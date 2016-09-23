﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// Helper functions for building SQL statements.
    /// </summary>
    public static class SqlBuilder
    {

        /// <summary>
        /// Gets parameters from an argument value.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="argumentValue">The argument value .</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static List<TParameter> GetParameters<TParameter>(object argumentValue)
            where TParameter : DbParameter, new()
        {
            return GetParameters(argumentValue, () => new TParameter());
        }

        /// <summary>
        /// Gets parameters from an argument value.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="argumentValue">The argument value .</param>
        /// <param name="parameterBuilder">The parameter builder. This should set the parameter's database specific DbType property.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static List<TParameter> GetParameters<TParameter>(object argumentValue, Func<TParameter> parameterBuilder)
            where TParameter : DbParameter
        {
            if (parameterBuilder == null)
                throw new ArgumentNullException(nameof(parameterBuilder), $"{nameof(parameterBuilder)} is null.");

            var result = new List<TParameter>();

            if (argumentValue is TParameter)
                result.Add((TParameter)argumentValue);
            else if (argumentValue is IEnumerable<TParameter>)
                foreach (var param in (IEnumerable<TParameter>)argumentValue)
                    result.Add(param);
            else if (argumentValue is IReadOnlyDictionary<string, object>)
                foreach (var item in (IReadOnlyDictionary<string, object>)argumentValue)
                {
                    var param = parameterBuilder();
                    param.ParameterName = (item.Key.StartsWith("@", StringComparison.OrdinalIgnoreCase)) ? item.Key : "@" + item.Key;
                    param.Value = item.Value ?? DBNull.Value;
                    result.Add(param);
                }
            else if (argumentValue != null)
                foreach (var property in MetadataCache.GetMetadata(argumentValue.GetType()).Properties.Where(x => x.MappedColumnName != null))
                {
                    var param = parameterBuilder();
                    param.ParameterName = (property.MappedColumnName.StartsWith("@", StringComparison.OrdinalIgnoreCase)) ? property.MappedColumnName : "@" + property.MappedColumnName;
                    param.Value = property.InvokeGet(argumentValue) ?? DBNull.Value;
                    result.Add(param);
                }

            return result;
        }

    }


}
