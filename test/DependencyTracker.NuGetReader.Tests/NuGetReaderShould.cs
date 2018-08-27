using Xunit;

namespace DependencyTracker.NuGetReader.Tests
{
    public class NuGetReaderShould
    {
        private NuGetReader _nuGetReader;

        [Theory]
        [InlineData(@"C:\gitrepo\Eps\xpsdev")]
        public void Read(string path)
        {
            _nuGetReader = new NuGetReader(path);
            var dependencies = _nuGetReader.Read();
            Assert.NotEmpty(dependencies);
        }
    }
}
