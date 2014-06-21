using System;

namespace FullTextSearchEngineTesting.InfoData
{
    class Result:IComparable<Result>
    {
        public decimal ElapsedTime { get; set; }
        public int TotalFound { get; set; }

        public int CompareTo(Result other)
        {
            return ElapsedTime.CompareTo(other.ElapsedTime);
        }
    }
}