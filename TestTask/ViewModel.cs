using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestTask
{
    public class ViewModel: INotifyPropertyChanged
    {
        private Downloader left, center, right;
        public Downloader Left 
        {
            get { return this.left; }
        }

        public Downloader Center 
        {
            get { return this.center; }
        }
        public Downloader Right
        {
            get { return this.right; }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
