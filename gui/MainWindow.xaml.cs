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

namespace gui
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum OperateMode { Select, IR_Node, IR_Link, Modify }
        public void radio_click(object sender, RoutedEventArgs e)
        {
            for (int i=0;i<4; i++)
            {
                if (sender == operateMode_can.Children[i])
                {
                    OpMode = (OperateMode)i;
                }
            }
            

        }
        public MainWindow()
        {
            InitializeComponent();
            Panel c = FindName("operateMode_can") as Panel;
            System.Diagnostics.Debug.Assert(c != null);
            foreach (RadioButton i in c.Children)
            {
                i.Click += radio_click;
            }

        }
        private OperateMode _opMode;
        public OperateMode OpMode
        {
            get { return _opMode; }
            set {
                _opMode = value;
                RadioButton r = operateMode_can.Children[(int)value] as RadioButton;
                r.IsChecked = true;
            }
        }
    }
}
