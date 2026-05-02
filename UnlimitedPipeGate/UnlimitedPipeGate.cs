using System;
using ILogger = Core.Logging.ILogger;
using ShapezShifter.Hijack;
using ShapezShifter.Flow;

namespace UnlimitedPipeGate
{

    public class UnlimitedPipeGate : IMod
    {
        public UnlimitedPipeGate(ILogger logger)
        {
            AdjustValve();
            logger.Info?.Log("Unlimited Pipe Gates!");
        }

        public void Dispose()
        {
        }

        private void AdjustValve()
        {
            // PipeGateMetaBuildingDefinition
            // ShapezShifter.Hijack.IBuildingsRewirer
            // ShapezShifter.Hijack.IBuildingModulesRewirer
            // IBuildingsRewirer.ModifyGameBuildings()
            // BuildingDefinitionId definitionId = GameBuildings.PipeGateBuildingId;
        }

        private void AdjustValveAlt()
        {
            //IBuildingModulesRewirer.
            //PipeGateBuildingModuleDataProvider
            
        }
    }
}
