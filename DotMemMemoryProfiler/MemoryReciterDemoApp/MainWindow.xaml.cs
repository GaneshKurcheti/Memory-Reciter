using MemoryReciterDemoApp.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MemoryReciterDemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _demo1ObjectCount = 0;
        int _demo2ObjectCount = 0;
        List<Demo1> _demo1Objects;  
        List<Demo2> _demo2Objects;  
        public MainWindow()
        {
            InitializeComponent();
            _demo1Objects = new List<Demo1>();
            _demo2Objects = new List<Demo2>();
            gcButton.IsEnabled = false;
            removeReferenceButton.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Demo1 t1 = new Demo1();
            // Weak reference wil;l keep the object available in memory for a while but doesntot restrict gc from collecting it.
            WeakReference Weakreference = new WeakReference(t1, false);
            t1.Test = _demo1ObjectCount;
            _demo1Objects.Add(t1);
            testObjects1Count.Content = ++_demo1ObjectCount;
            gcButton.IsEnabled = false;
            removeReferenceButton.IsEnabled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Demo2 t2 = new Demo2();

            WeakReference reference = new WeakReference(t2, false);
            _demo2Objects.Add(t2);
            t2.Test = _demo2ObjectCount;
            testObjects2Count.Content = ++_demo2ObjectCount;
            gcButton.IsEnabled = false;
            removeReferenceButton.IsEnabled = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            if (_demo1Objects.Count == 0 && _demo2Objects.Count == 0)
            {
                _demo1ObjectCount = 0;
                _demo2ObjectCount = 0;
                testObjects1Count.Content = "";
                testObjects2Count.Content = "";
                gcButton.IsEnabled = false;
                removeReferenceButton.IsEnabled = false;
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _demo1Objects = new List<Demo1>();
            _demo2Objects = new List<Demo2>();
            gcButton.IsEnabled = true;
            removeReferenceButton.IsEnabled = false;
        }
    }
}
