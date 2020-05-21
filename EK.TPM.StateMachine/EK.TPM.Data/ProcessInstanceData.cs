using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EK.TPM.Data
{
    /// <summary>
    /// Contains a serializable description of a transport process instance.
    /// </summary>
    public class ProcessInstanceData
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
        public ProcessInstanceData()
        {
            Steps = new List<ProcessStepInstanceData>();
            Edges = new List<EdgeData>();
        }

        //======================================================
        //      Public methods
        //======================================================
        /// <summary>
        /// Loads the data from a stream.
        /// </summary>
        /// <param name="stream">The stream from which to load the data. Must be readable.</param>
        /// <returns></returns>
        public static ProcessInstanceData Load(Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the data to a stream.
        /// </summary>
        /// <param name="stream">The stream to write the data to. Must be writeable.</param>
        public void Save(Stream stream)
        {
            throw new NotImplementedException();
        }

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets or sets the human readable name of the process.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of process steps configured for this process, in any order.
        /// </summary>
        public List<ProcessStepInstanceData> Steps
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the list of edges between process steps in this process.
        /// </summary>
        public List<EdgeData> Edges
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the <see cref="ProcessStepInstanceData.Id"/> of the initial state in the process.
        /// </summary>
        public int InitialStateId
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
