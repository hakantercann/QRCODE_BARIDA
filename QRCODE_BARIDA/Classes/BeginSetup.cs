using AForge.Video.DirectShow;
using System.Collections.ObjectModel;

namespace QRCODE_BARIDA.Classes
{
    public class BeginSetup
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;
        ObservableCollection<FilterInfo> videoCaptureDevices;
        
        public BeginSetup() 
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoCaptureDevices = new ObservableCollection<FilterInfo>();
            foreach(FilterInfo item in filterInfoCollection)
            {
                videoCaptureDevices.Add(item);
            }
            
            videoCaptureDevice = new VideoCaptureDevice(videoCaptureDevices[0].MonikerString);
            if(videoCaptureDevice != null)
            {
                TemporaryMemory.VideoCaptureDevice = videoCaptureDevice;
                TemporaryMemory.ScannerType = ScannerType.webcam;
                TemporaryMemory.Roles = Roles.Admin;
                LastLogin.isLoginned = false;
            }

                
        }
    }
}
