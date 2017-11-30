//using System.Linq;
//using Dependency.Core;
//using Xunit;

//namespace Dependency.Reader.Tests
//{
//    public class NuGetReaderShould
//    {
//        private readonly IDependencyReader _reader;

//        public NuGetReaderShould()
//        {
//            _reader = new NuGetReader("");
//        }

//        [Fact]
//        public void GetDependencies()
//        {
//            const string FILENAME = "C:\\Users\\vermeulenl\\Documents\\Visual Studio 2017\\Projects\\JiraCli\\JiraEngine\\packages.config";
//            var results = _reader.Read(nameof(NuGetReaderShould), FILENAME);

//            Assert.NotNull(results);
//            Assert.NotNull(results.First().DependencyId);
//        }
//    }
//}
