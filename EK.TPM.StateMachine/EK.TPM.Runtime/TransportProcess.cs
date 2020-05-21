using EK.TPM.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Runtime
{
    /// <summary>
    /// The main class of the transport process runtime.
    /// Contains the state machine logic for updating and routing through process states and loading processes from a deserialized description.
    /// </summary>
    public class TransportProcess
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
        public TransportProcess()
        {
            Edges = new List<Edge>();
            Steps = new List<ProcessStep>();
            Parameters = new Dictionary<string, object>();
        }

        //======================================================
        //      Public methods
        //======================================================
        public void Add( ProcessStep step )
        {
            if (step == null)
                return;

            Steps.Add(step);
            step.Parent = this;
        }

        public void Add(Edge edge)
        {
            if (edge == null)
                return;

            Edges.Add(edge);
        }

        /// <summary>
        /// Instanciates a new transport process which implements the logic described in a deserialized instance description.
        /// </summary>
        /// <param name="data">The deserialized transport process instance description.</param>
        /// <returns>The instanciated transport process, or null if an error occured.</returns>
        public static TransportProcess Instanciate(ProcessInstanceData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var result = new TransportProcess();

            // Construct states
            foreach (var stepData in data.Steps)
            {
                // Construct the state
                var step = ProcessStep.Instanciate(stepData);

                // Add the step to the automaton
                result.Add(step);
            }

            // Construct edges
            foreach ( var edgeData in data.Edges )
            {
                Edge edge = new Edge
                {
                    Source = result.Steps.FirstOrDefault(s => s.Id == edgeData.SourceId),
                    Destination = result.Steps.FirstOrDefault(s => s.Id == edgeData.DestinationId)
                };
                edge.SourceExit = edge.Source?.Definition?.Exits.FirstOrDefault(e => e.Name == edgeData.SourceExit);

                result.Add(edge);
            }

            // Set initial state
            result.InitialState = result.Steps.FirstOrDefault(s => s.Id == data.InitialStateId);

            return result;
        }

        /// <summary>
        /// Generates an instance description for this process.
        /// </summary>
        /// <returns>A serializable description of the automaton with its parameters, or null if an error occured.</returns>
        public ProcessInstanceData Save()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the next steps from a given step in the process.
        /// </summary>
        /// <param name="source">The source step from which edges are searched.</param>
        /// <param name="exit">The exit from which the edge should start.</param>
        /// <returns>A possibly empty list of states which follow in the automaton based on the defined <see cref="Edges"/>.</returns>
        public IEnumerable<ProcessStep> FindNextSteps(ProcessStep source, ExitPoint exit)
        {
            if (exit == null)
                throw new ArgumentNullException(nameof(exit));

            return Edges
                .Where(e => e.Source == source && e.SourceExit == exit.Definition)
                .Select(e => e.Destination);
        }

        /// <summary>
        /// Resets the automaton to the initial state.
        /// </summary>
        public void Reset()
        {
            if (CurrentStep == null)
                CurrentStep = InitialState;

            CurrentStep?.Reset();
        }

        /// <summary>
        /// Updates the state machine. This should be invoked periodically.
        /// </summary>
        public void Update()
        {
            if (CurrentStep == null)
                Reset();

            CurrentStep.Update();
            if (CurrentStep.ReachedExit != null)
            {
                var nextSteps = FindNextSteps(CurrentStep, CurrentStep.ReachedExit);
                if (nextSteps.Count() > 1)
                    throw new NotImplementedException();

                var nextStep = nextSteps.FirstOrDefault();
                CurrentStep.Leave();
                CurrentStep = nextStep;

                if (nextStep != null)
                {
                    nextStep.Enter();
                    Console.WriteLine("New step: " + nextStep.GetType().Name);
                }
                else
                {
                    Console.WriteLine("Process finished!");
                    IsFinished = true;
                }
            }
        }

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// Gets the current step of the state machine.
        /// </summary>
        public ProcessStep CurrentStep
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets if the transport process is finished and will not continue to change upon further invocations of <see cref="Update"/>.
        /// </summary>
        public bool IsFinished
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the initial state of the automaton. This should be set before starting the update loop and is applied through the <see cref="Reset"/> method.
        /// </summary>
        public ProcessStep InitialState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of steps used by the automaton.
        /// </summary>
        public List<ProcessStep> Steps
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the list of edges used by the automaton.
        /// </summary>
        public List<Edge> Edges
        {
            get;
            private set;
        }

        /// <summary>
        /// Maps names of process parameters to their values.
        /// </summary>
        public Dictionary<string, object> Parameters
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
    }

} // End of namespace EK.TPM.StateMachine
