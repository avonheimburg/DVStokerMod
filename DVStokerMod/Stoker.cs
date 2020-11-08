    using System;
    using CommandTerminal;

    namespace DVStokerMod
{
    public class Stoker
    {
        private const float MinWaterLevel = SteamLocoSimulation.BOILER_WATER_CAPACITY_L * 0.7f;
        private const float MaxWaterLevel = SteamLocoSimulation.BOILER_WATER_CAPACITY_L * 0.9f;
        private const float WaterLevelRange = MaxWaterLevel - MinWaterLevel;

        private const float CoalLevelLow = SteamLocoSimulation.COALBOX_OPTIMAL_CAPACITY_KG * 0.33f;
        private const float CoalLevelMed = SteamLocoSimulation.COALBOX_OPTIMAL_CAPACITY_KG * 0.67f;
        private const float CoalLevelHigh = SteamLocoSimulation.COALBOX_OPTIMAL_CAPACITY_KG;

        private StokerMode _mode = StokerMode.Off;
        private float _coalTarget = 0f;
        
        
        
        public StokerMode CycleMode()
        {
            _mode = (StokerMode) (((int) _mode + 1) % 4);
            _coalTarget = _mode switch
            {
                StokerMode.Off => 0,
                StokerMode.Low => CoalLevelLow,
                StokerMode.Medium => CoalLevelMed,
                StokerMode.High => CoalLevelHigh,
                _ => throw new ArgumentOutOfRangeException(nameof(_mode))
            };
            return _mode;
        }
        
        public void SimulateTick(SteamLocoSimulation locoSim)
        {
            if (_mode == StokerMode.Off || locoSim == null)
                return;
            
            float steamPressure = locoSim.boilerPressure.value;
            float coalLevel = locoSim.coalbox.value;
            float waterLevel = locoSim.boilerWater.value;
            
            if (
                coalLevel == 0 &&       // For some reason, these values are populated with real data only
                steamPressure == 0 &&   // once every 3 ticks. If they're the values above, then this is
                waterLevel == 14400     // one of those magical skipped ticks. Do nothing.
            )
            {
                return;
            }
            
            // add coal if level is too low
            if (coalLevel < _coalTarget)
            {
                locoSim.AddCoalChunk();
            }

            var injectorSetting = CalculateInjectorSetting(waterLevel);
            locoSim.injector.SetValue(injectorSetting);
        }

        private float CalculateInjectorSetting(float waterLevel)
        {
            // fully open injector if water level is below minimum
            if (waterLevel < MinWaterLevel)
                return 1.0f;
            // calculate injector setting based on water level 
            if (waterLevel < MaxWaterLevel)
                return (MaxWaterLevel - waterLevel) / WaterLevelRange;

            // fully close injector if water level is above maximum
            return 0.0f;
        }

    }

    public enum StokerMode
    {
        Off = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

}