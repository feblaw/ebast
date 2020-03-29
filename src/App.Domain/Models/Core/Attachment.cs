using System.ComponentModel.DataAnnotations;

namespace App.Domain.Models.Core
{
    public class Attachment
    {
        public const string FILE_TYPE_URL = "url";
        public const string FILE_TYPE_UPLOAD = "upload";

        [Required]
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Type { get; set; }
        public double Size { get; set; }
        public string Path { get; set; }
        public string CropedPath { get; set; }
        public Crop Crop { get; set; }
        public Crop OtherInfo { get; set; }
    }

    public enum AttachmentType
    {
        File,
        Url
    }

    public class Crop
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
