using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Json.Specification {
    public interface IJsonSpecification {
        ProjectSpecification GetSpecification(SpecificationContext ctxt);
    }
}
