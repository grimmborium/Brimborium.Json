using System;

namespace SampleLibrary2 {
    public record Person {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
    }

    public class PocoA { 
        public string? A { get; set; }
    }

    public class PocoB : PocoA {
        public string? B { get; set; }
    }

}
