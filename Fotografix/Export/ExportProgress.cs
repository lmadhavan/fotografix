namespace Fotografix.Export
{
    public struct ExportProgress
    {
        public int TotalItems { get; set; }
        public int ProcessedItems { get; set; }
        public int FailedItems { get; set; }
    }
}
