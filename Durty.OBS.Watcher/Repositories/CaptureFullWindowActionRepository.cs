using System;
using System.Collections.Generic;
using System.Text;
using Durty.OBS.Watcher.Models;

namespace Durty.OBS.Watcher.Repositories
{
    public class CaptureFullWindowActionRepository
    {
        private List<CaptureFullWindowAction> _data = new List<CaptureFullWindowAction>()
        {
            new CaptureFullWindowAction()
            {
                CaptureWindowTitle = "Microsoft Visual Studio",
                DisplayCaptureSourceName = "All IDE Capture",
                NeededWindowFocusTime = 3
            }
        };

        public CaptureFullWindowActionRepository()
        {
            
        }

        public List<CaptureFullWindowAction> GetAll()
        {
            return _data;
        }
    }
}
