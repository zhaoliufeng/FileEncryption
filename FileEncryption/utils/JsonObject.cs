using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryption.utils
{
    class JsonObject
    {
        private String mJsonData;
        private Dictionary<String, String> mDataDics;

        public JsonObject(String json)
        {
            this.mJsonData = json;
            this.mDataDics = new Dictionary<string, string>();
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

            Console.WriteLine(json);
            //以，为分隔符分解字符 拿到一个 "key":"value"的键值对
            String[] keyValArray = json.Split(',');
            foreach (String keyValue in keyValArray)
            {
                String[] keyValSplit = keyValue.Split(new char[] { ':' },2);
                mDataDics.Add(keyValSplit[0].Replace("\"", ""), keyValSplit[1].Replace("\"", ""));
            }
        }

        private String getValue(String key)
        {
            String val = "";
            if (mDataDics.TryGetValue(key, out val))
            {
                return val;
            }
            return val;
        }

        public short getShort(String key)
        {
            return short.Parse(getValue(key));
        }

        public int getInt(String key)
        {
            return int.Parse(getValue(key));
        }

        public long getLong(String key)
        {
            return long.Parse(getValue(key));
        }

        public String getString(String key)
        {
            return getValue(key);
        }

        public float getFloat(String key)
        {
            return float.Parse(getValue(key));
        }

        public double getDouble(String key)
        {
            return double.Parse(getValue(key));
        }

    }
}
