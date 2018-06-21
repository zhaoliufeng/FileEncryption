using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryption.model
{
    class ConstValue
    {
        private static int OP_GET_FILE = 0;
        private static int OP_GET_FILE_LIST = 1;
        private static int OP_DELETE_FILE = 2;

        //获取文件路径请求
        public static String GET_FILE_PATH = "/firmwarebin/FileServlet?op=" + OP_GET_FILE;

        //获取远程文件列表请求
        public static String GET_REMOTE_FILE_LIST = "/firmwarebin/FileServlet?op=" + OP_GET_FILE_LIST;
        //public static String GET_REMOTE_FILE_LIST = "http://127.0.0.1:8080/firmwarebin/FileServlet?op=1";

        //软件使用帮助网址
        public static String USE_HELP = "/firmwarebin/help.htm";

        //删除文件指令
        public static String DELETE_FILE = "/firmwarebin/FileServlet?op=" + OP_DELETE_FILE; 
    }
}
