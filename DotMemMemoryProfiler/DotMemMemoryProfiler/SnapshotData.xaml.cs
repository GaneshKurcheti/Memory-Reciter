using CLRMD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DotMemMemoryProfiler
{
    /// <summary>
    /// Interaction logic for SnapshotData.xaml
    /// </summary>
    public partial class SnapshotData : Window
    {
        ObservableCollection<AppDomainDetails> appDomainCollection=null;
        AppDomainDetails appdomainData=null;
        int index;
        HeapHelper.HeapData objectsData=null;
        ObservableCollection<HeapHelper.HeapData> heapObjectsDetails = null;
        public SnapshotData(ObservableCollection<AppDomainDetails> appDomainDetails,int index)
        {
            InitializeComponent();
            appDomainCollection = appDomainDetails;
            this.index = index;
        }
        public SnapshotData(AppDomainDetails appDomainDetails, int index)
        {
            InitializeComponent();
            appdomainData = appDomainDetails;
            this.index =  index;
        }
        public SnapshotData(HeapHelper.HeapData objectDetails, int index)
        {
            InitializeComponent();
            objectsData = objectDetails;
            this.index = index;
        }
        public SnapshotData(ObservableCollection<HeapHelper.HeapData> objectDetails, int index)
        {
            InitializeComponent();
            heapObjectsDetails = objectDetails;
            this.index = index;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(appDomainCollection!=null)
            {
                dataGridForStackDetails.Visibility = Visibility.Visible;
                ThreadDetails.Visibility = Visibility.Hidden;
                dataGridForStackDetails.ItemsSource = appDomainCollection;
                ModuleDetails.Visibility = Visibility.Hidden;
                datagridForObjects.Visibility = Visibility.Hidden;
                dataGridForHeapObjects.Visibility = Visibility.Hidden;
                DetailsLabel.Content = "Snapshot#" + index + " >>" + "APP DOMAIN DETAILS";
            }
            else if(appdomainData!=null)
            {
                ThreadDetails.ItemsSource = appdomainData.ThreadsInAppDomain;
                dataGridForStackDetails.Visibility = Visibility.Hidden;
                ThreadDetails.Visibility = Visibility.Visible;
                ModuleDetails.ItemsSource = appdomainData.ModulesInAppDomain;
                ModuleDetails.Visibility = Visibility.Visible;
                datagridForObjects.Visibility = Visibility.Hidden;
                dataGridForHeapObjects.Visibility = Visibility.Hidden;
                DetailsLabel.Content = "Snapshot#" + index + ">> APP DOMAIN DETAILS>>" + appdomainData.AppDomainName + ">>THREAD AND MODULE DETAILS";
            }
            else if(heapObjectsDetails!=null)
            {
                dataGridForHeapObjects.ItemsSource = heapObjectsDetails;
                dataGridForHeapObjects.Visibility = Visibility.Visible;
                ModuleDetails.Visibility = Visibility.Hidden;
                ThreadDetails.Visibility = Visibility.Hidden;
                datagridForObjects.Visibility = Visibility.Hidden;
                dataGridForStackDetails.Visibility = Visibility.Hidden;
                DetailsLabel.Content = "Snapshot#" + index + ">> HEAP DETAILS";
            }
            else if (heapObjectsDetails != null)
            {
                dataGridForHeapObjects.ItemsSource = heapObjectsDetails;
                dataGridForHeapObjects.Visibility = Visibility.Visible;
                ModuleDetails.Visibility = Visibility.Hidden;
                ThreadDetails.Visibility = Visibility.Hidden;
                datagridForObjects.Visibility = Visibility.Hidden;
                dataGridForStackDetails.Visibility = Visibility.Hidden;
                DetailsLabel.Content = "Snapshot#" + index + ">> HEAP DETAILS";

            }
            else if(objectsData!=null)
            {
                datagridForObjects.ItemsSource = objectsData.objects;
                datagridForObjects.Visibility = Visibility.Visible;
                ModuleDetails.Visibility = Visibility.Hidden;
                ThreadDetails.Visibility = Visibility.Hidden;
                dataGridForHeapObjects.Visibility = Visibility.Hidden;
                DetailsLabel.Content = "Snapshot#" + index + ">> HEAP DETAILS>>OBJECTSOF "+objectsData.Type;
            }
        }
    }
}
