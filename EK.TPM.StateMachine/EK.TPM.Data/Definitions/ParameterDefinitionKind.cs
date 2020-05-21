using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Data.Definitions
{
    /// <summary>
    /// Enumerates process step parameter kinds, i.e. if they are used for order parameter inference or not.
    /// </summary>
    public enum ParameterDefinitionKind
    {
        /// <summary>
        /// The parameter is a parameter for the process step itself.
        /// </summary>
        Standard,

        /// <summary>
        /// The parameter is added to the list of the parameters of the order when the step is used in a transport process.
        /// </summary>
        InferredForOrder
    }

} // End of namespace EK.TPM.StateMachine.Enums
