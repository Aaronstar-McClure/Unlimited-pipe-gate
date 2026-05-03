using System;
using ILogger = Core.Logging.ILogger;
using ShapezShifter.Hijack;
using Game.Core.Rendering.MeshGeneration;

namespace UnlimitedPipeGate
{
    public class UnlimitedPipeGate : IMod
    {
        /// Alternative way to setup logging in the mod so that sub classes/methods can access it
        private readonly ILogger logger;

        public UnlimitedPipeGate(ILogger logger)
        {
            this.logger = logger;
            // call our setup code we want to run at game startup
            AdjustValve();
            // Log a message to indicate the mod is active
            logger.Info?.Log("Unlimited Pipe Gates!");
        }

        public void Dispose()
        {
        }

        /// Registers the PipeGateModifier rewirer with GameRewirers (ShapezShifter.Hijack API).
        /// GameRewirers.AddRewirer() hooks the rewirer into the game's initialization pipeline.
        /// The rewirer will be called during building definitions setup.
        private void AdjustValve()
        {
            GameRewirers.AddRewirer(new PipeGateModifier(logger));
        }

        /// Inner class implementing IBuildingsRewirer from ShapezShifter.Hijack.
        /// Rewirers intercept and modify game data during initialization.
        /// This rewirer specifically modifies the pipe gate's fluid container configuration.
        private class PipeGateModifier : IBuildingsRewirer
        {
            private readonly ILogger logger;

            public PipeGateModifier(ILogger logger)
            {
                this.logger = logger;
            }

            /// Required method from IBuildingsRewirer interface.
            /// Called by ShapezShifter during game initialization to allow mods to modify building definitions.
            /// Parameters:
            /// - metaBuildings: Contains metadata about all building types in current game mode
            /// - gameBuildings: The GameBuildings collection containing all building definition groups
            /// - meshCache: Cache for building mesh assets (IMeshCache interface)
            /// - theme: Visual theme resources (VisualThemeBaseResources interface)
            public GameBuildings ModifyGameBuildings(
                MetaGameModeBuildings metaBuildings,
                GameBuildings gameBuildings,
                IMeshCache meshCache,
                VisualThemeBaseResources theme)
            {
                // Get the pipe gate's building definition group ID from GameBuildings singleton
                // GameBuildings.PipeGateBuildingId is a property that returns the pipe gate's BuildingDefinitionGroupId
                var pipeGateId = gameBuildings.PipeGateBuildingId;
                
                // Search the GameBuildings.All collection (IReadOnlyList<IBuildingDefinitionGroup>)
                // for the building definition group matching the pipe gate ID
                var pipeGate = gameBuildings.All.FirstOrDefault(b => b.Id == pipeGateId);

                // IBuildingDefinitionGroup.Definitions returns IReadOnlyList<IBuildingDefinition>
                // containing variant definitions for this building type (different rotations/configurations)
                foreach (var definition in pipeGate.Definitions)
                {
                    // TryConfigAs is an extension method from Game.Content.Features.BuildingConfigurationExtensions
                    // It attempts to extract the configuration object of type TConfig from the definition's CustomData
                    // Returns true if found and casts successfully; out parameter receives the configuration
                    if (definition.TryConfigAs<PipeGateMetaBuildingDefinition.Configuration>(out var pipeGateConfig))
                    {
                        logger.Info?.Log($"Original ProvidingRate: {pipeGateConfig.ContainerConfig.ProvidingRate}");
                        logger.Info?.Log($"Original ConsumingRate: {pipeGateConfig.ContainerConfig.ConsumingRate}");
                        
                        // PipeGateMetaBuildingDefinition.Configuration contains a StoringFluidContainerConfiguration
                        // StoringFluidContainerConfiguration has ProvidingRate and ConsumingRate properties (FluidRate type)
                        // These control the fluid throughput when the gate is open
                        pipeGateConfig.ContainerConfig.ProvidingRate *= 120;
                        pipeGateConfig.ContainerConfig.ConsumingRate *= 120;
                        
                        logger.Info?.Log($"Modified ProvidingRate: {pipeGateConfig.ContainerConfig.ProvidingRate}");
                        logger.Info?.Log($"Modified ConsumingRate: {pipeGateConfig.ContainerConfig.ConsumingRate}");
                    }
                }

                // Return the modified GameBuildings collection
                // The modifications to configurations are applied by reference, so changes persist
                return gameBuildings;
            }
        }
    }
}
