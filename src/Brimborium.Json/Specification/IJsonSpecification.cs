using System;
using System.Linq.Expressions;

namespace Brimborium.Json.Specification {
    public interface IJsonSpecification {
    }
    public interface IJsonSpecificationBuilder{
        IJsonTypeSpecificationBuilder<T> AddSerializationType<T>();
        JsonSpecification BuildSpecification();
    }
    public interface IJsonTypeSpecificationBuilder<T> {
        IJsonTypeSpecificationBuilder<T> WithName(string jsonName);

        IJsonTypeSpecificationBuilder<T> WithClassHierachie();
        
        IJsonTypeSpecificationBuilder<T> AddProperty<P>(
            Expression<Func<T, P>> getter,
            Action<IJsonPropertySpecificationBuilder<T,P>>? configure = default             
            );

        IJsonTypeSpecificationBuilder<T> IgnoreProperty<P>(
            Expression<Func<T, P>> getter
        );

        IJsonSpecificationBuilder BuildType();
    }

    public interface IJsonPropertySpecificationBuilder<T,P> {
        IJsonPropertySpecificationBuilder<T,P> WithName(string jsonName);
    }
    
    public class JsonSpecification{

    }
}
    