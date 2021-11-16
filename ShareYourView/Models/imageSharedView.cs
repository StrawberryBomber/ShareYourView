using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShareYourView.Models
{
    public class imageSharedView
    {
        public ImageMetadata imgData {get;set;}
        public List<UserDetail> userData { get; set; }

    }
}