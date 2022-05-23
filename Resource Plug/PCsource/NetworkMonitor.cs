using System.Collections.Generic;
using System.Diagnostics;
/// <summary>
/// from https://www.cnblogs.com/ZmissW/articles/13691068.html
/// </summary>
namespace Resource_Plug.PCsource {
    internal class NetworkMonitor {



        List<NetworkAdapter> adapters = new List<NetworkAdapter>();
        NetworkAdapter networkAdapter = null;
        public NetworkMonitor() {
            PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");

            foreach (string name in category.GetInstanceNames()) {
                //This one exists on every computer.
                if (name == "MS TCP Loopback interface" || name.Contains("isatap") || name.Contains("Interface"))
                    continue;
                //Create an instance of NetworkAdapter class, and create performance counters for it.
                NetworkAdapter adapter = new NetworkAdapter(name);
                adapter.dlCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", name);
                adapter.ulCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name);
                this.adapters.Add(adapter); // Add it to ArrayList adapter  
            }

            Reset(0);
        }

        public string Name {
            get => networkAdapter?.Name;
        }

        public int Count {
            get => adapters.Count;
        }

        public long DownloadSpeed {
            get => networkAdapter.DownloadSpeed;
        }

        public long UploadSpeed {
            get => networkAdapter.UploadSpeed;
        }

        public double UploadSpeedKbps {
            get => networkAdapter.UploadSpeedKbps;
        }

        public double DownloadSpeedKbps {
            get => networkAdapter.DownloadSpeedKbps;
        }

        public string[] Names() {
            List<string> names = new List<string>();
            foreach (NetworkAdapter adapter in adapters) {
                names.Add(adapter.Name);
            }
            return names.ToArray();
        }

        public void Update() {
            if (networkAdapter != null) {
                networkAdapter.refresh();
            }
        }

        public void Reset(int index) {
            if (index < Count) {
                networkAdapter = adapters[index];
            }
        }

        public void Reset(string name) {
            foreach (NetworkAdapter adapter in adapters) {
                if (adapter.Name.Equals(name)) {
                    networkAdapter = adapter;
                    break;
                }
            }
        }

    }
}
