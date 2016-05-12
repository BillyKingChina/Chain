﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.Materializers;
namespace Tortuga.Chain.SqlServer.CommandBuilders
{

    /// <summary>
    /// Class SqlServerProcedureCall.
    /// </summary>
    internal sealed class SqlServerProcedureCall : MultipleTableDbCommandBuilder<SqlCommand, SqlParameter>, ISupportsChangeListener
    {

        readonly object m_ArgumentValue;
        readonly StoredProcedureMetadata<SqlServerObjectName, SqlDbType> m_Metadata;
        readonly SqlServerObjectName m_ProcedureName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProcedureCall"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="argumentValue">The argument value.</param>
        internal SqlServerProcedureCall(SqlServerDataSourceBase dataSource, SqlServerObjectName procedureName, object argumentValue = null) : base(dataSource)
        {
            if (procedureName == SqlServerObjectName.Empty)
                throw new ArgumentException($"{nameof(procedureName)} is empty", nameof(procedureName));

            m_ArgumentValue = argumentValue;
            m_ProcedureName = procedureName;
            m_Metadata = DataSource.DatabaseMetadata.GetStoredProcedure(procedureName);
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
        public override CommandExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            if (materializer == null)
                throw new ArgumentNullException("materializer", "materializer is null.");

            List<SqlParameter> parameters;

            if (m_ArgumentValue is IEnumerable<SqlParameter>)
            {
                parameters = ((IEnumerable<SqlParameter>)m_ArgumentValue).ToList();
            }
            else
            {
                var sqlBuilder = m_Metadata.CreateSqlBuilder(StrictMode);
                sqlBuilder.ApplyArgumentValue(DataSource, OperationTypes.None, m_ArgumentValue);
                parameters = sqlBuilder.GetParameters();
            }

            return new SqlServerCommandExecutionToken(DataSource, m_ProcedureName.ToString(), m_ProcedureName.ToQuotedString(), parameters, CommandType.StoredProcedure);

        }

        /// <summary>
        /// Waits for change in the data that is returned by this operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns>Task that can be waited for.</returns>
        /// <remarks>This requires the use of SQL Dependency</remarks>
        public Task WaitForChange(CancellationToken cancellationToken, object state = null)
        {
            return WaitForChangeMaterializer.GenerateTask(this, cancellationToken, state);
        }

        SqlServerCommandExecutionToken ISupportsChangeListener.Prepare(Materializer<SqlCommand, SqlParameter> materializer)
        {
            return (SqlServerCommandExecutionToken)Prepare(materializer);
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public new SqlServerDataSourceBase DataSource
        {
            get { return (SqlServerDataSourceBase)base.DataSource; }
        }

        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata TryGetColumn(string columnName)
        {
            return null;
        }
    }

}




