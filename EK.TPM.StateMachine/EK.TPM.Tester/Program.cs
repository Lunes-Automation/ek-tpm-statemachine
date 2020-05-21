
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EK.TPM.Data;
using EK.TPM.Data.Definitions;
using EK.TPM.Runtime;
using EK.TPM.Runtime.Library;

namespace EK.TPM.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            //==========================================================================
            // Option 1: Load the process configuration as raw data, i.e. from the database
            // Then register the known process steps and construct an instance of the runtime.
            // First register the known process steps
            LoadProcessStepDefinitions();

            // Load the process configuration from the database and instanciate it
            ProcessInstanceData data = LoadProcessDescriptionFromDatabase();
            TransportProcess loadedProcess = TransportProcess.Instanciate(data);

            //==========================================================================
            // Option 2: Instanciate a new executable process directly at runtime.
            var sampleProcess = CreateSampleProcess();

            // We set the parameters for the transport process before we can run it.
            // These parameters are equivalent to those passed in the current TPM implementation via CustomOrder.NewOrder() method.
            // We can infer the parameters we will need for the process from the used steps.
            var inferredParameters = ProcessStepDefinition.GetInferredParameters(loadedProcess.Steps.Select(step => step.Definition));
            loadedProcess.Parameters[inferredParameters.FirstOrDefault().Name] = 10001010;

            // Run the state machine.
            while (true)
            {
                loadedProcess.Update();
                Thread.Sleep(100);
            }
        }

        private static TransportProcess CreateSampleProcess()
        {
            TransportProcess process = new TransportProcess();
            WaitStep stepA;
            DeliverStepDummy stepB;
            process.Add(stepA = new WaitStep() { Duration = 12.0 });
            process.Add(stepB = new DeliverStepDummy());
            process.Add(new Edge() { Source = stepA, Destination = stepB, SourceExit = ExitPointDefinition.Default });
            process.Add(new Edge() { Source = stepB, Destination = stepA, SourceExit = stepB.Done.Definition });
            process.Add(new Edge() { Source = stepB, Destination = stepB, SourceExit = stepB.Failed.Definition });
            process.InitialState = stepA;

            return process;
        }

        private static ProcessInstanceData LoadProcessDescriptionFromDatabase()
        {
            // Configure the transport process
            // This configuration should be done in the UI and only loaded from the database/file at this point.
            // No logic is contained in these classes, pure data level.
            // The setup is only meant to show the usage of the classes.
            var data = new ProcessInstanceData();
            data.Name = "My transport process";
            ProcessStepInstanceData dataA, dataB;
            data.Steps.Add(dataA = new ProcessStepInstanceData() { Id = 1, DefinitionName = "WaitStep" });
            dataA.ParameterValues["Duration"] = 12.0;
            data.Steps.Add(dataB = new ProcessStepInstanceData() { Id = 2, DefinitionName = "DeliverStepDummy" });
            data.InitialStateId = dataA.Id;
            data.Edges.Add(new EdgeData() { SourceId = dataA.Id, DestinationId = dataB.Id, SourceExit = ExitPointDefinition.Default.Name });
            data.Edges.Add(new EdgeData() { SourceId = dataB.Id, DestinationId = dataA.Id, SourceExit = "Done" });
            data.Edges.Add(new EdgeData() { SourceId = dataB.Id, DestinationId = dataB.Id, SourceExit = "Failed" });

            return data;
        }

        private static void LoadProcessStepDefinitions()
        {
            Assembly runtimeLibrary = typeof(WaitStep).Assembly;
            foreach (var processStepType in runtimeLibrary
                .GetTypes()
                .Where(t => typeof(ProcessStep).IsAssignableFrom(t)))
            {
                ProcessStepDefinition.Register(processStepType);
            }
        }
    }
}
