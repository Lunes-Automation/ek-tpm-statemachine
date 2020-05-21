using EK.TPM.Data.Attributes;
using EK.TPM.Data.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EK.TPM.Runtime.Library
{
    /// <summary>
    /// A process step that simply waits for a specified duration.
    /// </summary>
    public class WaitStep : ProcessStep
    {
        //======================================================
        //      Private fields and constants
        //======================================================
        private Stopwatch stopwatch;

        //======================================================
        //      Constructors and finalizers
        //======================================================
        /// <summary>
        /// Constructs a new, empty instance of this class.
        /// </summary>
        public WaitStep()
        {
            stopwatch = new Stopwatch();
        }

        //======================================================
        //      Public methods
        //======================================================
        /// <inheritdoc />
        public override void Enter()
        {
            Reset();
        }

        /// <inheritdoc />
        public override void Leave()
        {
            stopwatch.Stop();
        }

        /// <inheritdoc />
        public override void Reset()
        {
            ReachedExit = null;
            stopwatch.Restart();
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (stopwatch.Elapsed.TotalSeconds >= Duration)
                ReachedExit = Done;
            else
                ReachedExit = null;
        }

        //======================================================
        //      Properties
        //======================================================
        /// <inheritdoc />
        public override double Progress
            => Math.Min(stopwatch.Elapsed.TotalMilliseconds / Duration, 1);

        //======================================================
        //      Step parameters
        //======================================================
        /// <summary>
        /// Gets or sets the duration to wait in this step, in full and fractions of seconds.
        /// </summary>
        [ParameterDefinition(IsOptional = false, Kind = ParameterDefinitionKind.Standard)]
        public double Duration
        {
            get;
            set;
        }

        //======================================================
        //      Exit points
        //======================================================
        /// <summary>
        /// Invoked when the waiting time is over.
        /// </summary>
        [ExitPoint]
        public ExitPoint Done
        {
            get;
            protected set;
        }

        //======================================================
        //      Private methods
        //======================================================

        //======================================================
        //      Subtypes
        //======================================================
    }

} // End of namespace EK.TPM.Runtime.Library