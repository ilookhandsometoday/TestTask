using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestTask
{
    public class Image: INotifyPropertyChanged
    {
        private string path;
        private int expectedSize;
        private int currentSize;
        private bool finishedDownloading;

        public Image(string path, int size) 
        {
            this.path = path;
            this.expectedSize = size;
            this.currentSize = 0;
            this.finishedDownloading = false;
        }
        
        public string Path 
        { 
            get { return this.path; }
            set { this.path = value; }
        }

        public int Size
        {
            get { return this.expectedSize; }
            set { this.expectedSize = value; }
        }

        public bool FinishedDownloading
        {
            get { return this.finishedDownloading; }
            set { this.finishedDownloading = value; }
        }

        public int CurrentSize 
        {
            get { return this.currentSize; }
            set { this.currentSize = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = "") 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddToCurrentSize(object sender, DownloadEventArgs e) 
        {

        }
    }
}
