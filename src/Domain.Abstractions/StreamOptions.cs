namespace Orleans.Tournament.Domain.Abstractions
{
    public class StreamOptions
    {
        public string Name { get; }

        public string Namespace { get; }

        public StreamOptions(string name, string @namespace)
        {
            Name = name;
            Namespace = @namespace;
        }
    }
}
