using Main.Mvc.Models;
using System;

namespace Main.Mvc.DBModels
{
    public class MediaDetails: EntityBase
    {
        //////public string id { get; set; }
        public string youtubeURL { get; set; }
        public string ImageURL { get; set; }

        public string FileUploadedPath { get; set; }
        public Int16 UploadType { get; set; }
        public Guid  CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class DisplayMedia: MediaDetails
    {
        public bool TipSaved { get; set; }
    }

    public class MediaTip : EntityBase
    {
      public Guid MediaDetailsId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public enum UploadType
    {
        YoutubeUrl,
        ImageURL,
        FileUpload
    }
}
