using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShareYourView.Models
{
    public class GoogleDriveFile
    {
        public string _pathName { get; set; }
        public string Id { get; set; }
        public long? Size { get; set; }
        public string Name { get; set; }
        public long? Version { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}