using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMemMemoryProfiler
{
   public class ThreadDetails
    {
      public  bool IsAlive { get; set; }
      public  bool IsUnstarted { get; set; }
      public  bool IsAborted { get; set; }
      public  int ThreadId { get; set; }
      public  int BlockingObject { get; set; }
      public  int ManagedLockCount { get; set; }
      public  string CurrentException { get; set; }
      public  bool IsAbortRequested { get; set; }
      public  bool IsSuspendingTheRuntime { get; set; }      
      public ObservableCollection<StackFrameDetails> StackFrameDetails { get; set; }
    }
}
