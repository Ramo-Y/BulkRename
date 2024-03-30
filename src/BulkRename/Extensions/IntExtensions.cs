namespace BulkRename.Extensions
{
    internal static class IntExtensions
    {
        internal static int ToMilliseconds(this int seconds)
        {
            var milliseconds = seconds * 1000;
            return milliseconds;
        }
    }
}