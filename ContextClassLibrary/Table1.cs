using NodaTime;

namespace ContextClassLibrary
{
    public class Table1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Instant Starts { get; set; }
        public Instant Ends { get; set; }
    }
}