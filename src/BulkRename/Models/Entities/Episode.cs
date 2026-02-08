namespace BulkRename.Models.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public class Episode
    {
        [Key]
        public Guid EpisodeID { get; set; }

#pragma warning disable CS8618
        public string? EpsEpisodeOriginalName { get; set; }

        public string? EpsEpisodeNewName { get; set; }

        public string? EpsNumberString { get; set; }

        // ReSharper disable once InconsistentNaming
        public Guid EpsSeasonID_FK { get; set; }

        [ForeignKey(nameof(EpsSeasonID_FK))]
        public Season Season { get; set; }
    }
}