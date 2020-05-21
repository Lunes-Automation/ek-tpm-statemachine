using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Data.Definitions
{
    /// <summary>
    /// Contains the definition of a parameter for a process step.
    /// </summary>
    public class ParameterDefinition
    {
        //======================================================
        //      Private fields and constants
        //======================================================

        //======================================================
        //      Constructors and finalizers
        //======================================================
        /// <summary>
        /// Constructs a new, empty parameter definition.
        /// </summary>
        public ParameterDefinition()
        {

        }

        //======================================================
        //      Public methods
        //======================================================

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the variable type of the parameter.
        /// </summary>
        public Type Type
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
        }

        //======================================================
        //      Events
        //======================================================

        //======================================================
        //      Private methods
        //======================================================
    }

} // End of namespace EK.TPM.StateMachine
