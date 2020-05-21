using EK.TPM.Data.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Data.Attributes
{
    /// <summary>
    /// Marks a variable as a parameter for a process step.
    /// </summary>
    public class ParameterDefinitionAttribute : Attribute
    {
        //======================================================
        //      Private fields and constants
        //======================================================

        //======================================================
        //      Constructors and finalizers
        //======================================================
        /// <summary>
        /// Marks a variable as a parameter for a process step.
        /// </summary>
        public ParameterDefinitionAttribute()
        {
        }

        //======================================================
        //      Public methods
        //======================================================

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets or sets the name of the parameter. If set to null, the name of the underlying field or property is used.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets if this parameter is optional.
        /// </summary>
        public bool IsOptional
        {
            get;
            set;
        } = true;

        /// <summary>
        /// Gets or sets the kind of the parameter, i.e. if it is used for order parameter inference or not.
        /// </summary>
        public ParameterDefinitionKind Kind
        {
            get;
            set;
        } = ParameterDefinitionKind.Standard;

        //======================================================
        //      Events
        //======================================================

        //======================================================
        //      Private methods
        //======================================================
    }

} // End of namespace EK.TPM.Data.Attributes
