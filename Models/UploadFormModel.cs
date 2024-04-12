using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace SmartSheetLoader.Models
{
    public class UploadFormModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public IBrowserFile File { get; set; }
    }
}
