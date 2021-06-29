using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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

        public static ILogger GetLogger(Type type) => Log.Logger.ForContext(type);
        public static ILogger GetLogger() => GetLogger(new StackTrace().GetFrame(1)!.GetMethod()!.DeclaringType!);

        public static int Run<T>() where T : class, IApplication, new()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadNameEnricher(), new ExceptionEnricher(), new ThreadIdEnricher())
                .WriteTo.Console(outputTemplate: "[{Level:u4} {Timestamp:HH:mm:ss.ffffff} {SourceContext}]: {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("castaway.log", outputTemplate: "{Level:w4} {Timestamp:HH:mm:ss.ffffff} [{ThreadName} {ThreadId}] : {SourceContext}; {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(new CompactJsonFormatter(), "castaway.log.jsonl", LogEventLevel.Debug)
                .MinimumLevel.ControlledBy(LevelSwitch)
                .CreateLogger();
            Thread.CurrentThread.Name = "MainThread";
            var returnCode = 0;
            var logger = GetLogger();

            LoadConfig(logger);

            var application = new T();
            application.Init();
            logger.Information("Started application {@App}", application);
            try
            {
                while (!application.ShouldStop)
                {
                    try
                    {
                        ProcessFrame(application);
                    }
                    catch (RecoverableException e)
                    {
                        logger.Error(e, "A recoverable error occurred; frame will be passed");
                        application.Recover(e);
                    }
                    catch (MessageException e)
                    {
                        e.Log(logger);
                        e.Repair(logger);
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

        private static void LoadConfig(ILogger logger)
        {
            using var json = JsonDocument.Parse(File.ReadAllText("config.json"));
            var root = json.RootElement;
            var eLog = root.GetProperty("log");
            
            Try(logger, delegate
            {
                var eLogLevelS = eLog.OptionalProperty("level")?.GetString() 
                                 ?? Enum.GetName(LogEventLevel.Information)!;
                var eLogLevel = TryParseEnum<LogEventLevel>(eLogLevelS, true);
                LevelSwitch.MinimumLevel = eLogLevel 
                                           ?? throw new EnumParseException(
                                               "/log/level", eLogLevelS, typeof(LogEventLevel),
                                               () => LevelSwitch.MinimumLevel = LogEventLevel.Information,
                                               LogEventLevel.Information);
                logger.Verbose("LogLevel = {Value}", eLogLevel);
            });
            
            logger.Debug("Loaded config from config.json");
        }

        private static void ProcessFrame(IApplication application)
        {
            application.StartFrame();
            application.Render();
            application.Update();
            application.EndFrame();
        }

        private static JsonElement? OptionalProperty(this JsonElement e, string name) => 
            e.TryGetProperty(name, out var r) ? r : null;

        private static T? TryParseEnum<T>(string name, bool ignoreCase = false) where T : struct =>
            Enum.TryParse<T>(name, ignoreCase, out var t) ? t : null;

        private static void Try(ILogger logger, Action a)
        {
            try
            {
                a();
            }
            catch (MessageException e)
            {
                if (e.IsFatal) throw;
                e.Log(logger);
                e.Repair(logger);
            }
            catch (Exception e)
            {
                logger.Error("{Type} occurred in config stage: /log/level", e.GetType());
                throw;
            }
        }

        private static Task TryAsync(ILogger logger, Action a) => Task.Run(() => Try(logger, a));
    }
}