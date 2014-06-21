namespace FullTextSearchEngineTesting.InfoData
{
    class CaseResult
    {
        public decimal MaxElapsed { get; set; }
        public decimal MinElapsed { get; set; }
        public decimal AvgElasped { get; set; }
        public decimal AvgWithountFirstElasped { get; set; }
        public int MaxResults { get; set; }
        public int MinResults { get; set; }
        public decimal AvgResults { get; set; }
        public int MaxResultsWithoutFirst { get; set; }
        public int MinResultsWithoutFirst { get; set; }
        public decimal FirstElapsed { get; set; }
        public decimal LastElapsed { get; set; }
        public decimal MaxElapsedWithoutFirst { get; set; }
        public decimal CaseElapsedTime { get; set; }
    }
}