using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DotMemMemoryProfiler
{
    class DataBackUp
    {
        
        public string processName;
        public int processId;
        public string Date;
        public string Time;
        public List<string> timestamp;
        public SnapshotDataHolder snapshotData;
        public DataBackUp()
        {
            snapshotData = new SnapshotDataHolder();
            timestamp = new List<string>();
        }
    }
}
