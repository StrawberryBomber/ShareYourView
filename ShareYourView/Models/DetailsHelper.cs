using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace ShareYourView.Models
{
    public class DetailsHelper
    {

        public static string getMapPath()
        {
            string path = HttpContext.Current.Server.MapPath("~/GoogleDriveFiles");
            return path;
        }

        public static string getImage(GoogleDriveFile file)
        {
            string[] test = Directory.GetFiles(getMapPath());

            string image = "";
            
            foreach(var c in test)
            {
                if (c.Contains((Path.GetFileNameWithoutExtension(file.Name) + "@" + HttpContext.Current.User.Identity.Name + Path.GetExtension(file.Name))))
                {
                    image = c;
                    //Debug.WriteLine("\n\n\n\n" + c + "\n\n\n\n\n\n");
                    return image;
                }
            }
            
            return null;
        }

        public static string getSharedImage(GoogleDriveFile file)
        {
            string[] test = Directory.GetFiles(getMapPath());

            string image = "";

            foreach (var c in test)
            {
                if ( c.Contains(Path.GetFileNameWithoutExtension(file.Name) + "@") && c.Contains(Path.GetExtension(file.Name)) )
                {
                    image = c;
                    return image;
                }
            }

            return null;
        }

    }
}