namespace BulkRename.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Serie
    {
        [Key]
        public Guid SerieID { get; set; }

        public string? SerName { get; set; }
    }
}