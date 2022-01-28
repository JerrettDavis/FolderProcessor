namespace FolderProcessor.Mediator;

public class TypesDictionary : 
    Dictionary<Type, IEnumerable<Type>>
{
    public void Add(Type key, Type value)
    {
        if (ContainsKey(key))
        {
            var values = this[key].ToList();
            
            values.Add(value);

            this[key] = values;
        }
        else
            Add(key, new List<Type> {value});
    }
}