using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace FileEncryption.utils
{
    //http协议请求管理类
    class HTTPRequestMannager
    {
        /**
         * @param fileName 文件名
         * @param filePath 文件路径
         * @param serverPath 服务器路径
         * @return 请求解决状态字符串
         **/
        public static String uploadFile(String fileName, String filePath, String serverPath)
        {
            DateTime start = DateTime.Now;

            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(serverPath));

            string boundary = "----" + DateTime.Now.Ticks.ToString("x");//分割标记 随机数 
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"" + fileName + "" + "\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: application/octet-stream");
            sb.Append("\r\n\r\n");

            httpReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            httpReq.KeepAlive = true;
            httpReq.ContentType = "multipart/form-data; boundary=" + boundary;//报文头部结尾
            httpReq.Method = "POST";
            httpReq.ContinueTimeout = 6000;

            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);

                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sb.ToString());
                byte[] boundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                //http请求总长度  
                httpReq.ContentLength = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;
                Stream requestStream = httpReq.GetRequestStream();

                Console.WriteLine(httpReq.ContentLength);
                //写报文头
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];

                //写报文
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                fileStream.Dispose();
                //写报文尾
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

                WebResponse response = httpReq.GetResponse();
                Stream respStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(respStream);

                String sReturnString = reader.ReadLine();
                Console.WriteLine("retcode=" + sReturnString);
                respStream.Close();
                reader.Close();

                return "上传成功";
            }catch(FileNotFoundException fnfe)
            {
                return "文件不存在";
            }catch(IOException ioe)
            {
                return "上传失败";
            } 
        }

        public static String RequestDataRemote(String serverPath)
        {
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(serverPath));
            WebResponse response = httpReq.GetResponse();
            Stream respStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(respStream);

            String sReturnString = reader.ReadLine();
            Console.WriteLine("retcode=" + sReturnString);
            respStream.Close();
            reader.Close();
            return sReturnString;
        }
    }
}
