using EK.TPM.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EK.TPM.Runtime.Library
{
    /// <summary>
    /// Dummy implementation of a deliver step which randomly selects an outcome (success/fail) after some time.
    /// </summary>
    public class DeliverStepDummy : ProcessStep
    {
        //======================================================
        //      Private fields and constants
        //======================================================
        private Stopwatch stopwatch;
        private double duration;

        //======================================================
        //      Constructors and finalizers
        //======================================================
        public DeliverStepDummy()
        {
            stopwatch = new Stopwatch();
        }

        //======================================================
        //      Public methods
        //======================================================
        public override void Enter()
        {
            Reset();
        }

        public override void Leave()
        {
            stopwatch.Stop();
        }

        public override void Reset()
        {
            ReachedExit = null;
            duration = new Random().NextDouble() * 15;
            stopwatch.Restart();
        }

        public override void Update()
        {
            if (stopwatch.Elapsed.TotalSeconds >= duration)
            {
                if (ReachedExit == null)
                {
                    if (new Random().Next(2) == 0)
                    {
                        Console.WriteLine("Failed to deliver load to " + DeliverDestination);
                        ReachedExit = Failed;
                    }
                    else
                    {
                        Console.WriteLine("Delivered load to " + DeliverDestination);
                        ReachedExit = Done;
                    }
                }
            }
            else
                ReachedExit = null;
        }

        //======================================================
        //      Properties
        //======================================================
        public override double Progress
            => Math.Min(stopwatch.Elapsed.TotalMilliseconds / duration, 1);

        //======================================================
        //      Parameters
        //======================================================
        /// <summary>
        /// Gets the deliver destination set in the parent process.
        /// </summary>
        [ParameterDefinition(IsOptional = false, Kind = Data.Definitions.ParameterDefinitionKind.InferredForOrder)]
        public int DeliverDestination
            => (int)Parent.Parameters[nameof(DeliverDestination)];

        //======================================================
        //      Exit points
        //======================================================
        [ExitPoint]
        public ExitPoint Done
        {
            get;
            set;
        }

        [ExitPoint(IsError = true)]
        public ExitPoint Failed
        {
            get;
            set;
        }
    }

} // End of namespace EK.TPM.Runtime.Library
