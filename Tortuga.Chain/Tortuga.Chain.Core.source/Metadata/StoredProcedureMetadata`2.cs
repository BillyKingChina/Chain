using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Class StoredProcedureMetadata.
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public sealed class StoredProcedureMetadata<TName, TDbType> : IStoredProcedureMetadata
        where TDbType : struct
    {

        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        public StoredProcedureMetadata(TName name, IList<ParameterMetadata<TDbType>> parameters)
        {
            Name = name;
            Parameters = new ReadOnlyCollection<ParameterMetadata<TDbType>>(parameters);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public TName Name { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public ReadOnlyCollection<ParameterMetadata<TDbType>> Parameters { get; }

        /// <summary>
        /// Creates a SQL builder.
        /// </summary>
        /// <returns></returns>
        public SqlBuilder<TDbType> CreateSqlBuilder(bool strictMode)
        {
            return new SqlBuilder<TDbType>(Name.ToString(), Parameters, strictMode);
        }
    }
}
