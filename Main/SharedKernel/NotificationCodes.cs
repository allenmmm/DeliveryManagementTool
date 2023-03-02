using System;

namespace DMT.SharedKernel
{
    public static class NotificationCodes
    {
        public static readonly Guid OrderbooksGeneratedOK = 
            new Guid("A09E8195-763C-4B27-AAEC-82CED2A41AC8");
        public static readonly Guid OrderbookGeneratedOK = 
            new Guid("A26292D7-BE2B-481D-AABB-09E97723A6D9");
        public static readonly Guid PandOTableDeletionError = 
            new Guid("C38FC7EC-6B56-4A05-A069-0D4847D72626");
        public static readonly Guid OrderbooksGeneratedError = 
            new Guid("6A73FE66-4384-4789-9CE1-0887A0957076");
        public static readonly Guid OrderbookGeneratedError = 
            new Guid("BCE9693D-05E0-4CD1-842F-CFE2B3A29EDA");
        public static readonly Guid PandOInvalidTableAccess = 
            new Guid("E4335E16-F8AC-4B95-8E7A-B5CEFA2FCADF");
        public static readonly Guid OrderbookGenerationStarted = 
            new Guid("6D7D561B-B477-4F7C-96BA-E3C878273AC9");
        public static readonly Guid UnableToRetrieveOrderbookWeeks = 
            new Guid("4D623D87-0AFD-44FD-9B5A-BE2D164F891E");
        public static readonly Guid UnableToRetrieveOrderbookPreviews =
            new Guid("7D625937-1CC2-4DC5-A3CE-2223B3444E18");
        public static readonly Guid UnableToRetrieveOrderbook =
            new Guid("C4FAF266-389C-45A5-9B5A-3FE6FC18562E");
    }
}
