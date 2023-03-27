using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Kafka;

/// <summary>
/// Delegate for logging errors
/// </summary>
/// <param name="logger">The logger instance.</param>
/// <param name="sender">The sender that raised the error.</param>
/// <param name="error">The error instance.</param>
public delegate void ErrorHandler(ILogger logger, object sender, Error error);

/// <summary>
/// Delegate for logging messages.
/// </summary>
/// <param name="logger"></param>
/// <param name="sender"></param>
/// <param name="message"></param>
public delegate void LogHandler(ILogger logger, object sender, LogMessage message);

internal static class DefaultLogHandlers
{
    public static void HandleErrors(ILogger logger, object sender, Error error) =>
        logger.Log(LogLevel.Error, "{Type} : {Reason}", sender.GetType(), error.Reason);

    public static void HandleLogMessage(ILogger logger, object sender, LogMessage message) =>
        logger.Log(LogLevel.Information, "{Type} : {Reason}", sender.GetType(),  message.Message);
}
