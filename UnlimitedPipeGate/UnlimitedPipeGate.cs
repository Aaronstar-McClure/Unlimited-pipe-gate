using System;
using ILogger = Core.Logging.ILogger;
using ShapezShifter.Hijack;
using Game.Core.Rendering.MeshGeneration;

namespace UnlimitedPipeGate
{

    public class UnlimitedPipeGate : IMod
    {
        private readonly ILogger logger;

        public UnlimitedPipeGate(ILogger logger)
        {
            this.logger = logger;
            AdjustValve();
            logger.Info?.Log("Unlimited Pipe Gates!");
        }

        public void Dispose()
        {
        }

        private void AdjustValve()
        {
            // Since Flow is for adding new buildings, we use Hijack to modify the existing pipe gate
            GameRewirers.AddRewirer(new PipeGateModifier(logger));
        }

        private class PipeGateModifier : IBuildingsRewirer
        {
            private readonly ILogger logger;

            public PipeGateModifier(ILogger logger)
            {
                this.logger = logger;
            }

            public GameBuildings ModifyGameBuildings(
                MetaGameModeBuildings metaBuildings,
                GameBuildings gameBuildings,
                IMeshCache meshCache,
                VisualThemeBaseResources theme)
            {
                // Find the pipe gate building
                var pipeGateId = gameBuildings.PipeGateBuildingId;
                var pipeGate = gameBuildings.All.FirstOrDefault(b => b.Id == pipeGateId);

                foreach (var definition in pipeGate.Definitions)
                {
                    if (definition.TryConfigAs<PipeGateMetaBuildingDefinition.Configuration>(out var pipeGateConfig))
                    {
                        pipeGateConfig.ContainerConfig.ProvidingRate *= 24;
                        pipeGateConfig.ContainerConfig.ConsumingRate *= 24;
                        logger.Info?.Log($"Modified ProvidingRate: {pipeGateConfig.ContainerConfig.ProvidingRate}");
                        logger.Info?.Log($"Modified ConsumingRate: {pipeGateConfig.ContainerConfig.ConsumingRate}");
                    }
                }

                return gameBuildings;
            }
        }
    }
}
