using System;

namespace SampleLibrary2 {
    public record RPerson {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
    }

    public class CPerson {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    public class PocoA {
        public string? A { get; set; }
    }

    public class PocoB : PocoA {
        public string? B { get; set; }
    }

}
