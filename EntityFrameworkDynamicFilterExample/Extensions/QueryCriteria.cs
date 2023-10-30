namespace EntityFrameworkDynamicFilterExample.Extensions
{
    public class QueryCriteria
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public List<Filters>? Filters { get; set; }
    }

    public class Filters
    {
        public string PropertyName { get; set; }
        public Operator Operator { get; set; }
        public string Value { get; set; }
    }

    public enum Operator
    {
        Equal,
        GreaterThanOrEqual,
        LessThanOrEqual,
        GreaterThan,
        LessThan,
        NotEqual,
        Contains
    }
}