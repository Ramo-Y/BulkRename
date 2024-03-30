namespace BulkRename.Models.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public class Season
    {
        [Key]
        public Guid SeasonID { get; set; }

#pragma warning disable CS8618
        public string SsnNumberString { get; set; }

        // ReSharper disable once InconsistentNaming
        public Guid SsnSerieID_FK { get; set; }

        [ForeignKey(nameof(SsnSerieID_FK))]
        public Serie Serie { get; set; }
    }
}