namespace Orleans.Tournament.Domain.Abstractions
{
    public class StreamOptions
    {
        public string Provider { get; }

        public string Namespace { get; }

        public StreamOptions(string provider, string @namespace)
        {
            Provider = provider;
            Namespace = @namespace;
        }
    }
}