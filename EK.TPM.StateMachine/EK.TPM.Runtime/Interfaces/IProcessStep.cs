using System;
using System.Collections.Generic;
using System.Linq;
using EK.TPM.Data.Definitions;

namespace EK.TPM.Runtime.Interfaces
{
    /// <summary>
    /// Base interface for individual steps in the state machine.
    /// </summary>
    public interface IProcessStep
    {
        //======================================================
        //      Public methods
        //======================================================
        /// <summary>
        /// Invoked before the transport step is first updated.
        /// </summary>
        void Enter();

        /// <summary>
        /// Invoked to update the transport step.
        /// </summary>
        void Update();

        /// <summary>
        /// Invoked when the transport step is left.
        /// </summary>
        void Leave();

        /// <summary>
        /// Resets the state of the process step.
        /// </summary>
        void Reset();

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets the current progress of the step, in the range [0 .. 1]. This may be an estimate.
        /// </summary>
        double Progress
        {
            get;
        }

        /// <summary>
        /// Gets the definition of the transport process which serves as a prototype for this instance.
        /// </summary>
        ProcessStepDefinition Definition
        {
            get;
        }

        /// <summary>
        /// Gets the exit which was reached by this step. This will remain null until an exit has been reached.
        /// </summary>
        ExitPoint ReachedExit
        {
            get;
        }

        //======================================================
        //      Events
        //======================================================
    }

} // End of namespace EK.TPM.StateMachine
