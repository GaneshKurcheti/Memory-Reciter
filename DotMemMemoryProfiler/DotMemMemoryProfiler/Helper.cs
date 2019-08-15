using CLRMD;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
namespace DotMemMemoryProfiler
{
    class Helper : INotifyPropertyChanged
    {       
        string virtualMemoryUsage = "Not Found";
        string physicalMemoryUsage = "Not Found";
        string privateMemoryUsage = "Not Found";
        private string process;
        public DispatcherTimer _timer = new DispatcherTimer();
        public ChartValues<ObservableValue> DumpSizes { get; set; }
        public ChartValues<ObservableValue> DynamicValues { get; set; }
        public SeriesCollection SeriesCollection { get; set; }      

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Constructor with 0 arguments.
        /// </summary>
        public Helper()
        {
        }
        /// <summary>
        /// Constructor that takes process name as the argument.
        /// </summary>
        /// <param name="process"></param>
        public Helper(string process,int processId)
        {
            this.process = process;
            this.processId = processId.ToString();
            _timer.Interval = TimeSpan.FromMilliseconds(400);
            _timer.Tick += TimerOnTick;
            DynamicValues = new ChartValues<ObservableValue>
            {
                new ObservableValue(1),
            };
            DumpSizes = new ChartValues<ObservableValue>
            {
                new ObservableValue(1),
            };

            SeriesCollection = new SeriesCollection
            {
             new LineSeries
            {
                Title = "MemoryUsage",
                Name = "series_1",
                Foreground=Brushes.White,
                Values=DynamicValues
            }
            };
        }
        /// <summary>
        /// Timer on tick event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        int ProcessEndIdentifier=0;
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            Process procList=null;
         
            if (ProcessEndIdentifier == 0)
            {
                try
                {
                    procList = Process.GetProcessById(Convert.ToInt32(processId));
                }
                catch
                {
                    ProcessEndIdentifier = 1;
                    MessageBox.Show("Target process terminated unexpetedly.");                    
                }
            }
            try
            {
                if (procList != null)
                {
                    VirtualMemoryUsage = procList.VirtualMemorySize64 / 1024 + "kb";
                    PhysicalMemoryUsage = procList.WorkingSet64 / 1024 + "kb";
                    PrivateMemoryUsage = procList.PrivateMemorySize64 / 1024 + "kb";
                    PagedMemorySize = procList.PagedMemorySize64 + "kb";
                    try
                    {
                        TotalProcessorTime = procList.TotalProcessorTime + "seconds";
                    }
                    catch
                    {
                    }
                    ProcessId = procList.Id.ToString();
                    ProcessName = process;
                    Threads = procList.Threads;
                    Task.Factory.StartNew(() =>
                    {
                        PerformanceCounter PC = new PerformanceCounter();
                        PC.CategoryName = "Process";
                        PC.CounterName = "Working Set - Private";
                        PC.InstanceName = process;
                        long processSize = Convert.ToInt64(PC.NextValue()) / (int)(1024);
                        PC.Close();
                        PC.Dispose();
                        if (DynamicValues.Count < 50)
                        {
                            DynamicValues.Add(new ObservableValue(processSize));
                        }
                        else
                        {
                            DynamicValues.RemoveAt(0);
                            DynamicValues.Add(new ObservableValue(processSize));
                        }
                    });
                }
            }
            catch
            {

            }
           
        }/// <summary>
        /// Properties to acheive data binding to the ui elements.
        /// </summary>
        public string VirtualMemoryUsage
        {
            get
            {
                return virtualMemoryUsage;
            }
            set
            {
                virtualMemoryUsage = value;
                OnPropertyChanged("VirtualMemoryUsage");
            }
        }
        public string PhysicalMemoryUsage
        {
            get
            {
                return physicalMemoryUsage;
            }
            set
            {
                physicalMemoryUsage = value;
                OnPropertyChanged("PhysicalMemoryUsage");
            }
        }
        public string PrivateMemoryUsage
        {
            get
            {
                return privateMemoryUsage;
            }
            set
            {
                privateMemoryUsage = value;
                OnPropertyChanged("PrivateMemoryUsage");
            }
        }
        private string pagedMemorySize;

        public string PagedMemorySize
        {
            get { return pagedMemorySize; }
            set { pagedMemorySize = value; OnPropertyChanged("PagedMemorySize"); }
        }
        private string totalProcessorTime;
        public string TotalProcessorTime
        {
            get { return totalProcessorTime; }
            set
            {
                totalProcessorTime = value;
                OnPropertyChanged("TotalProcessorTime");
            }
        }
        private ProcessThreadCollection threads;

        public ProcessThreadCollection Threads
        {
            get { return threads; }
            set { threads = value; OnPropertyChanged("Threads"); }
        }
        private string processId;

        public string ProcessId
        {
            get { return processId; }
            set
            {
                processId = value;
                OnPropertyChanged("ProcessId");
            }
        }
        string procName;
        public string ProcessName
        {
            get
            {
                return procName;
            }
            set
            {
                procName = value;
                OnPropertyChanged("ProcessName");
            }
            
        }
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
