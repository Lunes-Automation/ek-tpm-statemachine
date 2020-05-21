using EK.TPM.Data.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Runtime
{
    /// <summary>
    /// The runtime representation of an edge between two <see cref="ProcessStep"/> instances.
    /// </summary>
    public class Edge
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
        /// Gets or sets the source step of the edge.
        /// </summary>
        public ProcessStep Source
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the exit point which, once reached, causes this edge to be followed.
        /// </summary>
        public ExitPointDefinition SourceExit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the destination of the edge.
        /// </summary>
        public ProcessStep Destination
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

} // End of namespace EK.TPM.Runtime
