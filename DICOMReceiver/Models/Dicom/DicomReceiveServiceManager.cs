using FellowOakDicom.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models.Dicom
{
    public class DicomReceiverServerManager
    {
        private IDicomServer _dicomServer;

        public void Start(int port = 104)
        {
            _dicomServer = DicomServerFactory.Create<DicomCStoreServiceProvider>(port);
        }

        public void Stop()
        {
            _dicomServer?.Dispose();
            _dicomServer = null;
        }
    }
}

