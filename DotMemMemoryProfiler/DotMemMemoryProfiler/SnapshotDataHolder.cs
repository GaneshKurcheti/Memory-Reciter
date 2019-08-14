using CLRMD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DotMemMemoryProfiler
{
  
    public class SnapshotDataHolder
    {       

       public Dictionary<int, ObservableCollection<AppDomainDetails>> appDomaianDataSet;

       public Dictionary<int, ObservableCollection<HeapHelper.HeapData>> heapDataSet;    
        public SnapshotDataHolder()
        {
            appDomaianDataSet = new Dictionary<int, ObservableCollection<AppDomainDetails>>();
            heapDataSet = new Dictionary<int, ObservableCollection<HeapHelper.HeapData>>();
        }
    }
}
