namespace DependencyWriter.Mssql
{
    public class IntCounter
    {
        private int _previous;

        public IntCounter(int seed)
        {
            _previous = seed;
        }

        public int Next()
        {
            int next = _previous + 1;
            _previous++;
            return next;
        }
    }
}
