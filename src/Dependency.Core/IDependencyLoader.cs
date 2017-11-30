namespace Dependency.Core
{
    public interface IDependencyLoader
    {
        void PreLoad();
        string Load();
        void PostLoad();
    }
}
