using Dependency.Core;

namespace Dependency.Loader
{
    public class LocationLoader : IDependencyLoader
    {
        private readonly string _location;

        public LocationLoader(string location)
        {
            _location = location;
        }

        public string Load()
        {
            return _location;
        }
    }
}
