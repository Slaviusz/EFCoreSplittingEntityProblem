using NodaTime;

namespace ContextClassLibrary
{
    public class Table1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Table2 Table2 { get; set; }
    }
}