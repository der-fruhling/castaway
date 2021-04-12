#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castaway.Assets;
using Castaway.Core;
using Castaway.Input;
using Castaway.Native;
using Castaway.Window;
using static Castaway.Assets.Properties<Castaway.Exec.Cast.CastProperty>;
using static Castaway.Exec.Cast.CastProperty;

namespace Castaway.Exec
{
    /// <summary>
    /// Main execution class for Castaway. Supports loading separate
    /// assemblies for execution, along with some attributes.
    /// </summary>
    public static class Cast
    {
        /// <summary>
        /// Properties used in Cast.properties.txt. This file is required
        /// and tells Cast how some things need to be set up.
        /// </summary>
        public enum CastProperty
        {
            DefaultWindowWidth,
            DefaultWindowHeight,
            WindowTitle,
            Fullscreen,
        }

        /// <summary>
        /// Stores all properties in Cast.properties.txt.
        /// </summary>
        /// <seealso cref="CastProperty"/>
        public static Properties<CastProperty>? Properties;

        /// <summary>
        /// Asset index of the <c>Cast.properties.txt</c> file. Initialized
        /// by <see cref="SetupProperties"/>, in <see cref="Events.PreInit"/>.
        /// </summary>
        public static int PropertiesAsset { get; private set; }
        
        /// <summary>
        /// Main GLFW window opened by Cast. Window parameters are configured
        /// in the properties.
        /// </summary>
        /// <seealso cref="GLFWWindow"/>
        public static GLFWWindow? Window;

        /// <summary>
        /// Enables debug rendering.
        /// </summary>
        /// TODO
        public static bool DebugMode;

        /// <summary>
        /// Sets up the property settings and loads the Cast.properties.txt
        /// file.
        /// </summary>
        /// <seealso cref="Start"/>
        private static void SetupProperties()
        {
            var settings = new Dictionary<CastProperty, Settings>
            {
                [DefaultWindowWidth] = new Settings(PropertyReaders.Int32),
                [DefaultWindowHeight] = new Settings(PropertyReaders.Int32),
                [WindowTitle] = new Settings(PropertyReaders.String)
            };
            Properties = new Properties<CastProperty>(settings);
            PropertiesAsset = AssetManager.Index("/Cast.properties.txt");
            Properties.Load(AssetManager.Get<string>(PropertiesAsset)!.Split('\n'));
        }
        
        /// <summary>
        /// Opens the window used by Castaway.
        /// </summary>
        /// <seealso cref="Window"/>
        /// <seealso cref="Start"/>
        private static void SetupWindow()
        {
            GLFWWindow.Init();
            Window = GLFWWindow.Windowed(
                Properties!.Get<int>(DefaultWindowWidth),
                Properties.Get<int>(DefaultWindowHeight),
                Properties.Get<string>(WindowTitle));
            Events.Finish += DrawDebugMode;
            Events.Finish += Window.Finish;
            Events.ShouldClose = () => Window.ShouldClose;
            unsafe { Window.KeyCallback = InputSystem.Keyboard.Handler; }
        }

        private static void DrawDebugMode()
        {
            if(!DebugMode) return;
            // TODO
        }
        
        /// <summary>
        /// Sets up some events and starts Castaway.
        /// </summary>
        public static async Task Start()
        {
            await Task.Run(() =>
            {
                Events.PreInit += SetupProperties;
                Events.PreInit += SetupWindow;
                Events.CloseNormally += GLFW.glfwTerminate;
                Events.Loop();
            });
        }

        private static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: cast <startup.dll>");
                return 1;
            }

            var asm = Assembly.LoadFile(args[0]);
            var types = asm.GetTypes();
            var entrypoints = types.Where(type => type.CustomAttributes.Any(a => a.AttributeType == typeof(EntrypointAttribute)));
            var moduleRequiring = types.Where(type => type.CustomAttributes.Any(a => a.AttributeType == typeof(RequiresModulesAttribute)));

            var ep = entrypoints as Type[] ?? entrypoints.ToArray();
            if (!ep.Any())
            {
                Console.Error.WriteLine($"{args[0]} does not contain any entrypoints.");
                return 2;
            }

            // Entrypoint processor
            foreach (var entrypoint in ep)
            {
                var con = entrypoint.GetConstructor(new Type[0]);
                if (con == null)
                {
                    Console.Error.WriteLine($"Entrypoint {entrypoint.FullName} does not contain a parameterless constructor.");
                    return 3;
                }

                var obj = con.Invoke(null);

                // Method Entrypoints
                var epm = entrypoint.GetMethods().Where(m => m.CustomAttributes.Any(a =>
                    a.AttributeType == typeof(EntrypointAttribute)));
                foreach (var m in epm)
                {
                    var parameters = m.GetParameters();
                    var paramList = parameters.Select(p => p.Name! switch
                        {
                            "args" when p.ParameterType == typeof(string[]) => args[1..],
                            _ => throw new ArgumentOutOfRangeException()
                        })
                        .Cast<object?>()
                        .ToList();
                    m!.Invoke(obj, paramList.Count == 0 ? null : paramList.ToArray());
                }
                
                // Event Handlers
                var ehm = entrypoint.GetMethods().Where(m => m.CustomAttributes.Any(a =>
                    a.AttributeType == typeof(EventHandlerAttribute)));
                foreach (var m in ehm)
                {
                    if (m!.GetParameters().Length > 0 || m!.ReturnType != typeof(void))
                    {
                        Console.Error.WriteLine("Error while processing event handlers:\n" +
                                               $"<In: {entrypoint.FullName}.{m.Name}>\n" +
                                                "Invalid signature. Must use void return type" +
                                                "and 0 parameters for handlers.");
                        return 4;
                    }

                    var type = m.GetCustomAttribute<EventHandlerAttribute>()!.EventType;
                    if (type == EventType.UseMethodName)
                        if (!Enum.TryParse(m.Name, out type))
                            throw new ApplicationException($"Invalid EventType name: {m.Name}");
                    
                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (type)
                    {
                        case EventType.PreInit: Events.PreInit += () => m.Invoke(obj, null); break;
                        case EventType.Init: Events.Init += () => m.Invoke(obj, null); break;
                        case EventType.PostInit: Events.PostInit += () => m.Invoke(obj, null); break;
                        case EventType.PreDraw: Events.PreDraw += () => m.Invoke(obj, null); break;
                        case EventType.Draw: Events.Draw += () => m.Invoke(obj, null); break;
                        case EventType.PostDraw: Events.PostDraw += () => m.Invoke(obj, null); break;
                        case EventType.PreUpdate: Events.PreUpdate += () => m.Invoke(obj, null); break;
                        case EventType.Update: Events.Update += () => m.Invoke(obj, null); break;
                        case EventType.PostUpdate: Events.PostUpdate += () => m.Invoke(obj, null); break;
                        case EventType.Finish: Events.Finish += () => m.Invoke(obj, null); break;
                        default: throw new ApplicationException($"Bad event type: {type}");
                    }
                }
            }
            
            // Module requiring
            foreach (var type in moduleRequiring)
            {
                var attrs = type.GetCustomAttributes<RequiresModulesAttribute>()!;
                foreach(var attr in attrs) 
                    foreach(var m in attr.Modules)
                        CModules.Load(m);
            }

            Start().Wait();

            return 0;
        }
    }
}
