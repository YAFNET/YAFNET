namespace YAF.Types.Models
{
    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces.Data;

    [Serializable]
    public partial class Attachment : IEntity, IHaveID
    {
        partial void OnCreated();

        public Attachment()
        {
            this.OnCreated();
        }

        #region Properties

        [AutoIncrement]
        [Alias("AttachmentID")]
        public int ID { get; set; }

        public int MessageID { get; set; }

        public string FileName { get; set; }

        public int Bytes { get; set; }

        public int? FileID { get; set; }

        public string ContentType { get; set; }

        public int Downloads { get; set; }

        public byte[] FileData { get; set; }


        #endregion
    }
}