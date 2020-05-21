using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Data
{
    /// <summary>
    /// Contains a serializable description of an edge between two steps in a transport process.
    /// </summary>
    public class EdgeData
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
        /// Gets or sets the <see cref="ProcessStepInstanceData.Id"/> of the source step.
        /// </summary>
        public int SourceId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the exit point on the source where the edge starts.
        /// </summary>
        public string SourceExit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="ProcessStepInstanceData.Id"/> of the destination step.
        /// </summary>
        public int DestinationId
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

} // End of namespace EK.TPM.Data
