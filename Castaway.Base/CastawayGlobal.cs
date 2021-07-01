﻿using System;
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

        private static volatile object _lock = new();
        private static volatile bool _ok = true;
        private static volatile bool _continue = true;
        public static double FrameTime { get; private set; } = 1.0 / 60.0;
        public static double Framerate => 1 / FrameTime;
        public static double RealFrameTime { get; private set; }

        public static ILogger GetLogger(Type type)
        {
            return Log.Logger.ForContext(type);
        }

        public static ILogger GetLogger()
        {
            return GetLogger(new StackTrace().GetFrame(1)!.GetMethod()!.DeclaringType!);
        }

        public static int Run<T>() where T : class, IApplication, new()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadNameEnricher(), new ExceptionEnricher(), new ThreadIdEnricher())
                .WriteTo.Console(
                    outputTemplate:
                    "[{Level:u3} @ {Timestamp:HH:mm:ss.ffffff}; {SourceContext}]: {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("castaway.log",
                    outputTemplate:
                    "{Level:w} {Timestamp:HH:mm:ss.ffffff} [{ThreadName}|{ThreadId}] : {SourceContext}; {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(new CompactJsonFormatter(), "castaway.log.jsonl", LogEventLevel.Debug)
                .MinimumLevel.ControlledBy(LevelSwitch)
                .CreateLogger();
            Thread.CurrentThread.Name = "MainThread";
            var logger = GetLogger();

            logger.Verbose("a");
            logger.Debug("b");
            logger.Information("c");
            logger.Warning("d");
            logger.Error("e");
            logger.Fatal("f");

            logger.Information("Hello");

            var returnCode = 0;
            var timeKill = new Thread(TimeKill) {Name = "TimeKill"};
            timeKill.Start();

            LoadConfig(logger);

            var application = new T();
            application.Init();
            var stopwatch = new Stopwatch();
            logger.Information("Started application {@App}", application);
            _ok = true;

            try
            {
                while (!application.ShouldStop)
                    try
                    {
                        application.StartFrame();
                        stopwatch.Restart();
                        application.Render();
                        application.Update();
                        var r = stopwatch.Elapsed.TotalSeconds;
                        application.EndFrame();
                        Thread.Sleep((int) Math.Max(16 - stopwatch.ElapsedMilliseconds, 0));
                        RealFrameTime = r;
                        FrameTime = stopwatch.Elapsed.TotalSeconds;
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
            catch (Exception e)
            {
                logger.Fatal(e, "An error occurred during execution");
                returnCode = 1;
            }
            finally
            {
                logger.Information("Application terminating");
                application.Dispose();
                lock (_lock) _continue = false;
                timeKill.Interrupt();
            }

            logger.Information("Goodbye");
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

        private static JsonElement? OptionalProperty(this JsonElement e, string name)
        {
            return e.TryGetProperty(name, out var r) ? r : null;
        }

        private static T? TryParseEnum<T>(string name, bool ignoreCase = false) where T : struct
        {
            return Enum.TryParse<T>(name, ignoreCase, out var t) ? t : null;
        }

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

        private static Task TryAsync(ILogger logger, Action a)
        {
            return Task.Run(() => Try(logger, a));
        }

        private static void TimeKill()
        {
            var logger = GetLogger();
            try
            {
                while (_continue)
                {
                    Thread.Sleep(50);
                    if (!_continue) return;
                    if (_ok)
                    {
                        lock (_lock) _ok = false;
                        continue;
                    }

                    Thread.Sleep(9950);
                    if (!_continue) return;
                    if (_ok)
                    {
                        lock (_lock) _ok = false;
                        continue;
                    }

                    logger.Warning("Haven't heard from Main Thread in 10 seconds!");

                    Thread.Sleep(20000);
                    if (!_continue) return;
                    if (_ok)
                    {
                        logger.Information("Recovered from silence");
                        lock (_lock) _ok = false;
                        continue;
                    }

                    logger.Warning("Haven't heard from Main Thread in 30 seconds!");

                    Thread.Sleep(30000);
                    if (!_continue) return;
                    if (_ok)
                    {
                        logger.Information("Recovered from silence");
                        lock (_lock) _ok = false;
                        continue;
                    }

                    logger.Error("Timeout!");
                    Environment.Exit(2);
                }
            }
            catch (ThreadInterruptedException)
            {
            }
        }
    }
}