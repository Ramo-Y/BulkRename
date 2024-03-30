namespace BulkRename.Models.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public class RenamingSessionToEpisode
    {
        [Key]
        public Guid RenamingSessionToEpisodeID { get; set; }

        // ReSharper disable once InconsistentNaming
        public Guid RseRenamingSessionID_FK { get; set; }

#pragma warning disable CS8618
        [ForeignKey(nameof(RseRenamingSessionID_FK))]
        public RenamingSession RenamingSession { get; set; }

        // ReSharper disable once InconsistentNaming
        public Guid RseEpisodeID_FK { get; set; }

        [ForeignKey(nameof(RseEpisodeID_FK))]
        public Episode Episode { get; set; }
    }
}