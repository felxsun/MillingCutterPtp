using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingCutterPtp
{
    public class msgString
    {
        private Dictionary<int, string> error;
        private Dictionary<string, string> messages;
        
        public string err(int i)
        {
            if (!error.ContainsKey(i))
                throw new Exception("Key " + i.ToString() + " not exist");
            return error[i];
        }

        public string str(string key)
        {
            if(!messages.ContainsKey(key))
                throw new Exception("Key " + key + " not exist");
            return messages[key];
        }

        public msgString ()
        {
            initialError();
            initialMessage();
        }

        private void initialError()
        {
            error = new Dictionary<int, string>();
            error.Add(0, "一般錯誤");
            error.Add(1, "設定檔遺失,無法起啟");
            error.Add(2, "找不到相機 {0} ： {1}");
            error.Add(4, "無法連接相機");
            error.Add(5, "找不到相機");
            error.Add(6, "相機初始化失敗");
            error.Add(7, "光源控制器參數錯誤");
            error.Add(8, "光源控制器通訊port開啟失敗");
            error.Add(9, "發送檢查訊息失敗");
            error.Add(10, "讀取光源chnnel失敗");
            error.Add(11, "讀取光源亮度失敗");
            error.Add(12, "PLC參數錯誤");
            error.Add(13, "PLC通訊port開啟失敗");
        }

        private void initialMessage()
        {
            messages = new Dictionary<string, string>();
            messages.Add("exit?", "結束程式?");
            messages.Add("millingCuterClassifier", "銑刀分類系統");
            messages.Add("disconnected", "斷線");
            messages.Add("connected", "連線中");
            messages.Add("cameraDisconnected", "相機斷線");
            messages.Add("Stop", "停機");
            messages.Add("Pause", "暫停");
            messages.Add("Error","處理錯誤");
            messages.Add("UnKnow", "未定義錯誤");
            messages.Add("OnLine", "上線服務");
            messages.Add("NotReady", "未備便,待機中");
            messages.Add("Turn On", "己開啟");
            messages.Add("Turn Off", "關閉");

        }
    }
}
