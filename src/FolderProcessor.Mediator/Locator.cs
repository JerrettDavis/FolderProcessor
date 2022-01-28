using System.Reflection;
using FolderProcessor.Common.Exceptions;
using MyMediator = FolderProcessor.Abstractions.Mediator;

namespace FolderProcessor.Mediator;

public static class Locator
{
    public static IDictionary<Type, IEnumerable<Type>> 
        GetInternalMediatorImplementations()
    {
        var types = new List<Type>
        {
            typeof(MyMediator.IMediator),
            typeof(MyMediator.IPublisher),
            typeof(MyMediator.IRequest<>),
            typeof(MyMediator.IRequestHandler<,>),
            typeof(MyMediator.ISender),
            typeof(MyMediator.IStreamRequest<>),
            typeof(MyMediator.IStreamRequestHandler<,>)
        };
        var assemblies = new List<Assembly>
        {
            typeof(AddToStoreException).Assembly,
            typeof(Locator).Assembly
        };
        var foundTypes = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => t.GetInterfaces().Any(i => types.Contains(i)))
            .ToList();
        var typeDict = new TypesDictionary();
        
        foreach (var type in types)
        {
            var matching = foundTypes.Where(t => t.GetInterfaces().Contains(type));
            typeDict.Add(type, matching);
        }

        return typeDict;
    }
}