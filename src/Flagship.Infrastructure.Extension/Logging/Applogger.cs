using Flagship.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

namespace Flagship.Infrastructure.Logging.Applogger {
    public class Applogger : IApplogger {
        private readonly ILogger<Applogger> _logger;
        public Applogger(ILogger<Applogger> logger) {
            _logger = logger;
        }
        public void LogVerbose(string message) {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Information))
                _logger.LogInformation(message);
        }
        public void LogInfo(string message) {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Information))
                _logger.LogInformation(message);
        }
        public void LogException(Exception ex) {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Error))
                _logger.LogError(ex.ToString());
        }
        public void LogExceptionWithSource(Exception ex, string source) {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Error))
                _logger.LogError(source + " : " + ex.ToString());
        }
        public void LogDebug(string message) {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                _logger.LogDebug(message);
        }
        public void LogEnter() {
            if (!Log.IsEnabled(Serilog.Events.LogEventLevel.Debug)) return;
            var trace = new StackTrace();
            if (trace.FrameCount <= 1) return;
            var method = trace.GetFrame(1)?.GetMethod();
            if (method?.DeclaringType != null)
                LogDebug($"Entering {method.DeclaringType.Name}.{method.Name}");
        }

        public void LogExit() {
            if (!Log.IsEnabled(Serilog.Events.LogEventLevel.Debug)) return;
            var trace = new StackTrace();
            if (trace.FrameCount <= 1) return;
            var method = trace.GetFrame(1)?.GetMethod();
            if (method?.DeclaringType != null)
                LogDebug($"Exiting {method.DeclaringType.Name}.{method.Name}");
        }
    }
}

