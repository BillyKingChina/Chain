using System;

namespace Tortuga.Chain
{
    /// <summary>
    /// Controls what happens when performing a model-based insert or update 
    /// </summary>
    [Flags]
    public enum UpsertOptions
    {

        /// <summary>
        /// Update all non-primary key columns using the primary key columns for the where clause.
        /// </summary>
        None = 0,

        /// <summary>
        /// Uses the IPropertyChangeTracking interface to only update changed properties. This flag has no effect when performing an insert.
        /// </summary>
        /// <remarks>If this flag is set and IPropertyChangeTracking.IsChanged is false, an error will occur.</remarks>
        ChangedPropertiesOnly = 1,

        /// <summary>
        /// Ignore the primary keys on the table and perform the update using the Key attribute on properties to construct the where clause.
        /// </summary>
        /// <remarks>This is generally used for heap-style tables, though technically heap tables may have primary, non-clustered keys.</remarks>
        UseKeyAttribute = 2
    }
}
