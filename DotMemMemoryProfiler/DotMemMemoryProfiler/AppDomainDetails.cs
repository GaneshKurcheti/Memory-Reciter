using CLRMD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMemMemoryProfiler
{
    public class AppDomainDetails
    {
        public string AppDomainName { get; set; }
        public string AppBase { get; set; }
        public string ConfigFile { get; set; }
        public ObservableCollection<ModuleDeatails> ModulesInAppDomain { get; set; }
        public ObservableCollection<ThreadDetails> ThreadsInAppDomain { get; set; }
    }

    public class Differences
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public int Size { get; set; }
        public bool PosOrNeg { get { return (Count > 0 ? true : false); } }
        public bool UserDefinedOrNot { get { return ((Type.StartsWith("System") || Type.StartsWith("Microsoft") || Type.StartsWith("Free")) ? false : true); } }
        public int Gen0ObjectsCount
        {
            get
            {
                int count = 0;
                foreach (var obj in objects)
                {
                    if (obj.Generation == 0)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        public int Gen2ObjectsCount
        {
            get
            {
                int count = 0;
                foreach (var obj in objects)
                {
                    if (obj.Generation == 2)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        public int Gen1ObjectsCount
        {
            get
            {
                int count = 0;
                foreach (var obj in objects)
                {
                    if (obj.Generation == 1)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        public ObservableCollection<HeapHelper.Object> objects { get; set; }
        public Differences()
        {
            objects = new ObservableCollection<HeapHelper.Object>();
        }

    }
}
