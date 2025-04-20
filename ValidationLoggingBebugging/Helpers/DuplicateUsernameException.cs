namespace ValidationLoggingBebugging.Helpers
{
	public class DuplicateUsernameException : Exception
	{
		public DuplicateUsernameException(string message) : base(message) { }
        public DuplicateUsernameException():base() {}
    }
}
