using Dependency.Core;

namespace DependencyLoader.Location
{
    public class LocationLoader : IDependencyLoader
    {
        public bool Success { get; }
        public string Location { get; }

        public LocationLoader(string location)
        {
            Success = true;
            Location = location;
        }

        public void Dispose()
        {
            // nothing to do
        }
    }
}
