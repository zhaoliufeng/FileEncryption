using System;
using System.Collections.Generic;
using System.IO;
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

namespace FileEncryption
{
    public partial class SetOutputPathWindow : Window
    {
        private TextBox mOutPathTextBox;

        public static int SET_LCOAL_PATH = 0;
        public static int SET_SERVER_PATH = 1;
        private int pathType = -1;

        public SetOutputPathWindow(int setType)
        {
            pathType = setType;
            InitializeComponent();
            mOutPathTextBox = (TextBox)this.FindName("outPathTextBox");
            getOutPathOnShow();
            this.Title = pathType == SET_LCOAL_PATH ? "配置输出路径" : "配置服务器地址";
        }
        
        private void getOutPathOnShow()
        {
            if(pathType == SET_LCOAL_PATH)
            {
                mOutPathTextBox.Text = AppConfiger.getOutputPath();
            }
            else
            {
                mOutPathTextBox.Text = AppConfiger.getServertPath();
            }
        }

        private void OnPathSave(object sender, RoutedEventArgs e)
        {
            String path = mOutPathTextBox.Text;
            if(pathType == SET_LCOAL_PATH)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!AppConfiger.saveOutputPath(mOutPathTextBox.Text))
                {
                    MessageBox.Show("保存失败");
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                if (!AppConfiger.saveServerPath(mOutPathTextBox.Text))
                {
                    MessageBox.Show("保存失败");
                }
                else
                {
                    this.Close();
                }
            }
        }
    }
}
