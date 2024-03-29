﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Exceptions.Core;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Display;

namespace Castaway.Base;

public static class CastawayGlobal
{
	public static readonly LoggingLevelSwitch LevelSwitch = new(LogEventLevel.Debug);

#if RELEASE
        private static volatile bool _ok;
        private static volatile bool _continue = true;
#endif

	private static LogEventLevel _logLevel = LogEventLevel.Information;

	private static string _consoleLogTemplate =
		"[{Level:u3} @ {Timestamp:MM/dd/yyyy HH:mm:ss.ffffff}; {SourceContext} | {ThreadName}({ThreadId})]: {Message:lj}{NewLine}{Exception}";

	public static double FrameTime { get; private set; } = 1.0 / 60.0;
	public static double Framerate => 1 / FrameTime;
	public static double DesiredFramerate => 60.0f;
	public static double FrametimeFulfillment => Framerate / DesiredFramerate;

	public static string Name => "Castaway";

	/// <summary>
	///     Creates a logger based on the calling type.
	/// </summary>
	/// <returns>The logger that was created.</returns>
	public static ILogger GetLogger()
	{
		return Log.Logger.ForContext(new StackTrace().GetFrame(1)!.GetMethod()!.DeclaringType!);
	}

	/// <summary>
	///     Runs the specified application. Should be called on the main
	///     thread.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static int Run<T>() where T : class, IApplication, new()
	{
		LoadConfig();
		Log.Logger = new LoggerConfiguration()
			.Enrich.With(new ThreadNameEnricher(), new ExceptionEnricher(), new ThreadIdEnricher())
			.WriteTo.Console(
				outputTemplate: _consoleLogTemplate,
				restrictedToMinimumLevel: _logLevel,
				standardErrorFromLevel: LogEventLevel.Error)
			.WriteTo.File(new MessageTemplateTextFormatter(
					"{Level:w} {Timestamp:HH:mm:ss.ffffff} [{ThreadName}|{ThreadId}] : {SourceContext}; {Message:lj}{NewLine}{Exception}"),
				"logs/castaway-.log",
				rollingInterval: RollingInterval.Hour)
			.WriteTo.File(new CompactJsonFormatter(), "logs/castaway-.jsonl", rollingInterval: RollingInterval.Hour)
			.MinimumLevel.ControlledBy(LevelSwitch)
			.CreateLogger();
		Thread.CurrentThread.Name = "MainThread";
		var logger = GetLogger();

		var asm = Assembly.GetExecutingAssembly();
		logger.Information("{Name} version {Version}", Name, asm.GetName().Version);

		var returnCode = 0;
#if RELEASE
            var timeKill = new Thread(TimeKill) {Name = "TimeKill"};
            timeKill.Start();
#endif

		var application = new T();
		application.Init();
		var stopwatch = new Stopwatch();
		logger.Information("Started application {@App}", application);
#if RELEASE
            _ok = true;
#endif

		try
		{
			while (!application.ShouldStop)
				try
				{
					stopwatch.Restart();
					application.StartFrame();
					application.Render();
					application.Update();
					application.EndFrame();
					Thread.Sleep(
						TimeSpan.FromMilliseconds(
							Math.Max(1000.0 / DesiredFramerate - stopwatch.Elapsed.TotalMilliseconds, 0)));
					FrameTime = stopwatch.Elapsed.TotalSeconds;
#if RELEASE
                        _ok = true;
#endif
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
			logger.Debug("Application terminating");
			application.Dispose();
#if RELEASE
                lock (_lock) _continue = false;
                timeKill.Interrupt();
#endif
			logger.Information("Goodbye");
		}

		return returnCode;
	}

	private static void LoadConfig()
	{
		using var json = JsonDocument.Parse(File.ReadAllText("config.json"));
		var root = json.RootElement;
		var eLog = root.GetProperty("log");

		if (eLog.TryGetProperty("level", out var e))
			_logLevel = TryParseEnum<LogEventLevel>(e.GetString()!, true)
			            ?? throw new InvalidOperationException($"Invalid log level: {e.GetString()}");

		if (eLog.TryGetProperty("template", out e))
			_consoleLogTemplate = e.GetString()!;
	}

	private static T? TryParseEnum<T>(string name, bool ignoreCase = false) where T : struct
	{
		return Enum.TryParse<T>(name, ignoreCase, out var t) ? t : null;
	}

#if RELEASE
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
#endif
}