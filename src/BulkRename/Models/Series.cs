namespace BulkRename.Models
{
    public class Series
    {
#pragma warning disable CS8618 
        public string OldName { get; set; }

        public string NewName { get; set; }

        public double FileSizeInMb { get; set; }
    }
}