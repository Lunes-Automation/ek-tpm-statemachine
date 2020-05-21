using EK.TPM.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EK.TPM.Data.Definitions
{
    /// <summary>
    /// Contains the definition of a process step, which can then be instanciated.
    /// </summary>
    public class ProcessStepDefinition : IProcessStepDefinition
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
        /// <summary>
        /// Globally registers a process step definition.
        /// </summary>
        /// <param name="definition">The definition to register.</param>
        public static void Register(ProcessStepDefinition definition)
        {
            Definitions.Add(definition.Name, definition);
        }

        /// <summary>
        /// Globally registers a process step definition through its runtime type representation.
        /// </summary>
        /// <typeparam name="TProcessStep">The type of the runtime process step. Must be a subclass of 'ProcessStep'.</typeparam>
        public static void Register<TProcessStep>()
        {
            Register(typeof(TProcessStep));
        }

        /// <summary>
        /// Globally registers a process step definition through its runtime type representation.
        /// </summary>
        /// <param name="processStepType">The type of the runtime process step. Must be a subclass of 'ProcessStep'.</param>
        public static void Register(Type processStepType)
        {
            ProcessStepDefinition definition = new ProcessStepDefinition
            {
                Name = processStepType.Name,
                Exits = FindExitPointDefinitions(processStepType),
                Parameters = FindParameters(processStepType, ParameterDefinitionKind.Standard),
                InferredOrderParameters = FindParameters(processStepType, ParameterDefinitionKind.InferredForOrder),
                RuntimeType = processStepType
            };

            Register(definition);
        }

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets the list of inferred parameter by a list of process step definitions.
        /// </summary>
        /// <param name="processSteps">The list of used process steps.</param>
        /// <returns>A query containing all the inferred paramters by the steps.</returns>
        public static IEnumerable<ParameterDefinition> GetInferredParameters(IEnumerable<ProcessStepDefinition> processSteps)
        {
            return processSteps
                .SelectMany(step => step.InferredOrderParameters)
                .Distinct(new LambdaEqualityComparer<ParameterDefinition, string>(new Func<ParameterDefinition, string>(p => p.Name)));
        }

        /// <summary>
        /// Maps process step names to their definitions.
        /// </summary>
        public static Dictionary<string, ProcessStepDefinition> Definitions
        {
            get;
            protected set;
        } = new Dictionary<string, ProcessStepDefinition>();

        /// <summary>
        /// The name of this process step.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of parameters which are inferred for an order which contains an instance of this step.
        /// </summary>
        public List<ParameterDefinition> InferredOrderParameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of parameters for this process step.
        /// </summary>
        public List<ParameterDefinition> Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of exit points for this process step.
        /// </summary>
        public List<ExitPointDefinition> Exits
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type which is used to instanciate this process step in a transport process.
        /// </summary>
        public Type RuntimeType
        {
            get;
            private set;
        }

        //======================================================
        //      Events
        //======================================================

        //======================================================
        //      Private methods
        //======================================================
        private static List<ExitPointDefinition> FindExitPointDefinitions( Type processStepType )
        {
            var memberInfoTuples = processStepType
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(mi => (mi, attr: mi.GetCustomAttribute<ExitPointAttribute>()))
                .Where(tuple => tuple.attr != null);

            List<ExitPointDefinition> exitPointDefinitions = new List<ExitPointDefinition>();
            foreach (var (memberInfo, attribute) in memberInfoTuples)
            {
                var type = Util.GetMemberInfoVariableType(memberInfo);
                if (type == null)
                    continue;

                exitPointDefinitions.Add(new ExitPointDefinition()
                {
                    IsError = attribute.IsError,
                    Name = attribute.Name ?? memberInfo.Name
                });
            }

            return exitPointDefinitions;
        }

        private static List<ParameterDefinition> FindParameters(Type processStepType, ParameterDefinitionKind kind)
        {
            var memberInfoTuples = processStepType
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(mi => (mi, attr: mi.GetCustomAttribute<ParameterDefinitionAttribute>()))
                .Where(tuple => tuple.attr != null)
                .Where(tuple => tuple.attr.Kind == kind);

            var parameterDefinitions = new List<ParameterDefinition>();
            foreach (var (memberInfo, attribute) in memberInfoTuples)
            {
                var type = Util.GetMemberInfoVariableType(memberInfo);
                if (type == null)
                    continue;

                parameterDefinitions.Add(new ParameterDefinition()
                {
                    IsOptional = attribute.IsOptional,
                    Name = attribute.Name ?? memberInfo.Name,
                    Type = type
                });
            }

            return parameterDefinitions;
        }
    }

} // End of namespace EK.TPM.StateMachine.Base
