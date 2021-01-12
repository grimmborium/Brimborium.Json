using System;
using System.Linq.Expressions;

namespace Brimborium.Json.Specification {
    public interface IJsonSpecification {
        JsonSpecification GetJsonSpecification(IJsonSpecificationBuilder builder);
    }

    public class JsonSpecification {
    }

    public interface IJsonSpecificationBuilder {
        JsonSpecification Build();
        IJsonSerializtaionBuilder<T> AddSerializationType<T>();
    }

    public interface IJsonSerializtaionBuilder<T> {
        IJsonSerializtaionBuilder<T> WithClassHierachie();
        IJsonSerializtaionBuilder<T> WithName(string name);
        IJsonSerializtaionBuilder<T> AddProperty<P>(Expression<Func<T, P>> expression, Action<IJsonPropertyBuilder<P>>? configure=default);
        IJsonSerializtaionBuilder<T> IgnoreProperty<P>(Expression<Func<T, P>> expression);
        IJsonSerializtaionBuilder<T> Validate();
        IJsonSpecificationBuilder Build();
    }

    public interface IJsonPropertyBuilder<P> {
        IJsonPropertyBuilder<P> WithName(string name);

        IJsonPropertyBuilder<P> WithConverter<C>()
            where C: IValueConverter<P>;

    }

    public interface IValueConverter<P> { 
    }
}
