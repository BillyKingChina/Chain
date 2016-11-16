#if !SqlDependency_Missing
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tortuga.Chain.Appenders;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.SqlServer.Appenders;
#endif

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerAppenders.
    /// </summary>
    public static class SqlServerAppenders
    {
#if !SqlDependency_Missing
        /// <summary>
        /// Attaches a SQL Server dependency change listener to this operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result type.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="eventHandler">The event handler to fire when the underlying data changes.</param>
        /// <returns>Tortuga.Chain.Core.ILink&lt;TResult&gt;.</returns>
        /// <remarks>This will only work for operations against non-transactional SQL Server data sources that also conform to the rules about using SQL Dependency.</remarks>
        public static ILink<TResult> WithChangeNotification<TResult>(this ILink<TResult> previousLink, OnChangeEventHandler eventHandler)
        {
            return new NotifyChangeAppender<TResult>(previousLink, eventHandler);
        }
#endif

        /// <summary>
        /// Gets the execution plan from a materializer.
        /// </summary>
        /// <typeparam name="TResult">The result type of the materializer.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="state">Optional user state.</param>
        /// <returns>XElement.</returns>
        /// <exception cref="System.ArgumentNullException">previousLink - previousLink is null.</exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static XElement GetExecutionPlan<TResult>(this ILink<TResult> previousLink, object state = null)

        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");

            var temp = previousLink;

            //Look for a materializer or at least an appender that we can unwrap.
            while (true)
            {
                if (temp is Materializer<SqlCommand, SqlParameter, TResult>)
                    return (new ExecutionPlanAppender<TResult>((Materializer<SqlCommand, SqlParameter, TResult>)temp)).GetExecutionPlan(state);
                else if (temp is Appender<TResult>)
                    temp = ((Appender<TResult>)temp).PreviousLink;
                else
                    throw new ArgumentException($"Cannot get execution plan from {previousLink.GetType().FullName}.");
            }


        }

        /// <summary>
        /// Gets the execution plan from a materializer.
        /// </summary>
        /// <typeparam name="TResult">The result type of the materializer.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">Optional user state.</param>
        /// <returns>XElement.</returns>
        public static Task<XElement> GetExecutionPlanAsync<TResult>(this ILink<TResult> previousLink, CancellationToken cancellationToken = default(CancellationToken), object state = null)
        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");

            var temp = previousLink;

            //Look for a materializer or at least an appender that we can unwrap.
            while (true)
            {
                if (temp is Materializer<SqlCommand, SqlParameter, TResult>)
                    return (new ExecutionPlanAppender<TResult>((Materializer<SqlCommand, SqlParameter, TResult>)temp)).GetExecutionPlanAsync(cancellationToken, state);
                else if (temp is Appender<TResult>)
                    temp = ((Appender<TResult>)temp).PreviousLink;
                else
                    throw new ArgumentException($"Cannot get execution plan from {previousLink.GetType().FullName}.");
            }
        }
    }
}
