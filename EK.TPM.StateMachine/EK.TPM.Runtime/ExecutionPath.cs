using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Runtime
{
    /// <summary>
    /// TODO: Description of this class.
    /// </summary>
    public class ExecutionPath
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
        public ProcessStep Start
        {
            get;
            set;
        }

        public ProcessStep End
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
