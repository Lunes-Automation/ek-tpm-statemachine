using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Data.Definitions
{
    /// <summary>
    /// Base interface for a process step definition.
    /// </summary>
    public interface IProcessStepDefinition
    {
        //======================================================
        //      Public methods
        //======================================================

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets the parameters for a transport order which contains this process step.
        /// </summary>
        List<ParameterDefinition> InferredOrderParameters
        {
            get;
        }

        /// <summary>
        /// Gets the parameters for this process step.
        /// </summary>
        List<ParameterDefinition> Parameters
        {
            get;
        }

        /// <summary>
        /// Gets the possible exit points from this step.
        /// </summary>
        List<ExitPointDefinition> Exits
        {
            get;
        }

        //======================================================
        //      Events
        //======================================================
    }

} // End of namespace EK.TPM.StateMachine.Interfaces
