using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CLRMD
{
    public class CLRHeap : IDisposable
    {
        private DataTarget dataTarget;
        public Dictionary<ClrType, ObservableCollection<HeapHelper.Object>> Objects { get; internal set; }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, int dwFlags);
        private const int LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100;

        private static void EnsureProperDebugEngineIsLoaded()
        {
            var sysdir = Environment.GetFolderPath(Environment.SpecialFolder.System);
            var res = LoadLibraryEx(Path.Combine(sysdir, "dbgeng.dll"), IntPtr.Zero, LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR);
            if (res == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        int processId;
        public CLRHeap(string processName,int processId)
        {
            this.processId = processId;
            Process process = Process.GetProcessesByName(processName)[0];           
            Objects = new Dictionary<ClrType, ObservableCollection<HeapHelper.Object>>();
        }

        public ObservableCollection<HeapHelper.HeapData> ObservableCollection()
        {
            EnsureProperDebugEngineIsLoaded();
            dataTarget = DataTarget.AttachToProcess(pid: processId, msecTimeout: 1000, attachFlag: AttachFlag.NonInvasive);
            Thread.Sleep(1000);
            ClrRuntime runtime = dataTarget.ClrVersions[0].CreateRuntime();
            HeapHelper.Object heapObj;
            foreach (var segment in runtime.Heap.Segments)
            {
                HeapHelper.Segment segmentType;
                if (segment.IsEphemeral)
                    segmentType = HeapHelper.Segment.Ephemeral;
                else if (segment.IsLarge)
                    segmentType = HeapHelper.Segment.Large;
                else
                    segmentType = HeapHelper.Segment.Segment;

                for (ulong obj = segment.FirstObject; obj != 0; obj = segment.NextObject(obj))
                {
                    var type = runtime.Heap.GetObjectType(obj);
                    if (type == null)
                        continue;

                    heapObj = new HeapHelper.Object(obj, type, segmentType, segment.GetGeneration(obj));

                    if (Objects.ContainsKey(type))
                        Objects[type].Add(heapObj);
                    else
                    {
                        Objects.Add(type, new ObservableCollection<HeapHelper.Object>());
                        Objects[type].Add(heapObj);
                    }
                }
            }
            var ObjectsCollection = new ObservableCollection<HeapHelper.HeapData>();
            foreach (var type in Objects)
            {
                ObjectsCollection.Add(new HeapHelper.HeapData(type.Key) { objects = type.Value });
            }
            dataTarget.Dispose();
            return ObjectsCollection;
        }

    
        public void Dispose()
        {
            dataTarget.Dispose();
            Objects = null;
        }
    }
}
