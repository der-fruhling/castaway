using System;
using System.Diagnostics;
using System.Threading;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Exceptions.Core;
using Serilog.Formatting.Compact;

namespace Castaway.Base
{
    public static class CastawayGlobal
    {
        public static readonly LoggingLevelSwitch LevelSwitch = new(LogEventLevel.Debug);
        
        private static readonly Logger Logger = new LoggerConfiguration()
            .Enrich.With(new ThreadNameEnricher(), new ExceptionEnricher())
            .WriteTo.Console(outputTemplate: "[{Level:u3} {Timestamp:HH:mm:ss.fff} {SourceContext}]: {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("castaway.log", outputTemplate: "{Level} {Timestamp:HH:mm:ss.fff} [{ThreadName}] [{SourceContext}]; {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(new CompactJsonFormatter(), "castaway.log.jsonl", LogEventLevel.Debug)
            .MinimumLevel.ControlledBy(LevelSwitch)
            .CreateLogger();

        public static ILogger GetLogger(Type type) => Logger.ForContext(type);
        public static ILogger GetLogger() => GetLogger(new StackTrace().GetFrame(1)!.GetMethod()!.DeclaringType!);

        public static int Run<T>() where T : class, IApplication, new()
        {
            Thread.CurrentThread.Name = "MainThread";
            var returnCode = 0;
            var logger = GetLogger();
            var application = new T();
            logger.Information("Started application {@App}", application);
            try
            {
                while (!application.ShouldStop)
                {
                    try
                    {
                        application.StartFrame();
                        application.Render();
                        application.Update();
                        application.EndFrame();
                    }
                    catch (RecoverableException e)
                    {
                        logger.Error(e, "A recoverable error occurred; frame will be passed");
                        application.Recover(e);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Fatal(e, "An error occurred during execution");
                returnCode = 1;
            }
            finally
            {
                logger.Information("Application terminating");
                application.Dispose();
            }
            return returnCode;
        }
    }
}