using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace ContextClassLibrary
{
    public class Table2
    {
        [Key] public int Table1Id { get; set; }
        public Instant Starts { get; set; }
        public Instant Ends { get; set; }
        public virtual Table1 Table1 { get; set; }
    }
}