using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Data.Attributes
{
    /// <summary>
    /// Marks a property or field as an exit point of a process step. Must be applied to a variable of type 'ExitPoint'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExitPointAttribute : Attribute
    {
        //======================================================
        //      Private fields and constants
        //======================================================

        //======================================================
        //      Constructors and finalizers
        //======================================================
        /// <summary>
        /// Marks a property or field as an exit point of a process step. Must be applied to a variable of type 'ExitPoint'.
        /// </summary>
        public ExitPointAttribute()
        {
        }

        //======================================================
        //      Public methods
        //======================================================

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets or sets if this exit point is invoked due to an error.
        /// </summary>
        public bool IsError
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the exit point. When set to null, the member name of the target property or field is used.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        //======================================================
        //      Events
        //======================================================

        //======================================================
        //      Private methods
        //======================================================
    }

} // End of namespace EK.TPM.Data.Attributes
