namespace BulkRename.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Serie
    {
        [Key]
        public Guid SerieID { get; set; }

#pragma warning disable CS8618
        public string SerName { get; set; }
    }
}