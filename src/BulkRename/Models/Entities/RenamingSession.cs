namespace BulkRename.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class RenamingSession
    {
        [Key]
        public Guid RenamingSessionID { get; set; }

#pragma warning disable CS8618
        public string? RenName { get; set; }

        public DateTime RenExecutingDateTime { get; set; }

        public bool RenRenamingSessionIsOk { get; set; }
    }
}