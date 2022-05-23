using OpenHardwareMonitor.Hardware;
using System.Collections.Generic;

namespace Resource_Plug.PCsource {

    class HardwareMonitor {
        private readonly Computer _computer;
        private ISensor[] _cpu;
        private ISensor[] _ram;
        private ISensor[] _gpu;

        private ISensor[] getSensorValue(ISensor[] sensors, SensorType type) {
            List<ISensor> resSensor = new List<ISensor>();
            foreach (var sensor in sensors) {
                if (sensor.SensorType == type) {
                    resSensor.Add( sensor );
                }
            }
            return resSensor.ToArray();
        }

        public HardwareMonitor() {

            _computer = new Computer() {
                CPUEnabled = true,
                GPUEnabled = true,
                RAMEnabled = true,
            };
            _computer.Open();
        }


        public void Updata() {
            foreach (var hardware in _computer.Hardware) {
                hardware.Update();
                switch (hardware.HardwareType) {
                    case HardwareType.CPU:
                        _cpu = hardware.Sensors;
                        break;
                    case HardwareType.GpuNvidia:
                        _gpu = hardware.Sensors;
                        break;
                    case HardwareType.GpuAti:
                        _gpu = hardware.Sensors;
                        break;
                    case HardwareType.RAM:
                        _ram = hardware.Sensors;
                        break;

                }
            }
        }

        public static float? GetValueFromSensor(ISensor[] sensors, int index) {
            if(index < sensors.Length) {
                return sensors[index].Value;
            }
            return null;
        }

        public static float? GetValueFromSensor(ISensor[] sensors, string name) {
            foreach (var iten in sensors) {
                if (iten.Name.Equals(name)) {
                    return iten.Value;
                }
            }
            return null;
        }

        public ISensor[] CPULoad {
            get {
                return getSensorValue( _cpu, SensorType.Load );
            }
        }
        public ISensor[] CPUTemperature {
            get {
                return getSensorValue( _cpu, SensorType.Temperature );
            }
        }
        public ISensor[] CPUPower {
            get {
                return getSensorValue( _cpu, SensorType.Power );
            }
        }
        public ISensor[] RAMLoad {
            get {
                return getSensorValue( _ram, SensorType.Load );
            }
        }
        public ISensor[] RAMData {
            get {
                return getSensorValue( _ram, SensorType.Data );
            }
        }
        public ISensor[] GPULoad {
            get {
                return getSensorValue( _gpu, SensorType.Load );
            }
        }
        public ISensor[] GPUTemperature {
            get { 
                return getSensorValue( _gpu, SensorType.Temperature );
            }
        }
        public ISensor[] GPUPower {
            get {
                return getSensorValue( _gpu, SensorType.Power );
            }
        }
        public ISensor[] GPUData {
            get {
                return getSensorValue( _gpu, SensorType.Data );
            }
        }
        public ISensor[] GPUThroughtput {
            get {
                return getSensorValue( _gpu, SensorType.Throughput );
            }
        }
        public void Dispose() {
            _computer.Close();
        }
    }
}
