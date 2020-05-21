using EK.TPM.Data.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Data
{
    /// <summary>
    /// Contains a serializable description of an instance of a process step inside a transport process.
    /// </summary>
    public class ProcessStepInstanceData
    {
        //======================================================
        //      Private fields and constants
        //======================================================

        //======================================================
        //      Constructors and finalizers
        //======================================================
        /// <summary>
        /// Constructs a new, empty instance of this class.
        /// </summary>
        public ProcessStepInstanceData()
        {
            ParameterValues = new Dictionary<string, object>();
        }

        //======================================================
        //      Public methods
        //======================================================

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets or sets the unique Id for this step instance. There must not be multiple steps with the same Id inside the same process.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="ProcessStepDefinition"/> which is the prototype for this instance.
        /// </summary>
        public string DefinitionName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the process step definition which serves as the prototype for this instance.
        /// </summary>
        public ProcessStepDefinition Definition
        {
            get
            {
                if (ProcessStepDefinition.Definitions.TryGetValue(DefinitionName, out var result))
                    return result;

                return null;
            }

            set
            {
                DefinitionName = value?.Name;
            }
        }

        /// <summary>
        /// Maps the parameter names to values. The names must match the parameters defined in the <see cref="Definition"/>.
        /// </summary>
        public Dictionary<string, object> ParameterValues
        {
            get;
            protected set;
        }

        //======================================================
        //      Events
        //======================================================

        //======================================================
        //      Private methods
        //======================================================
    }

} // End of namespace EK.TPM.Data
