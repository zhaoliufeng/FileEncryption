using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileEncryption.utils
{
    class JsonArray
    {
        private String mJsonData;
        private List<JsonObject> mObjList;

        public JsonArray(String json)
        {
            this.mJsonData = json;
            mObjList = new List<JsonObject>();
            parserJsonData(json);
        }

        private void parserJsonData(String json)
        {
            //去除\"转义字符
            json = json.Replace("\\", "");

            if (json.StartsWith("[{") && json.EndsWith("}]"))
            {
                Console.WriteLine("开始解析json字符串");

                //开始剥离外层花括号
                json = json.Replace("[{", "");
                json = json.Replace("}]", "");
            }
            else if (json.StartsWith("{") && json.EndsWith("}"))
            {
                //开始剥离外层花括号
                json = json.Replace("{", "");
                json = json.Replace("}", "");
            }
            else
            {
                Console.WriteLine("json字符串错误");
                return;
            }
            //根据, 拆分数组
            String[] objStringAraay = Regex.Split(json, "},{", RegexOptions.IgnoreCase);
            foreach (String objString in objStringAraay)
            {
                mObjList.Add(new JsonObject(objString));
            }
        }

        public JsonObject get(int index)
        {
            return mObjList[index];
        }

        public int length()
        {
            return mObjList.Count;
        }
    }
}
