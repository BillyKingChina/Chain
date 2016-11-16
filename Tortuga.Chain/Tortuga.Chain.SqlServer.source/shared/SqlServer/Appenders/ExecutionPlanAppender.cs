using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SqlServer.Appenders
{
    internal class ExecutionPlanAppender<TIn>
    {
        readonly Materializer<SqlCommand, SqlParameter, TIn> m_PreviousLink;

        public ExecutionPlanAppender(Materializer<SqlCommand, SqlParameter, TIn> previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");

            m_PreviousLink = previousLink;
        }

        public XElement GetExecutionPlan(object state = null)
        {
            object temp = null;

            var token = m_PreviousLink.Prepare();
            if (token is SqlServerCommandExecutionToken)
                ((SqlServerCommandExecutionToken)token).ExecutionMode = SqlServerCommandExecutionMode.ExecutionPlanXml;

            token.Execute(cmd =>
            {
                temp = cmd.ExecuteScalar();
                return null;
            }, state);

            if (temp == DBNull.Value || temp == null)
                return null;

            return XElement.Parse((string)temp);

        }

        public async Task<XElement> GetExecutionPlanAsync(CancellationToken cancellationToken, object state = null)
        {
            object temp = null;


            var token = m_PreviousLink.Prepare();
            if (token is SqlServerCommandExecutionToken)
                ((SqlServerCommandExecutionToken)token).ExecutionMode = SqlServerCommandExecutionMode.ExecutionPlanXml;

            await token.ExecuteAsync(async cmd =>
            {
                temp = await cmd.ExecuteScalarAsync();
                return null;
            }, cancellationToken, state);

            if (temp == DBNull.Value || temp == null)
                return null;

            return XElement.Parse((string)temp);
        }

    }
}
