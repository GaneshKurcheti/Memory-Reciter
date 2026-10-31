using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DotMemMemoryProfiler
{
    static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);
    }
    static class ProcessExtensionClass
    {
        /// <summary>
        /// Extension method to check whether the Target application is 64 bit or 32 bit. 
        /// </summary>
        public static bool IsWin64Emulator(this Process process)
        {
            if ((Environment.OSVersion.Version.Major > 5)
                || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1)))
            {
                bool retVal;
                return NativeMethods.IsWow64Process(process.Handle, out retVal) && retVal;
            }
            return false;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable dt;
        string fileLocation = "EMPTY";
        DataRow dr;
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
        }
        /// <summary>
        ///  Window OnCloseEvent Handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (fileLocation != "EMPTY")
            {
                try
                {
                    File.Delete(fileLocation +@"\System.Data.SQLite.dll");
                    Directory.Delete(fileLocation + @"\x64");
                    Directory.Delete(fileLocation + @"\x32");
                }
                catch(Exception)
                {
                   
                }
            }
        }
        /// <summary>
        /// Window OnLoadEvent Handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dt = new DataTable("PROCESS");
            DataColumn dc1 = new DataColumn("PROCESS ID", typeof(int));
            DataColumn dc2 = new DataColumn("PROCESS NAME", typeof(string));
            DataColumn dc3 = new DataColumn("START TIME", typeof(string));
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dataGridForAllProcess.ItemsSource = dt.DefaultView;
            dataGridForAllProcess.Visibility = Visibility.Hidden;
            refreshButton.Visibility = Visibility.Hidden;
            cancelButton.Visibility = Visibility.Hidden;
            LoadingTextBox.Visibility = Visibility.Hidden;
            loaderForAttachToProcess.Visibility = Visibility.Hidden;
            loaderForRefresh.Visibility = Visibility.Hidden;
            backgroundRectangle.Visibility = Visibility.Hidden;
            textBackground.Visibility = Visibility.Hidden;
            //this.WindowState = System.Windows.WindowState.Maximized;
        }
        /// <summary>
        /// Creating Directory If not found in given path.
        /// </summary>
        /// <param name="path"></param>
        //private void CreateIfMissing(string path)
        //{
        //    Directory.CreateDirectory(Path.Combine(path, "x64"));
        //    Directory.CreateDirectory(Path.Combine(path, "x86"));
        //}
        /// <summary>
        /// Copying files from specified source to destination.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="fileName"></param>
        //private void CopyIfNotExits(string sourcePath, string destPath, string fileName)
        //{
        //    string srcPath = System.IO.Path.Combine(sourcePath, fileName);
        //    string targetPath = System.IO.Path.Combine(destPath, fileName);
        //    if (File.Exists(srcPath) && !File.Exists(targetPath))
        //    {
        //        File.Copy(srcPath, targetPath,true);
        //    }
        //}
        /// <summary>
        /// Creates table to return the details regarding the Running Processes(which runs as a Task).
        /// </summary>
        /// <returns></returns>
        private DataTable TableCreator()
        {
            DataTable tab = new DataTable();
            DataColumn dc1 = new DataColumn("PROCESS ID", typeof(int));
            DataColumn dc2 = new DataColumn("PROCESS NAME", typeof(string));
            DataColumn dc3 = new DataColumn("START TIME", typeof(string));
            tab.Columns.Add(dc1);
            tab.Columns.Add(dc2);
            tab.Columns.Add(dc3);
            // Enumerating all the Running Processes.
            Process[] procList = Process.GetProcesses();
            foreach (var process in procList)
            {
                try
                {
                    dr = tab.NewRow();
                    dr[0] = process.Id;
                    dr[1] = process.ProcessName;
                    try
                    {
                        dr[2] = process.StartTime;
                    }
                    catch
                    {
                        dr[2] = "not found";
                    }
                    tab.Rows.Add(dr);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return tab;
        }
        /// <summary>
        /// Attaching a Running Process Module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void attachToRunningProcessButton_Click(object sender, RoutedEventArgs e)
        {
            Task<DataTable> task = new Task<DataTable>(TableCreator);
            task.Start();
            LoadingTextBox.Visibility = Visibility.Visible;
            loaderForAttachToProcess.Visibility = Visibility.Visible;
            dt = await task; 
            browsingExeButton.Visibility = Visibility.Collapsed; 
            dataGridForAllProcess.Visibility = Visibility.Visible;
            dataGridForAllProcess.ItemsSource = dt.DefaultView;
            attachToRunningProcessButton.Visibility = Visibility.Collapsed;
            refreshButton.Visibility = Visibility.Visible;
            cancelButton.Visibility = Visibility.Visible;
            LoadingTextBox.Visibility = Visibility.Hidden;
            loaderForAttachToProcess.Visibility = Visibility.Hidden;
            backgroundRectangle.Visibility = Visibility.Visible;
            textBackground.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Browsing an .exe file Module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browsingExeButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            if (fd.FileName.EndsWith(".exe"))
            {
                var proc = Process.Start(fd.FileName);
                var platformFinder = GetBitSize(proc);
                try
                {
                    fileLocation = proc.MainModule.FileName;
                    //var fileName2 = Path.GetDirectoryName(fileLocation);
                    //CreateIfMissing(fileName2);
                    //var currentDirectory = Directory.GetCurrentDirectory();
                    //CopyIfNotExits(currentDirectory, fileName2, "System.Data.SQLite.dll");
                    //CopyIfNotExits(currentDirectory + "\\x64", fileName2 + "x64", "SQLite.Interop.dll");
                    //CopyIfNotExits(currentDirectory + "\\x86", fileName2 + "x86", "SQLite.Interop.dll");
                }
                catch
                {
                    MessageBox.Show("Sorry  could not locate main module of the specified file:( try other or check with io permissions");
                }
                if (!string.IsNullOrEmpty(platformFinder) && fileLocation != "EMPTY")
                {
                    //DirectoryInfo d = new DirectoryInfo(@".\Profiler" + platformFinder);
                    //FileInfo[] Files = d.GetFiles("*.dll");
                    //string location = Process.GetCurrentProcess().MainModule.FileName;
                    //location = location.Substring(0, location.LastIndexOf("\\"));
                    //foreach (FileInfo file in Files)
                    //{                      
                    //    CopyIfNotExits(file.DirectoryName, location, file.Name);                     
                    //}
                    var newWindow = new AnalyzingWindow(fd.FileName, platformFinder, proc.Id);
                    newWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("could not get data of that process sorry :(");
                }
            }
        }

        private static string GetBitSize(Process proc)
        {
            var platformFinder = "";
            try
            {
                platformFinder = proc.IsWin64Emulator() ? "32" : "64";
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode != 0x00000005)
                {
                    throw;
                }
            }

            return platformFinder;
        }

        /// <summary>
        /// This Event Handler is used to refresh the Running Processes. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingTextBox.Visibility = Visibility.Hidden;
            loaderForRefresh.Visibility = Visibility.Visible;
            dt.Rows.Clear();
            Task<DataTable> task = new Task<DataTable>(TableCreator);
            task.Start();
           
            dt = await task;
            dataGridForAllProcess.Visibility = Visibility.Visible;
            dataGridForAllProcess.ItemsSource = dt.DefaultView;
            loaderForRefresh.Visibility = Visibility.Hidden;
           
        }
        /// <summary>
        /// This Event Handler is used to go back to the home page(cancel button).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            dataGridForAllProcess.Visibility = Visibility.Hidden;
            browsingExeButton.Visibility = Visibility.Visible;
            attachToRunningProcessButton.Visibility = Visibility.Visible;
            refreshButton.Visibility = Visibility.Hidden;
            cancelButton.Visibility = Visibility.Hidden;
            backgroundRectangle.Visibility = Visibility.Hidden;
            textBackground.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// Selecting one of the Running process from the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            int processId = 0;
            if (dataGridForAllProcess.SelectedItems.Count > 0)
            {
                for (int i = 0; i < dataGridForAllProcess.SelectedItems.Count; i++)
                {
                    DataRowView selectedFile = (DataRowView)dataGridForAllProcess.SelectedItems[i];
                    processId = (int)selectedFile.Row.ItemArray[0];
                }
            }
            Process proc;
            try
            {
                proc = Process.GetProcessById(processId);
            }
            catch
            {
                MessageBox.Show("Process may be closed or currently out of scope");
                return;
            }
            var platformFinder = GetBitSize(proc);
            try
            {
                fileLocation = proc.MainModule.FileName;
                //var fileName2 = Path.GetDirectoryName(fileLocation);
                //CreateIfMissing(fileName2);
                //var currentDirectory = Directory.GetCurrentDirectory();
                //CopyIfNotExits(currentDirectory, fileName2, "System.Data.SQLite.dll");
                //CopyIfNotExits(currentDirectory + "\\x64", fileName2 + "x64", "SQLite.Interop.dll");
                //CopyIfNotExits(currentDirectory + "\\x86", fileName2 + "x86", "SQLite.Interop.dll");
            }
            catch
            {
                MessageBox.Show("Sorry could not locate main module of the specified file:( try other or checkek with io permissions");
            }
            if (!string.IsNullOrEmpty(platformFinder) && fileLocation != "EMPTY")
            {
                //var fileName2 = Path.GetDirectoryName(fileLocation);
                //DirectoryInfo d = new DirectoryInfo(@".\Profiler" + platformFinder);
                //FileInfo[] Files = d.GetFiles("*.dll");               
                //foreach (FileInfo file in Files)
                //{
                //    CopyIfNotExits(file.DirectoryName, fileName2, file.Name);
                //}
                AnalyzingWindow newWindow = new AnalyzingWindow(fileLocation, platformFinder, proc.Id);
                newWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("couldnot get data of that process sorry :( TRY other");
            }
        }
        /// <summary>
        /// On window size changed event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            myCanvas.Width = e.NewSize.Width;
            myCanvas.Height = e.NewSize.Height;
            double xChange = 1, yChange = 1;
            if (e.PreviousSize.Width != 0)
                xChange = (e.NewSize.Width / e.PreviousSize.Width);
            if (e.PreviousSize.Height != 0)
                yChange = (e.NewSize.Height / e.PreviousSize.Height);
            foreach (FrameworkElement fe in myCanvas.Children)
            {
                if (fe is TextBlock == false && fe is FontAwesome.WPF.ImageAwesome == false)
                {
                    fe.Height = fe.ActualHeight * yChange;
                    fe.Width = fe.ActualWidth * xChange;
                    Canvas.SetTop(fe, Canvas.GetTop(fe) * yChange);
                    Canvas.SetLeft(fe, Canvas.GetLeft(fe) * xChange);

                }
                else if (fe is TextBlock != false)
                {
                    fe.Width = fe.ActualWidth * xChange;
                    fe.Height = fe.ActualHeight * yChange;
                    if (fe.Width >= e.NewSize.Width)
                    {
                        fe.Width = fe.ActualWidth / xChange;
                        fe.Height = fe.ActualHeight / yChange;
                    }
                    Canvas.SetLeft(fe, Canvas.GetLeft(fe) * xChange);
                    Canvas.SetTop(fe, Canvas.GetTop(fe) * yChange);
                    if (fe as TextBlock != null)
                    {
                        value.FontSize = value.FontSize * xChange;
                    }
                }
                else if (fe is FontAwesome.WPF.ImageAwesome != false)
                {

                    Canvas.SetTop(fe, Canvas.GetTop(fe) * yChange);
                    Canvas.SetLeft(fe, Canvas.GetLeft(fe) * xChange);
                }
                TreeView treeView = fe as TreeView;
                if (treeView != null)
                {
                    treeView.FontSize = e.NewSize.Width / 100;
                }
                ListBox listView = fe as ListBox;
                if (listView != null)
                {
                    foreach (ListBoxItem listboxitem in listView.Items)
                    {
                        listboxitem.FontSize = e.NewSize.Width / 100;
                    }
                }
                Label testLabel = fe as Label;
                if (testLabel != null)
                {
                    testLabel.FontSize = testLabel.FontSize * xChange;
                }
                Button testButton = fe as Button;
                if (testButton != null)
                {
                    testButton.FontSize = testButton.FontSize * xChange;
                }
            }
        }

        
    }
}
