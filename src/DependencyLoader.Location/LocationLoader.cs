using Dependency.Core;

namespace DependencyLoader.Location
{
    public class LocationLoader : IDependencyLoader
    {
        private readonly string _location;

        public LocationLoader(string location)
        {
            _location = location;
        }

        public void PreLoad()
        {
            // nothing to do
        }

        public string Load()
        {
            return _location;
        }

        public void PostLoad()
        {
            // nothing to do
        }
    }
}
