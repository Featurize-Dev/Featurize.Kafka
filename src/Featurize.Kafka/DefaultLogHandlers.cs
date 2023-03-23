using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Kafka;

public delegate void ErrorHandler(ILogger logger, object sender, Error error);
public delegate void LogHandler(ILogger logger, object sender, LogMessage message);

internal static class DefaultLogHandlers
{
    public static void HandleErrors(ILogger logger, object sender, Error error) =>
        logger.Log(LogLevel.Error, "{Type} : {Reason}", sender.GetType(), error.Reason);

    public static void HandleLogMessage(ILogger logger, object sender, LogMessage message) =>
        logger.Log(LogLevel.Information, "{Type} : {Reason}", sender.GetType(),  message.Message);
}
