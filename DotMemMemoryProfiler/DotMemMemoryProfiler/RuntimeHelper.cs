using CLRMD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMemMemoryProfiler
{
    class RuntimeHelper
    {       
       public ObservableCollection<HeapHelper.HeapData> HeapDataRuntime;
       public ObservableCollection<AppDomainDetails> AppDomainDetailsRuntime;
        public RuntimeHelper()
        {
            HeapDataRuntime = new ObservableCollection<HeapHelper.HeapData>();
            AppDomainDetailsRuntime = new ObservableCollection<AppDomainDetails>();                
        }
    }
}
