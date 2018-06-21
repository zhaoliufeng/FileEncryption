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
using System.IO;
using System.Text.RegularExpressions;
using FileEncryption.utils;
using FileEncryption.model;

namespace FileEncryption
{

    public partial class MainWindow : Window
    {
        //需要加密文件路径
        private String srcFilePath = "";
        //输出文件路径
        private String outFilePath = "C:\\firmwareout";
        //文件上传路径
        private String severFileUpdatePath = "http://firmwarebin.we-smart.cn:8000";
        //private String severFileUpdatePath = "http://127.0.0.1:8000";
        //当前操作文件名
        private String fileName = "";

        //判断是否选中列表子项
        private bool isItemSelected = false;

        //判断是不是从文件列表中被选择的 如果是则不能够再次加密 防止二次加密导致文件解析出错
        private bool isFromFileList = false;


        public MainWindow()
        {
            InitializeComponent();
            outFilePath = AppConfiger.getOutputPath();
            severFileUpdatePath = AppConfiger.getServertPath();
            //判断本地路径是否存在 不存在就创建
            if (!Directory.Exists(outFilePath))
            {
                Directory.CreateDirectory(outFilePath);
            }
            //读取本地文件列表
            refreshFileList(outFilePath);
        }

        /************拖拽事件************/
        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                srcFilePath = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            }

            pathTextBlock.Text = srcFilePath;
            string[] fArray = Regex.Split(srcFilePath, "\\\\", RegexOptions.IgnoreCase);
            fileName = fArray[fArray.Length - 1];
            fileNameLabel.Content = fileName;
            isFromFileList = false;
            Console.WriteLine("文件路径 {0}", srcFilePath);
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            pathTextBlock.Text = "松手拖入文件";
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            pathTextBlock.Text = "拖入固件文件";
        }
        /************拖拽事件************/

        //加密文件
        private void startEncryption(String name)
        {
            if (isFromFileList)
            {
                pathTextBlock.Text = "不能加密输出列表中的文件";
                return;
            }

            //如果不存在输出路径就新建目录
            if (!Directory.Exists(outFilePath))
            {
                Directory.CreateDirectory(outFilePath);
            }
            //如果之前文件存在 则删除文件 重新创建
            if(File.Exists(outFilePath + "\\" + name))
            {
                File.Delete(outFilePath + "\\" + name);
            }
            FileStream fs = new FileStream(srcFilePath, FileMode.Open);
            byte[] array = new byte[fs.Length];
            byte[] outArray = new byte[fs.Length];
            fs.Read(array, 0, array.Length);
            fs.Close();

            //判断是否需要加密
            if (checkBoxEncrypt.IsChecked == true)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    outArray[i] = (byte)(array[i] ^ (i & 0xFF));
                }
            }
            else
            {
                outArray = array;
            }

            string str = Encoding.Default.GetString(outArray);
            Console.WriteLine(str);


            //输出到指定路径
            FileStream outFs = new FileStream(outFilePath + "\\" + name, FileMode.OpenOrCreate);
            outFs.Write(outArray, 0, outArray.Length);
            outFs.Close();
            pathTextBlock.Text = "加密完成";

            refreshFileList(outFilePath);
        }

        //上传至服务器
        private void OnUploadToServer(object sender, RoutedEventArgs e)
        {

            if (srcFilePath == "")
            {
                pathTextBlock.Text = "文件路径为空，请拖入文件";
                return;
            }

            if (codeTextBox.Text == "")
            {
                pathTextBlock.Text = "请输入固件编码";
                return;
            }

            //获取输出文件名 文件名 = 固件编码_固件版本
            fileName = codeTextBox.Text +
                (versionTextBox.Text == "" ?
                "" : "_" + versionTextBox.Text);

            String desc = descTextBox.Text;

            startEncryption(fileName);
            pathTextBlock.Text = fileName + " " + HTTPRequestMannager.uploadFile(fileName, outFilePath + "\\" + fileName, severFileUpdatePath +
          "/firmwarebin/FileUpdateServlet?desc=" + desc);

        }

        /********** 设置 ********/
        //设置本地输出路径
        private void SetOutputPath(object sender, RoutedEventArgs e)
        {
            SetOutputPathWindow outputPathWindow = new SetOutputPathWindow(SetOutputPathWindow.SET_LCOAL_PATH);
            outputPathWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            outputPathWindow.Owner = this;
            outputPathWindow.ShowDialog();
        }

        //设置服务器上传路径
        private void SetServerPath(object sender, RoutedEventArgs e)
        {
            SetOutputPathWindow outputPathWindow = new SetOutputPathWindow(SetOutputPathWindow.SET_SERVER_PATH);
            outputPathWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            outputPathWindow.Owner = this;
            outputPathWindow.ShowDialog();
        }

        //窗口激活时 刷新对应信息
        private void OnWindowActivated(object sender, EventArgs e)
        {
            outFilePath = AppConfiger.getOutputPath();
            severFileUpdatePath = AppConfiger.getServertPath();
            refreshFileList(outFilePath);
        }

        //打开本地文件夹
        private void OnOpenLocalDir(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", outFilePath);
        }

        //列表单击选中
        private void listviewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pathTextBlock.Text = "双击选择文件";
        }

        //双击选中列表项
        private void selectedItem(object sender, MouseButtonEventArgs e)
        {
            FileItem fileItem = listView.SelectedItem as FileItem;
            if (isItemSelected)
            {
                pathTextBlock.Text = fileItem.Name;
                fileNameLabel.Content = fileItem.Name.Replace("_", "__");
                parseFileName(fileItem.Name);
                isFromFileList = true;
            }
        }

        //解析文件名
        private void parseFileName(String fileName)
        {
            String[] keyValSplit = fileName.Split(new char[] { '_' }, 2);
            codeTextBox.Text = keyValSplit[0];
            if(keyValSplit.Length != 1)
            {
                versionTextBox.Text = keyValSplit[1];
            }
        }

        //刷新文件列表
        private void refreshFileList(string path)
        {
            listView.Items.Clear();
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                listView.Items.Add(
                    new FileItem { Name = f.Name, OutTime = f.CreationTime.ToString() }
                );
            }
        }

        //删除文件
        private void deleteFile(string path)
        {
            File.Delete(path);
        }

        //删除文件 点击事件
        private void delete_file(object sender, RoutedEventArgs e)
        {
            if (!isItemSelected)
                return;
            FileItem fileItem = listView.SelectedItem as FileItem;
            deleteFile(outFilePath + "\\" + fileItem.Name);
            refreshFileList(outFilePath);
        }

        //右键弹出删除文件菜单 判断当前是否已经选中文件
        private void listView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            object item = GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
            isItemSelected = (item != null);
        }

        //获取列表对象
        private object GetElementFromPoint(ItemsControl itemsControl, Point point)
        {
            UIElement element = itemsControl.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return null;
                object item = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (!item.Equals(DependencyProperty.UnsetValue))
                    return item;
                element = (UIElement)VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        //右键选中列表对象
        private void listView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object item = GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
            isItemSelected = (item != null);
        }

        private List<FileItem> fileItems;
        //从远程服务器获取文件列表
        private void getFileListForRemote(object sender, RoutedEventArgs e)
        {
            RemoteFileListWindow remoteFileListWindow = new RemoteFileListWindow();
            remoteFileListWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            remoteFileListWindow.Owner = this;
            remoteFileListWindow.ShowDialog();
        }

        //使用帮助
        private void useHelp(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(severFileUpdatePath + ConstValue.USE_HELP);
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /*********** checkbox *************/
        private void encryptChecked(object sender, RoutedEventArgs e)
        {
            if (checkBoxUnEncrypt != null)
            {
                checkBoxUnEncrypt.IsChecked = false;
            }
        }

        private void encryptUnChecked(object sender, RoutedEventArgs e)
        {
            checkBoxUnEncrypt.IsChecked = true;
        }

        private void unEncryptChecked(object sender, RoutedEventArgs e)
        {
            checkBoxEncrypt.IsChecked = false;
        }

        private void unEncryptUnChecked(object sender, RoutedEventArgs e)
        {
            checkBoxEncrypt.IsChecked = true;
        }
    }
}
