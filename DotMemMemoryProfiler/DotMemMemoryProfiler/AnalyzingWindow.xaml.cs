using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using CLRMD;
using Newtonsoft.Json;
namespace DotMemMemoryProfiler
{
    /// <summary>
    /// Interaction logic for AnalyzingWindow32.xaml
    /// </summary>
    public partial class AnalyzingWindow : Window
    {
        static string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\ProfilerDataFolder";
        public string connection = @"Data Source=" + path + @"\ProfilerData.sqlite;Version=3; FailIfMissing=True; Foreign Keys=True;";
        private string currentMethod;
        public string processName;
        int processId;
        int clickcounter = 0;
        private string totalPath;
        string processEnvironment;
        private int snapshotCounter = 0;
        public string profilerDataConnection;
        //Dictionary<int, Dictionary<string, ObjectDetails>> snapshotDataHolder = new Dictionary<int, Dictionary<string, ObjectDetails>>();
        Dictionary<int, ObservableCollection<AppDomainDetails>> appDomaianDataHolder = new Dictionary<int, ObservableCollection<AppDomainDetails>>();
        Dictionary<int, ObservableCollection<HeapHelper.HeapData>> heapDataHolder = new Dictionary<int, ObservableCollection<HeapHelper.HeapData>>();
        AssemblyExaminer assemblyDataHolder = new AssemblyExaminer();
        /// <summary>
        /// Constructor that takes 2 arguments (is used for 32 bit processes).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="bit"></param>
        Stopwatch stopwatch;
        public AnalyzingWindow(string message, string bit, int processId)
        {
            stopwatch = Stopwatch.StartNew();
            InitializeComponent();
            processName = message;
            this.processId = processId;
            Helper x = new Helper(processName, this.processId);
            x._timer.Start();
            this.DataContext = x;
            clickcounter = 1;
            totalPath = Process.GetProcessesByName(processName)[0].MainModule.FileName;
            processEnvironment = bit;
            Closing += AnalyzingWindow_Closing;
            try
            {
                Directory.CreateDirectory(path);
                File.Create(path + @"\ProfilerData.sqlite");
            }
            catch
            {
                MessageBox.Show("Database is being accessed by some other program or may be other instance of same program please stop the process and cancel the Messgae box to get reliable data ");
            }


        }
        /// <summary>
        /// Constructor that takes 3 arguments (is used for 64 bit processes).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exePath"></param> 
        /// <param name="bit"></param> 
        public AnalyzingWindow(string message, string exePath, string bit, int processId)
        {
            stopwatch = Stopwatch.StartNew();
            InitializeComponent();
            processName = message;
            this.processId = processId;
            Helper x = new Helper(processName, this.processId);
            x._timer.Start();
            this.DataContext = x;
            clickcounter = 1;
            processEnvironment = bit;
            try
            {
                Directory.CreateDirectory(path);
                File.Create(path + @"\ProfilerData.sqlite");
            }
            catch
            {
                MessageBox.Show("Database is being accessed by some other program or may be other instance of same program please stop the process and cancel the Messgae box ");
            }

            totalPath = exePath;
        }
        /// <summary>
        /// On window load event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            Loadlabel.Visibility = Visibility.Hidden;
            loader.Visibility = Visibility.Hidden;
            LoadingLabel.Visibility = Visibility.Hidden;
            listBoxForStack.Visibility = Visibility.Hidden;
            listBoxForInnerMethods.Visibility = Visibility.Hidden;
            labelForAssemblyName.Visibility = Visibility.Hidden;
            AssemblyItemLabel.Visibility = Visibility.Hidden;
            labelForSnapshotData.Visibility = Visibility.Hidden;
            heapObjectsHeadingLabel.Visibility = Visibility.Hidden;
            InnerMethodsHeadingLabel.Visibility = Visibility.Hidden;
            MethodStackHeadingLabel.Visibility = Visibility.Hidden;
            HeapStringHeadingLabel.Visibility = Visibility.Hidden;
            namespaceNameLabel.Visibility = Visibility.Hidden;
            itemNameLabel.Visibility = Visibility.Hidden;
            treeView.Visibility = Visibility.Hidden;
            listBoxForSnapshotTracking.Visibility = Visibility.Hidden;
            ProcessAssemblieHeadingLabel.Visibility = Visibility.Hidden;
            listBoxForCode.Visibility = Visibility.Hidden;
            textBoxForCode.Visibility = Visibility.Hidden;
            textBoxForIlcode.Visibility = Visibility.Hidden;
            dataGridForCompareHeapObjects.Visibility = Visibility.Hidden;
            comboBoxForA.Visibility = Visibility.Hidden;
            comboBoxForB.Visibility = Visibility.Hidden;
            dataGridForHeapObjects.Visibility = Visibility.Hidden;
            buttonToCompareSnapshots.Visibility = Visibility.Hidden;
            treeViewForAssemblyDetails.Visibility = Visibility.Hidden;
            listBoxForLocalVariables.Visibility = Visibility.Hidden;
            dataGridForStackDetails.Visibility = Visibility.Hidden;
            textBoxToSearchHeap.Visibility = Visibility.Hidden;
            startProfilingButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF9CAFEA"));
            appDomaianObjectsSwappperButton.Visibility = Visibility.Hidden;
            openDetailsInNewWindowButton.Visibility = Visibility.Hidden;
            this.WindowState = System.Windows.WindowState.Maximized;
        }
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
            Directory.Delete(target_dir, false);

        }
        /// <summary>
        /// Module for start Profiling.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public int assemblyDataGetter()
        {
            AssemblyExaminer assemblyData = new AssemblyExaminer();
            string tempPath = null;
            if (totalPath != null)
            {
                tempPath = totalPath;
            }
            else
            {
                try
                {
                    tempPath = Process.GetProcessesByName(processName)[0].MainModule.FileName;
                }
                catch
                {
                    MessageBox.Show("The process location is unidentified since the process is stopped");
                }
            }
            List<string> pathList = new List<string>();
            pathList.Add(tempPath);
            assemblyData.AssemblyReader(pathList);
            assemblyDataHolder = assemblyData;
            return 1;
        }
        private async void profilerButton_Click(object sender, RoutedEventArgs e)
        {
            closeCurrentSessionButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            pdbInfoButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            startProfilingButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            profilerButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            codeGenerationButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            exitButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            profilerButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF9CAFEA"));
            listBoxForCode.Visibility = Visibility.Hidden;
            textBoxForCode.Visibility = Visibility.Hidden;
            listBoxForSnapshotTracking.Visibility = Visibility.Hidden;
            dataGridForHeapObjects.Visibility = Visibility.Hidden;
            buttonToCompareSnapshots.Visibility = Visibility.Hidden;
            comboBoxForA.Visibility = Visibility.Hidden;
            comboBoxForB.Visibility = Visibility.Hidden;
            textBoxToSearchHeap.Visibility = Visibility.Hidden;
            labelForSnapshotData.Visibility = Visibility.Hidden;
            dataGridForStackDetails.Visibility = Visibility.Hidden;
            allHeapObjectsButton.Visibility = Visibility.Hidden;
            treeViewForAssemblyDetails.Visibility = Visibility.Hidden;
            appDomaianObjectsSwappperButton.Visibility = Visibility.Hidden;
            openDetailsInNewWindowButton.Visibility = Visibility.Hidden;
            listBoxForLocalVariables.Visibility = Visibility.Hidden;
            dataGridForCompareHeapObjects.Visibility = Visibility.Hidden;
            if (clickcounter == 1)
            {
                clickcounter++;
                if (!processName.Contains("\\"))
                {
                    loader.Visibility = Visibility.Visible;
                    LoadingLabel.Visibility = Visibility.Visible;
                    Task<int> assemblyDataGetterTask = new Task<int>(assemblyDataGetter);
                    assemblyDataGetterTask.Start();
                    int i = await assemblyDataGetterTask;
                    LoadingLabel.Visibility = Visibility.Collapsed;
                    loader.Visibility = Visibility.Hidden;
                    List<string> distinctNamespaces = assemblyDataHolder.namespaceTable.NamespaceRecords.Select(e1 => e1.NamespaceName).Distinct().ToList();
                    foreach (var namespaceName in distinctNamespaces)
                    {
                        TreeViewItem item = new TreeViewItem() { Header = namespaceName };
                        List<string> distinctItems = assemblyDataHolder.namespaceTable.NamespaceRecords.Where(e1 => e1.NamespaceName == namespaceName).Select(e1 => e1.Item).Distinct().ToList();
                        foreach (var distinctItem in distinctItems)
                        {
                            TreeViewItem nesteditem = new TreeViewItem() { Header = distinctItem };
                            nesteditem.MouseLeave += Itm_MouseLeave;
                            List<string> distinctInternalItems = assemblyDataHolder.itemExternalInfoTable.ItemExternalDetails.Where(e1 => (e1.ItemName == distinctItem)).Where(e1 => (e1.InternalItemType.Contains("Method"))).Select(e1 => e1.InternalItems).Distinct().ToList();
                            foreach (var distinctInternalItem in distinctInternalItems)
                            {
                                TreeViewItem nesteditem2 = new TreeViewItem() { Header = distinctInternalItem };
                                nesteditem2.MouseDoubleClick += Nesteditem2_MouseDoubleClick;
                                nesteditem.Items.Add(nesteditem2);
                                nesteditem2.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7")); ;
                            }
                            item.Items.Add(nesteditem);
                            nesteditem.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
                            nesteditem.MouseDoubleClick += Nesteditem_MouseDoubleClick1;
                        }
                        treeView.Items.Add(item);
                        item.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
                        treeView.FontSize = myCanvas.ActualWidth / 100;
                        ProcessAssemblieHeadingLabel.Visibility = Visibility.Visible;
                    }
                }
            }
            treeView.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Double click event handler on the tree view (on the items other than classes).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nesteditem_MouseDoubleClick1(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                if (item.Items.Count == 0)
                {
                    try
                    {
                        string AssemblyItemLabelData = item.Header.ToString();
                        string labelForAssemblyNameData = AssemblyItemLabelData.Substring(0, AssemblyItemLabelData.LastIndexOf("."));
                        AssemblyItemLabel.Content = AssemblyItemLabelData;
                        labelForSnapshotData.Visibility = Visibility.Hidden;
                        labelForAssemblyName.Content = labelForAssemblyNameData;
                        labelForAssemblyName.Visibility = Visibility.Visible;
                        AssemblyItemLabel.Visibility = Visibility.Visible;
                        listBoxForStack.Visibility = Visibility.Hidden;
                        heapObjectsHeadingLabel.Visibility = Visibility.Hidden;
                        InnerMethodsHeadingLabel.Visibility = Visibility.Hidden;
                        MethodStackHeadingLabel.Visibility = Visibility.Hidden;
                        HeapStringHeadingLabel.Visibility = Visibility.Hidden;
                        listBoxForInnerMethods.Visibility = Visibility.Hidden;
                        appDomaianObjectsSwappperButton.Visibility = Visibility.Hidden;
                        openDetailsInNewWindowButton.Visibility = Visibility.Hidden;
                        textBoxForIlcode.Visibility = Visibility.Visible;
                        dataGridForCompareHeapObjects.Visibility = Visibility.Hidden;
                    }
                    catch (Exception E)
                    {

                    }
                }
            }
        }
        /// <summary>
        /// Double click event handler on vthe methods displayed in the treeview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nesteditem2_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            listBoxForStack.Items.Clear();
            dataGridForCompareHeapObjects.Visibility = Visibility.Hidden;
            listBoxForInnerMethods.Items.Clear();
            listBoxForStack.Visibility = Visibility.Hidden;
            listBoxForInnerMethods.Visibility = Visibility.Visible;
            labelForAssemblyName.Visibility = Visibility.Visible;
            AssemblyItemLabel.Visibility = Visibility.Visible;
            heapObjectsHeadingLabel.Visibility = Visibility.Hidden;
            InnerMethodsHeadingLabel.Visibility = Visibility.Visible;
            MethodStackHeadingLabel.Visibility = Visibility.Hidden;
            namespaceNameLabel.Visibility = Visibility.Visible;
            itemNameLabel.Visibility = Visibility.Visible;
            HeapStringHeadingLabel.Visibility = Visibility.Hidden;
            TreeViewItem tempItem = sender as TreeViewItem;
            string AssemblyItemLabelData = tempItem.Header.ToString().Substring(0, tempItem.Header.ToString().LastIndexOf("."));
            string labelForAssemblyNameData = AssemblyItemLabelData.Substring(0, AssemblyItemLabelData.LastIndexOf("."));
            AssemblyItemLabel.Content = AssemblyItemLabelData;
            labelForAssemblyName.Content = labelForAssemblyNameData;
            if (tempItem != null)
            {
                currentMethod = tempItem.Header.ToString();
                string method = tempItem.Header.ToString();
                List<string> methodsofMethod = new List<string>();
                methodsofMethod = assemblyDataHolder.methodMethodsTable.MethodInternalMethods.Where(e1 => e1.MethodName == method).Select(e1 => e1.InternalMethods).ToList();
                foreach (var methodofMethod in methodsofMethod)
                {
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = methodofMethod;
                    itm.FontSize = myCanvas.ActualWidth / 100;
                    itm.Background = Brushes.Transparent;
                    itm.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
                    listBoxForInnerMethods.Items.Add(itm);
                }
                List<string> variablesOfMethod = new List<string>();
                variablesOfMethod = (from s in assemblyDataHolder.methodVariablesTable.MethodVariables
                                     where s.MethodName == method
                                     orderby s.Index
                                     select s.VariableType).ToList();
                int enumerator = 0;
                foreach (var variableOfMethod in variablesOfMethod)
                {
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = (variableOfMethod + "(" + enumerator + ")").ToString();
                    enumerator++;
                    itm.MouseEnter += Itm_MouseEnterListBoxForStack;
                    itm.MouseLeave += Itm_MouseLeave;
                    itm.FontSize = myCanvas.ActualWidth / 100;
                    itm.Background = Brushes.Transparent;
                    itm.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
                    listBoxForStack.Items.Add(itm);
                    listBoxForStack.Visibility = Visibility.Visible;
                    MethodStackHeadingLabel.Visibility = Visibility.Visible;
                }
                string methodIlCode = assemblyDataHolder.methodIlCodeTable.MethodIlCodes.Where(e1 => e1.MethodName == method).Select(e1 => e1.MethodIl).ToList().First();
                textBoxForIlcode.Text = methodIlCode.ToString();
                textBoxForIlcode.Visibility = Visibility.Visible;
            }
        }
        private void Itm_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MyPopup.IsOpen = false;
        }
        /// <summary>
        /// Mouse enter event handler for the listbox that displays the heap objects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dataGridForHeapObjects.SelectedItems.Count > 0)
            {
                for (int i = 0; i < dataGridForHeapObjects.SelectedItems.Count; i++)
                {
                    DataRowView selectedFile = (DataRowView)dataGridForHeapObjects.SelectedItems[i];
                    processName = Convert.ToString(selectedFile.Row.ItemArray[0]);
                }
            }
            DataGrid dg = sender as DataGrid;
            if (dg == null)
                return;
            if (dg.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            else
                dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }
        /// <summary>
        /// Event Handler that activates the popup window on mouse enter on the  the listboxof stack elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Itm_MouseEnterListBoxForStack(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item != null)
            {
                string temp = item.Content.ToString();
                temp = temp.Substring(temp.LastIndexOf("(") + 1, (temp.Length - (temp.LastIndexOf('(') + 2)));
                List<MethodVariables> methodVariables = assemblyDataHolder.methodVariablesTable.MethodVariables.Where(e1 => e1.Index == temp).ToList();
                string text = "";
                foreach (var variable in methodVariables)
                {
                    text = text + "Variable index : " + variable.Index + "\nType of variable : " + variable.VariableType + "\nType  : " + variable.LocalOrReference + "\nValue  : " + variable.Value + "\nMethod Name : " + currentMethod;
                }
                textBlock.Text = text;
                MyPopup.Placement = PlacementMode.MousePoint;
                MyPopup.IsOpen = true;
            }
        }
        /// <summary>
        /// PDB files Module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pdbInfoButton_Click(object sender, RoutedEventArgs e)
        {
            closeCurrentSessionButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            pdbInfoButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            startProfilingButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            profilerButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            codeGenerationButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            exitButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            pdbInfoButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF9CAFEA"));
            loader.Visibility = Visibility.Hidden;
            LoadingLabel.Visibility = Visibility.Hidden;
            textBoxToSearchHeap.Visibility = Visibility.Hidden;
            listBoxForStack.Visibility = Visibility.Hidden;
            listBoxForInnerMethods.Visibility = Visibility.Hidden;
            labelForAssemblyName.Visibility = Visibility.Hidden;
            AssemblyItemLabel.Visibility = Visibility.Hidden;
            heapObjectsHeadingLabel.Visibility = Visibility.Hidden;
            InnerMethodsHeadingLabel.Visibility = Visibility.Hidden;
            MethodStackHeadingLabel.Visibility = Visibility.Hidden;
            HeapStringHeadingLabel.Visibility = Visibility.Hidden;
            namespaceNameLabel.Visibility = Visibility.Hidden;
            dataGridForCompareHeapObjects.Visibility = Visibility.Hidden;
            itemNameLabel.Visibility = Visibility.Hidden;
            treeView.Visibility = Visibility.Hidden;
            labelForSnapshotData.Visibility = Visibility.Hidden;
            listBoxForSnapshotTracking.Visibility = Visibility.Hidden;
            ProcessAssemblieHeadingLabel.Visibility = Visibility.Hidden;
            listBoxForCode.Visibility = Visibility.Hidden;
            textBoxForCode.Visibility = Visibility.Hidden;
            textBoxForIlcode.Visibility = Visibility.Hidden;
            comboBoxForA.Visibility = Visibility.Hidden;
            comboBoxForB.Visibility = Visibility.Hidden;
            dataGridForHeapObjects.Visibility = Visibility.Hidden;
            buttonToCompareSnapshots.Visibility = Visibility.Hidden;
            allHeapObjectsButton.Content = "BROWSE A PDB";
            allHeapObjectsButton.Visibility = Visibility.Visible;
            treeViewForAssemblyDetails.Visibility = Visibility.Hidden;
            listBoxForLocalVariables.Visibility = Visibility.Hidden;
            dataGridForStackDetails.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// Generate Code Module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        int clickonCodeGeneration = 1;
        private void codeGenerationButton_Click(object sender, RoutedEventArgs e)
        {
            closeCurrentSessionButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            pdbInfoButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            startProfilingButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            profilerButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            codeGenerationButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            exitButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            codeGenerationButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF9CAFEA"));
            allHeapObjectsButton.Visibility = Visibility.Hidden;
            loader.Visibility = Visibility.Visible;
            LoadingLabel.Visibility = Visibility.Visible;
            listBoxForStack.Visibility = Visibility.Hidden;
            listBoxForInnerMethods.Visibility = Visibility.Hidden;
            labelForAssemblyName.Visibility = Visibility.Hidden;
            dataGridForCompareHeapObjects.Visibility = Visibility.Hidden;
            AssemblyItemLabel.Visibility = Visibility.Hidden;
            heapObjectsHeadingLabel.Visibility = Visibility.Hidden;
            InnerMethodsHeadingLabel.Visibility = Visibility.Hidden;
            textBoxToSearchHeap.Visibility = Visibility.Hidden;
            MethodStackHeadingLabel.Visibility = Visibility.Hidden;
            HeapStringHeadingLabel.Visibility = Visibility.Hidden;
            namespaceNameLabel.Visibility = Visibility.Hidden;
            itemNameLabel.Visibility = Visibility.Hidden;
            labelForSnapshotData.Visibility = Visibility.Hidden;
            treeView.Visibility = Visibility.Hidden;
            listBoxForSnapshotTracking.Visibility = Visibility.Hidden;
            ProcessAssemblieHeadingLabel.Visibility = Visibility.Hidden;
            listBoxForCode.Visibility = Visibility.Hidden;
            textBoxForCode.Visibility = Visibility.Hidden;
            textBoxForIlcode.Visibility = Visibility.Hidden;
            comboBoxForA.Visibility = Visibility.Hidden;
            comboBoxForB.Visibility = Visibility.Hidden;
            buttonToCompareSnapshots.Visibility = Visibility.Hidden;
            appDomaianObjectsSwappperButton.Visibility = Visibility.Hidden;
            openDetailsInNewWindowButton.Visibility = Visibility.Hidden;
            dataGridForHeapObjects.Visibility = Visibility.Hidden;
            treeViewForAssemblyDetails.Visibility = Visibility.Hidden;
            listBoxForLocalVariables.Visibility = Visibility.Hidden;
            allHeapObjectsButton.Content = "OPEN IN VS";
            allHeapObjectsButton.Visibility = Visibility.Visible;
            dataGridForStackDetails.Visibility = Visibility.Hidden;
            if (clickonCodeGeneration == 1)
            {
                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\Decompile"))
                {
                    try
                    {
                        Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\Decompile", true);
                    }
                    catch (Exception ex)
                    {

                    }

                }
                if (codeGenerationButton.Content.ToString() == "GENERATE CODE")
                {
                    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\Decompile"))
                    {
                        try
                        {
                            Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\Decompile");
                        }
                        catch (Exception excep)
                        {

                        }
                    }
                    ProcessStartInfo startInfo = new ProcessStartInfo(@".\Decompiler\Decompiler.exe");
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    string loc;
                    try
                    {
                        Process proc = Process.GetProcessesByName(processName)[0];
                        loc = proc.MainModule.FileName;
                    }
                    catch
                    {
                        loc = totalPath;
                    }
                    startInfo.Arguments = "\"" + loc + "\"";
                    Process ptemp = Process.Start(startInfo);
                    ptemp.WaitForExit();
                    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\Decompile"))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\Decompile");
                        FileInfo[] info = dirInfo.GetFiles("*.cs", SearchOption.AllDirectories);
                        foreach (FileInfo f in info)
                        {
                            if (!f.Name.ToString().StartsWith("TemporaryGeneratedFile"))
                            {
                                ListBoxItem item = new ListBoxItem();
                                item.Content = f.Name;
                                item.Tag = f.FullName;
                                item.FontSize = 16;
                                item.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
                                item.MouseDoubleClick += listItemForCode_MouseDoubleClick;
                                item.Background = Brushes.Transparent;
                                listBoxForCode.Items.Add(item);
                            }
                        }

                    }
                }
                if (codeGenerationButton.Content.ToString() == "Open Project")
                {

                }
                clickonCodeGeneration++;
            }
            listBoxForCode.Visibility = Visibility.Visible;
            textBoxForCode.Visibility = Visibility.Visible;
            loader.Visibility = Visibility.Hidden;
            LoadingLabel.Visibility = Visibility.Hidden;
        }
        private void listItemForCode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            if (item != null)
            {
                string fileLocation = item.Tag.ToString();
                if (File.Exists(fileLocation))
                {
                    textBoxForCode.Text = File.ReadAllText(fileLocation);
                }
            }
        }
        //private int dataSaveToFile(RuntimeHelper dataObject)
        //{        
        //    using (var fs = File.Open(runtimeDirectory+"\\Snapshot"+snapshotCounter, FileMode.Create))
        //    using (var sw = new StreamWriter(fs))
        //    using (var jw = new JsonTextWriter(sw))
        //    {
        //        var serializer = new JsonSerializer();
        //        serializer.Serialize(jw, dataObject);
        //    }
        //    return 1;
        //}
        /// <summary>
        /// Event handler calls the function that enumerates the heap of the process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>   

        private async void AllHeapObjectsButton_Click(object sender, RoutedEventArgs e)
        {
            Button buttonClicked = sender as Button;
            if (buttonClicked != null)
            {
                if (buttonClicked.Content.ToString() == "TAKE SNAPSHOT")
                {
                    allHeapObjectsButton.Visibility = Visibility.Hidden;
                    var elapsedTime = stopwatch.ElapsedMilliseconds;
                    snapshotCounter++;
                    loader.Visibility = Visibility.Visible;
                    Loadlabel.Visibility = Visibility.Visible;
                    RuntimeHelper helperData = new RuntimeHelper();
                    Loadlabel.Content = "Getting Into Heap \n       Please wait";
                    CLRStack stackDetails = new CLRStack(processName, processId);
                    Task<ObservableCollection<AppDomainDetails>> temp = new Task<ObservableCollection<AppDomainDetails>>(stackDetails.GetData);
                    temp.Start();
                    appDomaianDataHolder[snapshotCounter] = await temp;
                    CLRHeap heapDetails = new CLRHeap(processName, processId);
                    Task<ObservableCollection<HeapHelper.HeapData>> heapDataGetter = new Task<ObservableCollection<HeapHelper.HeapData>>(heapDetails.ObservableCollection);
                    heapDataGetter.Start();
                    heapDataHolder[snapshotCounter] = await heapDataGetter;
                    loader.Visibility = Visibility.Hidden;
                    helperData.AppDomainDetailsRuntime = appDomaianDataHolder[snapshotCounter];
                    helperData.HeapDataRuntime = heapDataHolder[snapshotCounter];
                    //Task<int> saveDataToFile = new Task<int>(()=> { return dataSaveToFile(helperData);});
                    //saveDataToFile.Start();
                    allHeapObjectsButton.Visibility = Visibility.Visible;
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = "Snapshot#" + snapshotCounter;
                    itm.Background = Brushes.Transparent;
                    itm.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
                    TimeSpan t = TimeSpan.FromMilliseconds(elapsedTime);
                    string readableTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
                    itm.Tag = readableTime;
                    itm.MouseEnter += Itm_MouseEnterListBoxForSnapshotTracking;
                    itm.MouseDoubleClick += SnapshotTrackingItm_MouseDoubleClick;
                    itm.MouseLeave += Itm_MouseLeave;
                    itm.FontSize = myCanvas.ActualWidth / 100;
                    listBoxForSnapshotTracking.Items.Insert(0, itm);
                    SnapshotTrackingItm_MouseDoubleClick(itm, null);
                    appDomaianObjectsSwappperButton.Visibility = Visibility.Visible;
                    openDetailsInNewWindowButton.Visibility = Visibility.Visible;
                    Loadlabel.Visibility = Visibility.Hidden;
                    ComboBoxItem cbItemA = new ComboBoxItem();
                    cbItemA.Content = itm.Content;
                    cbItemA.Tag = snapshotCounter;
                    ComboBoxItem cbItemB = new ComboBoxItem();
                    cbItemB.Content = itm.Content;
                    cbItemB.Tag = snapshotCounter;
                    comboBoxForA.Items.Insert(0, cbItemA);
                    comboBoxForB.Items.Insert(0, cbItemB);
                    listBoxForSnapshotTracking.Visibility = Visibility.Visible;
                    if (snapshotCounter > 1)
                    {
                        comboBoxForA.Visibility = Visibility.Visible;
                        comboBoxForB.Visibility = Visibility.Visible;
                        buttonToCompareSnapshots.Visibility = Visibility.Visible;
                    }
                }
                else if (buttonClicked.Content.ToString() == "BROWSE A PDB")
                {
                    allHeapObjectsButton.Visibility = Visibility.Hidden;
                    Task<int> task = new Task<int>(pdbReader);
                    task.Start();
                    int i = await task;
                    if (File.Exists("./PDB.sqlite"))
                    {
                        try
                        {
                            using (FileStream sourceStream = File.Open("./PDB.sqlite", FileMode.Open))
                            {
                                using (FileStream destinationStream = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\ProfilerDataFolder\PDB.sqlite"))
                                {
                                    await sourceStream.CopyToAsync(destinationStream);
                                    sourceStream.Close();
                                    File.Delete("./PDB.sqlite");
                                }
                            }
                        }
                        catch (IOException ioex)
                        {
                            MessageBox.Show("An IOException occured during move, " + ioex.Message);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An Exception occured during move, " + ex.Message);
                        }
                    }
                    try
                    {
                        string connectionForPdb = @"Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\ProfilerDataFolder\PDB.sqlite;Version =3; FailIfMissing=True; Foreign Keys=True;";
                        SQLiteConnection conn = new SQLiteConnection(connectionForPdb);
                        conn.Open();
                        string sql = "select distinct Namespace from PDB";
                        SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                        treeViewForAssemblyDetails.Items.Clear();
                        listBoxForLocalVariables.Items.Clear();
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TreeViewItem item = new TreeViewItem() { Header = reader["Namespace"].ToString() };
                                string sql2 = "select distinct Module from PDB where Namespace='" + reader["Namespace"].ToString() + "'";
                                SQLiteCommand cmd2 = new SQLiteCommand(sql2, conn);
                                using (SQLiteDataReader reader2 = cmd2.ExecuteReader())
                                {
                                    while (reader2.Read())
                                    {
                                        TreeViewItem nesteditem = new TreeViewItem() { Header = reader2["Module"].ToString() };
                                        string sql3 = "select DISTINCT Function from PDB where Module='" + reader2["Module"].ToString() + "';";
                                        SQLiteCommand cmd3 = new SQLiteCommand(sql3, conn);
                                        using (SQLiteDataReader reader3 = cmd3.ExecuteReader())
                                        {
                                            while (reader3.Read())
                                            {
                                                TreeViewItem nesteditem2 = new TreeViewItem() { Header = reader3["Function"].ToString() };
                                                nesteditem2.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
                                                nesteditem2.MouseDoubleClick += NesteditemForPDB_MouseDoubleClick;
                                                nesteditem.Items.Add(nesteditem2);
                                                nesteditem.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
                                            }
                                        }
                                        item.Items.Add(nesteditem);
                                        item.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
                                    }
                                }
                                treeViewForAssemblyDetails.Items.Add(item);
                                treeViewForAssemblyDetails.FontSize = myCanvas.ActualWidth / 100;
                            }
                        }
                        conn.Close();
                    }
                    catch (SQLiteException e1)
                    {
                    }
                    allHeapObjectsButton.Visibility = Visibility.Visible;
                    treeViewForAssemblyDetails.Visibility = Visibility.Visible;
                }
                else if (buttonClicked.Content.ToString() == "OPEN IN VS")
                {
                    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\Decompile"))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\Decompile");
                        FileInfo[] info = dirInfo.GetFiles("*.sln", SearchOption.AllDirectories);
                        if (info.Length != 0)
                        {
                            try
                            {
                                MessageBox.Show("Select a location where you need to save files");
                                var dialog = new VistaFolderBrowserDialog();
                                dialog.SelectedPath = @"C:\Data\";
                                dialog.ShowDialog();
                                string destination = dialog.SelectedPath;
                                int i = 1;
                                string status = "notdone";
                                while (status == "notdone")
                                {
                                    try
                                    {
                                        Directory.Move(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\Decompile", destination + @"\Reciter" + (i++) + "");
                                        status = "done";
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                Thread.Sleep(500);
                                DirectoryInfo dirInfo2 = new DirectoryInfo(destination + @"\Reciter" + ((i) - 1) + "\\");
                                FileInfo[] info2 = dirInfo2.GetFiles("*.sln", SearchOption.AllDirectories);
                                Process temp = Process.Start(info2[0].FullName);
                            }
                            catch (Exception excep)
                            {
                                MessageBox.Show("closed by not selecting the location...No probs try again");
                            }
                        }
                    }
                }
            }
        }
        private void NesteditemForPDB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            listBoxForLocalVariables.Items.Clear();
            TreeViewItem tempItem = sender as TreeViewItem;
            if (tempItem != null)
            {
                string connectionForPdb = @"Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\ProfilerDataFolder\PDB.sqlite;Version =3; FailIfMissing=True; Foreign Keys=True;";
                SQLiteConnection conn = new SQLiteConnection(connectionForPdb);
                conn.Open();
                string sql = "select DISTINCT Data FROM PDB where Function='" + (tempItem.Header.ToString()) + "';";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    int counter = 0;
                    while (reader.Read())
                    {
                        ListBoxItem itm = new ListBoxItem();
                        itm.Content = reader["Data"].ToString() + " -->" + counter;
                        itm.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
                        itm.Background = Brushes.Transparent;
                        listBoxForLocalVariables.Items.Add(itm);
                        itm.FontSize = myCanvas.ActualWidth / 100;
                        counter += 1;
                    }
                }
                conn.Close();
                listBoxForLocalVariables.Visibility = Visibility.Visible;
            }
        }
        private int pdbReader()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            string fileLocation = "";
            if (fd.FileName != null && fd.FileName.EndsWith(".pdb"))
            {
                fileLocation = fd.FileName;
            }
            if (File.Exists("./PDB.sqlite"))
            {
                File.Delete("./PDB.sqlite");
            }
            ProcessStartInfo startInfo = new ProcessStartInfo(@".\PdbInfo\PDBExamination.exe");
            startInfo.Arguments = "\"" + fileLocation + "\"";
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            Process ptemp = Process.Start(startInfo);
            ptemp.WaitForExit();
            return 1;
        }
        int currentIndexTracker = 0;
        //private RuntimeHelper jsonToObject(int index)
        //{
        //    RuntimeHelper objectFromJson = new RuntimeHelper();
        //    using (FileStream sr = new FileStream(runtimeDirectory+"\\Snapshot"+index+".txt", FileMode.Open, FileAccess.Read))
        //    {
        //        using (StreamReader reader = new StreamReader(sr))
        //        {
        //            using (JsonReader jsReader = new JsonTextReader(reader))
        //            {
        //                JsonSerializer serializer = new JsonSerializer();
        //                objectFromJson = serializer.Deserialize<RuntimeHelper>(jsReader);
        //            }
        //        }
        //    }
        //    return objectFromJson; 
        //}
        private async void SnapshotTrackingItm_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listItem = sender as ListBoxItem;
            if (listItem != null)
            {
                int snapshotIndex = Convert.ToInt32(listItem.Content.ToString().Substring(9, listItem.Content.ToString().Length - 9));
                //Task<RuntimeHelper> dataObject = new Task<RuntimeHelper>(()=> {return jsonToObject(snapshotIndex); });
                //dataObject.Start();
                currentIndexTracker = snapshotIndex;
                labelForSnapshotData.Visibility = Visibility.Visible;
                openDetailsInNewWindowButton.Visibility = Visibility.Visible;
                dataGridForHeapObjects.Visibility = Visibility.Visible;
                textBoxToSearchHeap.Visibility = Visibility.Visible;
                dataGridForCompareHeapObjects.Visibility = Visibility.Hidden;
                //RuntimeHelper dataObjects = await dataObject;
                ObservableCollection<HeapHelper.HeapData> uiBinderForHeapData = heapDataHolder[currentIndexTracker];
                //objectData.HeapList = heapDataHolder[currentIndexTracker];
                dataGridForHeapObjects.ItemsSource = uiBinderForHeapData;
                ObservableCollection<AppDomainDetails> uiBinderForStackData = appDomaianDataHolder[currentIndexTracker];
                dataGridForStackDetails.ItemsSource = uiBinderForStackData;
                int totalCount = 0;
                int totalSize = 0;
                int totalObjectsCreated = 0;
                int gen0 = 0;
                int gen0Size = 0;
                int gen1 = 0;
                int gen1Size = 0;
                int gen2 = 0;
                int gen2Size = 0;
                totalCount = uiBinderForHeapData.Count;
                foreach (var uiBinderForHeapDataItem in uiBinderForHeapData)
                {
                    totalSize += uiBinderForHeapDataItem.Size;
                    totalObjectsCreated += uiBinderForHeapDataItem.objects.Count;
                    foreach (var objects in uiBinderForHeapDataItem.objects)
                    {
                        if (objects.Generation == 0)
                        {
                            gen0++;
                            gen0Size += objects.Size;
                        }
                        else if (objects.Generation == 1)
                        {
                            gen1++;
                            gen1Size += objects.Size;
                        }
                        else if (objects.Generation == 2)
                        {
                            gen2++;
                            gen2Size += objects.Size;
                        }
                    }
                }
                labelForSnapshotData.Content = "Snapshots>> Snapshot#" + currentIndexTracker + " ||Total Count " + totalCount + " ||Total Size " + totalSize + " ||TotalObjects " + totalObjectsCreated + "||Gen0 " + gen0 + "(" + gen0Size + ")" + "||Gen1 " + gen1 + "(" + gen1Size + ")" + "||Gen2 " + gen2 + "(" + gen2Size + ")";

            }
        }
        private void Itm_MouseEnterListBoxForSnapshotTracking(object sender, MouseEventArgs e)
        {
            var item = sender as ListBoxItem;
            textBlock.Text = "SNAPSHOT CREATED AT: " + item.Tag + " after process start\n SNAPSHOT NAME :" + item.Content;
            MyPopup.Placement = PlacementMode.MousePoint;
            MyPopup.IsOpen = true;
        }
        /// <summary>
        /// Configuring element height and width on window size changed.
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
        private void startProfilingButton_Click(object sender, RoutedEventArgs e)
        {
            closeCurrentSessionButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            pdbInfoButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            startProfilingButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            profilerButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            codeGenerationButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            exitButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            startProfilingButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF9CAFEA"));
            treeViewForAssemblyDetails.Visibility = Visibility.Hidden;
            dataGridForCompareHeapObjects.Visibility = Visibility.Hidden;
            listBoxForLocalVariables.Visibility = Visibility.Hidden;
            allHeapObjectsButton.Content = "TAKE SNAPSHOT";
            listBoxForCode.Visibility = Visibility.Hidden;
            textBoxForCode.Visibility = Visibility.Hidden;
            treeView.Visibility = Visibility.Hidden;
            listBoxForInnerMethods.Visibility = Visibility.Hidden;
            listBoxForStack.Visibility = Visibility.Hidden;
            if (listBoxForSnapshotTracking.Items.Count > 0)
            {
                listBoxForSnapshotTracking.Visibility = Visibility.Visible;
                dataGridForHeapObjects.Visibility = Visibility.Visible;
                textBoxToSearchHeap.Visibility = Visibility.Visible;
                openDetailsInNewWindowButton.Visibility = Visibility.Visible;
                appDomaianObjectsSwappperButton.Visibility = Visibility.Visible;
            }
            if (listBoxForSnapshotTracking.Items.Count > 1)
            {
                buttonToCompareSnapshots.Visibility = Visibility.Visible;
                comboBoxForA.Visibility = Visibility.Visible;
                comboBoxForB.Visibility = Visibility.Visible;
            }
            textBoxForIlcode.Visibility = Visibility.Hidden;
            allHeapObjectsButton.Visibility = Visibility.Visible;
        }
        private ObservableCollection<Differences> DifferenceIdentifier()
        {
            ObservableCollection<HeapHelper.HeapData> snapshotA = heapDataHolder[selValueInA];
            ObservableCollection<HeapHelper.HeapData> snapshotB = heapDataHolder[selValueInB];
            ObservableCollection<Differences> differenceObjects = new ObservableCollection<Differences>();
            foreach (var item in snapshotA)
            {
                List<HeapHelper.HeapData> snapshotBItems = snapshotB.Where(e1 => e1.Type == item.Type).ToList();
                if (snapshotBItems.Count > 0)
                {
                    List<HeapHelper.HeapData> mappedData = snapshotB.Where(e1 => e1.Type == item.Type).ToList();
                    HeapHelper.HeapData output = mappedData[0];
                    if (item.Count != mappedData[0].Count)
                    {
                        Differences differenceObject = new Differences();
                        differenceObject.Count = item.Count - output.Count;
                        differenceObject.Size = item.Size - output.Size;
                        differenceObject.Type = item.Type;
                        List<HeapHelper.Object> objects = (from firstItem in item.objects
                                                           join secondItem in output.objects
                                                           on firstItem.Address equals secondItem.Address
                                                           select firstItem).ToList();
                        List<HeapHelper.Object> frstItemObjects = item.objects.Where(e1 => !objects.Any(item2 => item2.Address == e1.Address)).ToList();
                        List<HeapHelper.Object> secondItemObjects = output.objects.Where(e1 => !objects.Any(item2 => item2.Address == e1.Address)).ToList();

                        foreach (var obj in frstItemObjects)
                        {
                            differenceObject.objects.Add(obj);
                        }
                        foreach (var obj in secondItemObjects)
                        {
                            differenceObject.objects.Add(obj);
                        }
                        differenceObjects.Add(differenceObject);
                    }
                }
                else
                {
                    Differences differenceObject = new Differences();
                    differenceObject.Count = item.Count;
                    differenceObject.Size = item.Size;
                    differenceObject.Type = item.Type;
                    foreach (var obj in item.objects)
                    {
                        differenceObject.objects.Add(obj);
                    }
                    differenceObjects.Add(differenceObject);
                }
            }
            foreach (var item2 in snapshotB)
            {
                List<HeapHelper.HeapData> snapshotAItems = snapshotA.Where(e1 => e1.Type == item2.Type).ToList();
                if (snapshotAItems.Count < 1)
                {
                    Differences differenceObject = new Differences();
                    differenceObject.Count = -item2.Count;
                    differenceObject.Size = -item2.Size;
                    differenceObject.Type = item2.Type;
                    foreach (var obj in item2.objects)
                    {
                        differenceObject.objects.Add(obj);
                    }
                    differenceObjects.Add(differenceObject);
                }
            }
            return differenceObjects;
        }
        int selValueInA = 0;
        int selValueInB = 0;
        private async void buttonToCompareSnapshots_Click(object sender, RoutedEventArgs e)
        {
            textBoxToSearchHeap.Visibility = Visibility.Visible;
            if (comboBoxForA.SelectedValue != null && comboBoxForB.SelectedValue != null)
            {
                var test = comboBoxForA.SelectedValue.ToString();
                ComboBoxItem itemSelectedFromA = (ComboBoxItem)comboBoxForA.SelectedItem;
                ComboBoxItem itemSelectedFromB = (ComboBoxItem)comboBoxForB.SelectedItem;
                int selectedValueInA = (int)itemSelectedFromA.Tag;
                selValueInA = selectedValueInA;
                int selectedValueInB = (int)itemSelectedFromB.Tag;
                selValueInB = selectedValueInB;
                if (selectedValueInA == selectedValueInB)
                {
                    MessageBox.Show("Please choose different snapshots to compare");
                    return;
                }
                else
                {
                    Loadlabel.Content = "Getting Differences\n    Please wait";
                    Loadlabel.Visibility = Visibility.Visible;
                    Task<ObservableCollection<Differences>> diffCollector = new Task<ObservableCollection<Differences>>(DifferenceIdentifier);
                    diffCollector.Start();
                    var differenceObjects = await diffCollector;
                    if (differenceObjects.Count > 0)
                    {
                        dataGridForCompareHeapObjects.ItemsSource = differenceObjects;
                        dataGridForCompareHeapObjects.Visibility = Visibility.Visible;
                        labelForSnapshotData.Visibility = Visibility.Visible;
                        labelForSnapshotData.Content = "Comparing Snapshot#" + selectedValueInA + " Snapshot#" + selectedValueInB;
                        openDetailsInNewWindowButton.Visibility = Visibility.Hidden;
                        currentDifference = differenceObjects;
                        Loadlabel.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        MessageBox.Show("There is no difference between objects at both snapshots");
                        labelForSnapshotData.Visibility = Visibility.Hidden;
                        Loadlabel.Visibility = Visibility.Hidden;
                    }

                }
            }
            else
            {
                MessageBox.Show("Please select the snapshots and click me to compare");
            }
        }
        ObservableCollection<Differences> currentDifference = new ObservableCollection<Differences>();
        /// <summary>
        /// On close event handler of the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalyzingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path);
                }
                catch (Exception ex)
                {
                }
            }
        }
        private void DataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg == null)
                return;
            if (dg.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            else
                dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }
        private void appDomaianObjectsSwappperButton_Click(object sender, RoutedEventArgs e)
        {
            Button datachanger = sender as Button;
            if (dataGridForHeapObjects.Visibility == Visibility.Visible)
            {
                dataGridForHeapObjects.Visibility = Visibility.Hidden;
                dataGridForCompareHeapObjects.Visibility = Visibility.Hidden;
                dataGridForStackDetails.Visibility = Visibility.Visible;
                datachanger.Content = "OBJECT DETAILS";
                labelForSnapshotData.Content = "Snapshot>>Snapshot#" + currentIndexTracker + ">>AppDomainDetails";
                textBoxToSearchHeap.Visibility = Visibility.Hidden;
            }
            else if (dataGridForStackDetails.Visibility == Visibility.Visible)
            {
                textBoxToSearchHeap.Visibility = Visibility.Visible;
                dataGridForHeapObjects.Visibility = Visibility.Visible;
                dataGridForCompareHeapObjects.Visibility = Visibility.Hidden;
                dataGridForStackDetails.Visibility = Visibility.Hidden;
                datachanger.Content = "APP DOMAIN DETAILS";
                int totalCount = 0;
                int totalSize = 0;
                int totalObjectsCreated = 0;
                int gen0 = 0;
                int gen0Size = 0;
                int gen1 = 0;
                int gen1Size = 0;
                int gen2 = 0;
                int gen2Size = 0;
                ObservableCollection<HeapHelper.HeapData> uiBinderForHeapData = heapDataHolder[currentIndexTracker];
                totalCount = uiBinderForHeapData.Count;
                foreach (var uiBinderForHeapDataItem in uiBinderForHeapData)
                {
                    totalSize += uiBinderForHeapDataItem.Size;
                    totalObjectsCreated += uiBinderForHeapDataItem.objects.Count;
                    foreach (var objects in uiBinderForHeapDataItem.objects)
                    {
                        if (objects.Generation == 0)
                        {
                            gen0++;
                            gen0Size += objects.Size;
                        }
                        else if (objects.Generation == 1)
                        {
                            gen1++;
                            gen1Size += objects.Size;
                        }
                        else if (objects.Generation == 2)
                        {
                            gen2++;
                            gen2Size += objects.Size;
                        }
                    }
                }
                labelForSnapshotData.Content = "Snapshots>> Snapshot#" + currentIndexTracker + " ||Total Count " + totalCount + " ||Total Size " + totalSize + " ||TotalObjects " + totalObjectsCreated + "||Gen0 " + gen0 + "(" + gen0Size + ")" + "||Gen1 " + gen1 + "(" + gen1Size + ")" + "||Gen2 " + gen2 + "(" + gen2Size + ")";
            }
            else if (dataGridForCompareHeapObjects.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Please double click snapshot in list");
            }
        }
        private void openInNewWindowButton_Click(object sender, RoutedEventArgs e)
        {
            int index = currentIndexTracker;
            if (dataGridForStackDetails.Visibility == Visibility.Visible)
            {
                SnapshotData openWindow = new SnapshotData(appDomaianDataHolder[index], index);
                openWindow.Show();
            }
            else if (dataGridForHeapObjects.Visibility == Visibility.Visible)
            {
                SnapshotData openWindow = new SnapshotData(heapDataHolder[currentIndexTracker], index);
                openWindow.Show();

            }
        }
        private void threadModuleDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            AppDomainDetails selectedAppDomain = dataGridForStackDetails.SelectedItem as AppDomainDetails;
            if (selectedAppDomain != null)
            {
                SnapshotData dataSender = new SnapshotData(selectedAppDomain, currentIndexTracker);
                dataSender.Show();
            }
        }
        private void objectDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            HeapHelper.HeapData selectedObjects = dataGridForHeapObjects.SelectedItem as HeapHelper.HeapData;
            if (selectedObjects != null)
            {
                SnapshotData dataSender = new SnapshotData(selectedObjects, currentIndexTracker);
                dataSender.Show();
            }
        }

        private void textBoxToSearchHeap_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataGridForHeapObjects.Visibility == Visibility.Visible)
            {
                ObservableCollection<HeapHelper.HeapData> heapData = heapDataHolder[currentIndexTracker];
                List<HeapHelper.HeapData> filteredHeaData = heapData.Where(e1 => e1.Type.ToLower().Contains(textBoxToSearchHeap.Text)).ToList();
                dataGridForHeapObjects.ItemsSource = filteredHeaData;
            }
            if (dataGridForCompareHeapObjects.Visibility == Visibility.Visible)
            {
                ObservableCollection<Differences> heapData = currentDifference;
                List<Differences> filteredHeaData = heapData.Where(e1 => e1.Type.ToLower().Contains(textBoxToSearchHeap.Text)).ToList();
                dataGridForCompareHeapObjects.ItemsSource = filteredHeaData;
            }

        }

        private async void exitButton_Click(object sender, RoutedEventArgs e)
        {
            closeCurrentSessionButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            pdbInfoButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            startProfilingButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            profilerButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            codeGenerationButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            exitButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            exitButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF9CAFEA"));
            App.Current.Shutdown();
            exitButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
        }
        int functionToCloseSession()
        {
            if (MessageBox.Show("Close current session data ", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                return 1;
            }
            else return 0;
        }
        private async void closeCurrentSessionButton_Click(object sender, RoutedEventArgs e)
        {
            closeCurrentSessionButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            pdbInfoButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            startProfilingButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            profilerButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            codeGenerationButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            exitButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            closeCurrentSessionButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF9CAFEA"));

            Task<int> task = new Task<int>(functionToCloseSession);
            task.Start();
            int i = await task;
            if (i == 1)
            {
                MainWindow window = new MainWindow();
                window.Show();
                this.Close();
            }
            else
            {
                MainWindow window = new MainWindow();
                window.Show();
                closeCurrentSessionButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7186C7"));
            }
        }
        private int SaveData(DataBackUp dataToStore)
        {
            //DataBackUp dataToStore = new DataBackUp();
            //dataToStore.Date = DateTime.Now.ToString("d");
            //dataToStore.Time = DateTime.Now.ToString("T");
            //dataToStore.processName = processName;
            //foreach (var items in listBoxForSnapshotTracking.Items)
            //{
            //    var item = items as ListBoxItem;
            //    if (item != null)
            //    {
            //        dataToStore.timestamp.Add(item.Tag.ToString());
            //    }
            //}
            //dataToStore.snapshotData.heapDataSet = heapDataHolder;
            //dataToStore.snapshotData.appDomaianDataSet = appDomaianDataHolder;
            ////FileStream fs = new FileStream(".\\DataStore\\Test.txt", FileMode.Create);
            //TextWriter tmp = Console.Out;
            //StreamWriter sw = new StreamWriter(fs);
            //Console.SetOut(sw);
            //var jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(dataToStore, Formatting.Indented,
            //  new JsonSerializerSettings
            //  {
            //      PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            //      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //  }
            //  );
            //DataBackUp dataStored = JsonConvert.DeserializeObject<DataBackUp>(jsonResult);
            //Console.WriteLine(jsonResult);
            //sw.Close();
            //var filePath = ".\\DataStore\\Test.txt";

            using (var fs = File.Open(".\\DataStore\\Test.txt", FileMode.Create))
            using (var sw = new StreamWriter(fs))
            using (var jw = new JsonTextWriter(sw))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jw, dataToStore);
            }
            //TextFileToBinary(".\\DataStore\\Test.txt");           
            return 1;
        }
        //private static void TextFileToBinary(string TexFilePath)
        //{
        //    string fileContents;
        //    using (FileStream fileStream = new FileStream(TexFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        using (StreamReader sr = new StreamReader(fileStream))
        //        {
        //            fileContents = sr.ReadToEnd();
        //        }
        //    }
        //    using (FileStream fs = new FileStream(TexFilePath.Replace("txt", "bin"), FileMode.Create))
        //    {
        //        // Construct a BinaryFormatter and use it to serialize the data to the stream.
        //        BinaryFormatter formatter = new BinaryFormatter();
        //        formatter.Serialize(fs, fileContents);
        //    }
        //}
        //private static void BinaryToTextFile(string BinaryFilePath)
        //{
        //    string fileContents;
        //    using (FileStream fileStream = new FileStream(BinaryFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        BinaryFormatter formatter = new BinaryFormatter();
        //        fileContents = (string)formatter.Deserialize(fileStream);
        //    }
        //    using (FileStream fs = new FileStream(BinaryFilePath.Replace("bin", "txt"), FileMode.Create))
        //    {
        //        using (StreamWriter sw = new StreamWriter(BinaryFilePath.Replace("bin", "txt")))
        //        {
        //            sw.Write(fileContents);
        //        }
        //    }
        //}
        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //m_dbConnection.Close();
            if (MessageBox.Show("ARE YOU WANT TO CLOSE?", "CLOSING", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
                //if (heapDataHolder.Keys.Count >= 1)
                //{
                //    if (MessageBox.Show("Do you want to save this session data", "Save Data", MessageBoxButton.YesNo) == MessageBoxResult.No)
                //    {
                //        e.Cancel = false;
                //    }
                //    else
                //    {
                //        //DataBackUp dataToStore = new DataBackUp();
                //        //dataToStore.Date = DateTime.Now.ToString("d");
                //        //dataToStore.Time = DateTime.Now.ToString("T");
                //        //dataToStore.processName = processName;
                //        //foreach (var items in listBoxForSnapshotTracking.Items)
                //        //{
                //        //    var item = items as ListBoxItem;
                //        //    if (item != null)
                //        //    {
                //        //        dataToStore.timestamp.Add(item.Tag.ToString());
                //        //    }
                //        //}
                //        //textBlockLoading.Text = "Packing your data please wait...";
                //        ////.Placement = PlacementMode.MousePoint;
                //        //MyPopupLoader.IsOpen = true;
                //        //dataToStore.snapshotData.heapDataSet = heapDataHolder;
                //        //dataToStore.snapshotData.appDomaianDataSet = appDomaianDataHolder;
                //        //loader.Visibility = Visibility.Visible;
                //        //SaveData(dataToStore);
                //        //MyPopupLoader.IsOpen = false;
                //        //MessageBox.Show("done");
                //        //e.Cancel = false;
                //    }
                //}
            }
        }
    }
}
