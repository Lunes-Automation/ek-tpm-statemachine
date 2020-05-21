using EK.TPM.Data;
using EK.TPM.Data.Attributes;
using EK.TPM.Data.Definitions;
using EK.TPM.Runtime.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EK.TPM.Runtime
{
    /// <summary>
    /// Base class for individual steps in the state machine.
    /// </summary>
    public abstract class ProcessStep : IProcessStep
    {
        //======================================================
        //      Private fields and constants
        //======================================================

        //======================================================
        //      Constructors and finalizers
        //======================================================
        /// <summary>
        /// Constructs a new, empty instance of this class and assigns default values to all exit points.
        /// </summary>
        public ProcessStep()
        {
            PopulateExitPoints();
        }

        //======================================================
        //      Public methods
        //======================================================
        /// <summary>
        /// Instanciates a new process step from a deserialized description.
        /// </summary>
        /// <param name="data">The data describing the process step.</param>
        /// <returns>The instanciated process step, or null if an error occured.</returns>
        public static ProcessStep Instanciate( ProcessStepInstanceData data )
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Check the runtime type for validity
            var definition = data?.Definition;
            var runtimeType = definition?.RuntimeType;
            if (runtimeType == null)
                return null;

            // Try instanciating an instance of the runtime state
            ProcessStep step = null;
            try
            {
                step = Activator.CreateInstance(runtimeType) as ProcessStep;
            }
            catch
            {
                return null;
            }

            // Set the parameters
            step.Id = data.Id;
            step.Definition = definition;
            foreach ( var parameterKvp in data.ParameterValues )
            {
                try
                {
                    Util.SetMemberValue(step, parameterKvp.Key, parameterKvp.Value);
                }
                catch { }
            }

            return step;
        }

        /// <summary>
        /// Invoked before the transport step is first updated.
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// Invoked when the transport step is left.
        /// </summary>
        public abstract void Leave();

        /// <summary>
        /// Resets the state of the process step.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Invoked to update the transport step.
        /// </summary>
        public abstract void Update();

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets or sets the process-unique ID of the step.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current progress of the step, in the range [0 .. 1]. This may be an estimate.
        /// </summary>
        public abstract double Progress
        {
            get;
        }

        /// <summary>
        /// Gets the definition of the transport process which serves as a prototype for this instance.
        /// </summary>
        public ProcessStepDefinition Definition
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the exit which was reached by this step. This will remain null until an exit has been reached.
        /// </summary>
        public ExitPoint ReachedExit
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the containing process of this process step.
        /// </summary>
        public TransportProcess Parent
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
        /// <summary>
        /// Assigns values to all variables of type <see cref="ExitPoint"/> marked with the <see cref="ExitPointAttribute"/> in the current instance.
        /// </summary>
        private void PopulateExitPoints()
        {
            var memberInfoTuples = GetType()
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(mi => (mi, attr: mi.GetCustomAttribute<ExitPointAttribute>()))
                .Where(tuple => tuple.attr != null);

            foreach ( var (memberInfo, attribute) in memberInfoTuples )
            {
                var type = Util.GetMemberInfoVariableType(memberInfo);
                if (type == null)
                    continue;

                if (!(type.IsAssignableFrom(typeof(ExitPoint))))
                    continue;

                var exitPoint = new ExitPoint()
                {
                    Definition = new ExitPointDefinition()
                    {
                        IsError = attribute.IsError,
                        Name = attribute.Name ?? memberInfo.Name
                    }
                };

                Util.SetMemberValue(this, memberInfo, exitPoint);
            }
        }
    }

} // End of namespace EK.TPM.StateMachine
