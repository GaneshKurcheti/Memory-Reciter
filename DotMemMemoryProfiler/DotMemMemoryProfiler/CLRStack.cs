using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotMemMemoryProfiler
{
    class CLRStack:IDisposable
    {
        private DataTarget dataTarget;
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
        private int processId;
        public CLRStack(string processName,int processId)
        {
            Process process = Process.GetProcessesByName(processName)[0];
            this.processId = processId;        
        }
        public  ObservableCollection<AppDomainDetails> GetData()
        {           
            ObservableCollection<AppDomainDetails> totalAppDomains = new ObservableCollection<AppDomainDetails>();
            //if (showCLRVersion) {// ClrVersion(); 
            //}
                EnsureProperDebugEngineIsLoaded();
                dataTarget = DataTarget.AttachToProcess(pid: processId, msecTimeout: 1000, attachFlag: AttachFlag.NonInvasive);
            //    while(true)
            //{
            //    try
            //    {
            //        ClrRuntime rt = dataTarget.ClrVersions[0].CreateRuntime();
            //        break;
            //    }
            //    catch
            //    {

            //    }
            //}
                ClrRuntime runtime = dataTarget.ClrVersions[0].CreateRuntime();
                foreach (var appDomain in runtime.AppDomains)
                {
                    AppDomainDetails appDomainDetails = new AppDomainDetails()
                    {
                        AppDomainName = appDomain.Name,
                        ConfigFile = appDomain.ConfigurationFile,
                        AppBase = appDomain.ApplicationBase
                    };
                    ObservableCollection<ModuleDeatails> modulesInsideAppdomain = new ObservableCollection<ModuleDeatails>();
                    var module = appDomain.Modules;
                    for (int i = 0; i < appDomain.Modules.Count; i++)
                    {
                        ModuleDeatails moduleDetails = new ModuleDeatails()
                        {
                            ModuleName = module[i].Name,
                            ModuleSize = (int)module[i].Size,
                            AssemblyId = (int)module[i].AssemblyId
                        };
                        modulesInsideAppdomain.Add(moduleDetails);
                    }
                    appDomainDetails.ModulesInAppDomain = modulesInsideAppdomain;
                    var thread = runtime.Threads;
                    ObservableCollection<ThreadDetails> threadsInsideAppdomain = new ObservableCollection<ThreadDetails>();
                    for (int i = 0; i < runtime.Threads.Count; i++)
                    {
                        ThreadDetails threadDetails = new ThreadDetails()
                        {
                            IsAlive = thread[i].IsAlive,
                            IsAborted = thread[i].IsAborted,
                            IsUnstarted = thread[i].IsUnstarted,
                            IsSuspendingTheRuntime = thread[i].IsSuspendingEE,
                            IsAbortRequested = thread[i].IsAbortRequested,
                            ThreadId = thread[i].ManagedThreadId,
                            ManagedLockCount = (int)thread[i].LockCount,
                            BlockingObject = thread[i].BlockingObjects.Count,

                        };
                        if (thread[i].CurrentException != null)
                        {
                            threadDetails.CurrentException = thread[i].CurrentException.ToString();
                        }
                        else { threadDetails.CurrentException = "no exception"; }
                        ObservableCollection<StackFrameDetails> stackFrameList = new ObservableCollection<StackFrameDetails>();
                        foreach (ClrStackFrame frame in thread[i].StackTrace)
                        {
                            StackFrameDetails stackFrame = new StackFrameDetails()
                            {
                                StackPointer = frame.StackPointer.ToString(),
                                InstructionPointer = frame.InstructionPointer.ToString(),
                                Frame = frame.ToString()
                            };
                            stackFrameList.Add(stackFrame);
                        }
                        threadDetails.StackFrameDetails = stackFrameList;
                        threadsInsideAppdomain.Add(threadDetails);
                    }
                    appDomainDetails.ThreadsInAppDomain = threadsInsideAppdomain;
                    totalAppDomains.Add(appDomainDetails);
                
            }
            dataTarget.Dispose();
            //ListOfAppDomainDetails list = new ListOfAppDomainDetails();
            //list.AppDomainDetails = totalAppDomains;

            return totalAppDomains;
          
        }

        public void Dispose()
        {
            dataTarget.Dispose();
        }

        //public bool ClrVersion()
        //{
        //    foreach (ClrInfo version in dataTarget.ClrVersions)
        //    {
        //        Console.WriteLine("CLR version: {0}", version.Version.ToString());                
        //        ModuleInfo dacInfo = version.DacInfo;
        //        Console.WriteLine("Filesize:  {0:X}", dacInfo.FileSize);
        //        Console.WriteLine("Timestamp: {0:X}", dacInfo.TimeStamp);
        //        Console.WriteLine("Dac File:  {0}", dacInfo.FileName);

        //        string dacLocation = version.LocalMatchingDac;
        //        if (!string.IsNullOrEmpty(dacLocation))
        //            Console.WriteLine("Local dac location: " + dacLocation);
        //    }
        //    return true;
        //}      
    }
}
