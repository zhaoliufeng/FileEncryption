using FileEncryption.model;
using FileEncryption.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class RemoteFileListWindow : Window
    {
        private const string COLUM_FILE_NAME = "文件名";
        private const string COLUM_OUT_TIME = "输出时间";
        private const string COLUM_DOWNLOAD_COUNT = "下载量";

        //判断是否选中列表子项
        private bool isItemSelected = false;

        public ListView mListView;

        private List<FileItem> fileItems;

        //当前选中的列名 当点击列表头进行排序的时候 如果点击的列表头名与当前选中列名相同 则取反当前排序方式
        private String mCurrColum = "Name";
        private ListSortDirection mCurrSortDirection = ListSortDirection.Ascending;
        //文件请求路径
        private String severFileUpdatePath = "http://firmwarebin.we-smart.cn:8000";

        public RemoteFileListWindow()
        {
            InitializeComponent();
            mListView = (ListView)this.FindName("listView");
            severFileUpdatePath = AppConfiger.getServertPath();
            getReomteFileList();
        }

        private void getReomteFileList()
        {
            fileItems = new List<FileItem>();
            String jsonString = HTTPRequestMannager.RequestDataRemote(severFileUpdatePath + ConstValue.GET_REMOTE_FILE_LIST);
            FileItem file = new FileItem { };
            JsonArray jsonArray = new JsonArray(jsonString);
            for (int i = 0; i < jsonArray.length(); i++)
            {
                FileItem fileItem = new FileItem();
                fileItem.Name = jsonArray.get(i).getString("mFileName");
                fileItem.OutTime = jsonArray.get(i).getString("mFileUpdateTime");
                fileItem.Count = jsonArray.get(i).getString("mCount");
                fileItem.Desc = jsonArray.get(i).getString("mDesc");
                fileItems.Add(fileItem);
            }

            foreach (FileItem f in fileItems)
            {
                mListView.Items.Add(
                    new FileItem { Name = f.Name, OutTime = f.OutTime , Count = f.Count, Desc = f.Desc}
                );
            }

        }

        private void delete_file(object sender, RoutedEventArgs e)
        {
            if (!isItemSelected)
                return;
            FileItem fileItem = mListView.SelectedItem as FileItem;
            deleteFile(fileItem);
        }

        //删除文件
        private void deleteFile(FileItem fileItem)
        {
            String rcode = HTTPRequestMannager.RequestDataRemote(severFileUpdatePath + ConstValue.DELETE_FILE + "&fc=" + fileItem.Name);
            if (rcode.Equals("OK"))
            {
                mListView.Items.Remove(fileItem);
            }
            else
            {
                MessageBox.Show("删除失败");
            }
        }

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

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader gch = e.OriginalSource as GridViewColumnHeader;
            String columnHeaderContent = (String)gch.Content;
            String propertyName;
            switch (columnHeaderContent)
            {
                case COLUM_FILE_NAME:
                    propertyName = "Name";
                    break;
                case COLUM_DOWNLOAD_COUNT:
                    propertyName = "Count";
                    break;
                case COLUM_OUT_TIME:
                    propertyName = "OutTime";
                    break;
                default:
                    propertyName = "Name";
                    break;
            }
            //如果当前点击的列表头名称跟上次相同 则排序方式取反
            if (mCurrColum.Equals(propertyName))
            {
                mCurrSortDirection = 
                    mCurrSortDirection == ListSortDirection.Ascending ?
                    ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            mCurrColum = propertyName;
            sort(propertyName, mCurrSortDirection);
        }

        //排序
        private void sort(string propertyName, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(this.listView.Items);//获取数据源视图
            dataView.SortDescriptions.Clear();//清空默认排序描述
            SortDescription sd = new SortDescription(propertyName, direction);
            dataView.SortDescriptions.Add(sd);//加入新的排序描述
            dataView.Refresh();//刷新视图
        }
    }
}
