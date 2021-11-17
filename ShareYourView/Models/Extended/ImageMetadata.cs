using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShareYourView.Models
{
    [MetadataType(typeof(MetaData))]
    public partial class ImageMetadata
    {
        public HttpPostedFileBase imageFile { get; set; }

        [Display(Name = "Shared By")]
        public string sharedBy { get; set; }
    }

    public class MetaData
    {        

        [Display(Name = "Longitude")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Longitude Required")]
        public double Longitude { get; set; }

        [Display(Name = "Latitude")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Latitude Required")]
        public double Latitude { get; set; }

        [Display(Name = "City")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City Required")]
        public string City { get; set; }

        [Display(Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address Required")]
        public string Address { get; set; }

        [Display(Name ="Captured Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode =true, DataFormatString ="{0:MM-dd-yyyy}")]
        public DateTime capturedData { get; set; }

        [Display(Name = "Captured By")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name Required")]
        public string capturedBy { get; set; }
                
    }
}