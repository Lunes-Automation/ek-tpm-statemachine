using EK.TPM.Data.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Runtime
{
    /// <summary>
    /// A runtime representation of an exit point of a transport process step.
    /// </summary>
    public class ExitPoint
    {
        //======================================================
        //      Private fields and constants
        //======================================================

        //======================================================
        //      Constructors and finalizers
        //======================================================

        //======================================================
        //      Public methods
        //======================================================

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets or sets the definition for this exit point.
        /// </summary>
        public ExitPointDefinition Definition
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
