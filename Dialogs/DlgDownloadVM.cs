using System;
using System.ComponentModel;
using System.Windows;

namespace WpfCef.Dialogs
{
    public class DlgDownloadVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private System.Net.WebClient client;
        private int retry;
        private string fileName;

        public string url { get; set; }
        public string ret { get; set; }

        public string DownloadTitle { get; set; }
        /// <summary>
        /// Download Progress
        /// </summary>
        public int ProgressValue { get; set; }

        public void Init() {
            if (retry > 0) {
                DownloadTitle += "("+ retry + ")";
            }
            OnPropertyChanged("DownloadTitle");
            OnPropertyChanged("ProgressValue");

            fileName = System.Guid.NewGuid().ToString();
            client = new System.Net.WebClient();
            client.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.Proxy = System.Net.WebRequest.DefaultWebProxy;
            client.Proxy.Credentials = new System.Net.NetworkCredential();
            client.DownloadFileAsync(new Uri(url), fileName);
        }

        /// <summary>
        /// 下载进度条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;
            OnPropertyChanged("ProgressValue");
        }


        /// <summary>
        /// 下载完成调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ret = System.IO.File.ReadAllText(fileName);
            OnPropertyChanged("ret");
            //this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            try { System.IO.File.Delete(fileName); } catch { }
        }


    }
}
