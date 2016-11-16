namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Enum SqlServerCommandExecutionMode
    /// </summary>
    public enum SqlServerCommandExecutionMode
    {
        /// <summary>
        /// Pass the prepared command to the materializer for execution.
        /// </summary>
        Materializer = 0,

        /// <summary>
        /// Capture the execution plan instead of performing the normal operation.
        /// </summary>
        ExecutionPlanXml = 1
    }
}
