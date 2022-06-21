namespace Flagship.Core.Interfaces {
    public interface IApplogger {
        public void LogInfo(string message);
        public void LogDebug(string message);
        public void LogVerbose(string message);
        public void LogException(Exception ex);
        public void LogExceptionWithSource(Exception ex, string source);
        public void LogEnter();
        public void LogExit();
    }
}
