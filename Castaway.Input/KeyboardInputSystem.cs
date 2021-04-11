using System;
using System.Collections.Generic;
using static Castaway.Input.Keys;

namespace Castaway.Input
{
    public enum Keys
    {
        Unknown,
        
        // Alphabet
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        
        // Numbers
        N0, N1, N2, N3, N4, N5, N6, N7, N8, N9,
        
        // F keys
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
        
        // Numpad
        P0, P1, P2, P3, P4, P5, P6, P7, P8, P9,
        PDecimal, PDivide, PMultiply, PSubtract, PAdd, PEnter, PEqual,

        // Symbols
        Apostrophe, Comma, Minus, Period, Slash, Semicolon, Equals, LeftBracket, Backslash,
        RightBracket, GraveAccent,
        
        // Control characters
        Escape, Enter, Tab, Backspace, Insert, Delete, Spacebar,
        LeftShift, LeftControl, LeftAlt, LeftSuper,
        RightShift, RightControl, RightAlt, RightSuper,
        
        // Directions
        Right, Left, Down, Up,
        
        // Other Direction keys
        PageUp, PageDown, Home, End, Pause,
        
        // Locks
        Caps, ScrollLock, NumLock,
        
        // Everything else
        PrintScreen
    }
    
    public class KeyboardInputSystem : IInputSystem<Keys>
    {
        private readonly List<Keys> _pressed = new List<Keys>();
        private readonly List<Keys> _pressedNow = new List<Keys>();
        private readonly List<Keys> _notPressed = new List<Keys>();

        public void TriggerButtonStart(Keys e)
        {
            _pressed.Add(e);
            _pressedNow.Add(e);
        }

        public void TriggerButtonStop(Keys e)
        {
            _pressed.Remove(e);
            _notPressed.Add(e);
        }

        public bool IsPressed(Keys e) => _pressed.Contains(e);
        public bool IsPressedNow(Keys e) => _pressedNow.Contains(e);
        public bool IsNoLongerPressed(Keys e) => _notPressed.Contains(e);

        public void Tick()
        {
            _pressedNow.Clear();
            _notPressed.Clear();
        }

        public unsafe void Handler(void* window, int key, int scan, int action, int mods)
        {
            var k = key switch
            {
                32 => Spacebar,
                39 => Apostrophe,
                44 => Comma,
                45 => Minus,
                46 => Period,
                47 => Slash,
                48 => N0,
                49 => N1,
                50 => N2,
                51 => N3,
                52 => N4,
                53 => N5,
                54 => N6,
                55 => N7,
                56 => N8,
                57 => N9,
                59 => Semicolon,
                61 => Keys.Equals,
                65 => A,
                66 => B,
                67 => C,
                68 => D,
                69 => E,
                70 => F,
                71 => G,
                72 => H,
                73 => I,
                74 => G,
                75 => K,
                76 => L,
                77 => M,
                78 => N,
                79 => O,
                80 => P,
                81 => Q,
                82 => R,
                83 => S,
                84 => T,
                85 => U,
                86 => V,
                87 => W,
                88 => X,
                89 => Y,
                90 => Z,
                91 => LeftBracket,
                92 => Backslash,
                93 => RightBracket,
                96 => GraveAccent,
                256 => Escape,
                257 => Enter,
                258 => Tab,
                259 => Backspace,
                260 => Insert,
                261 => Delete,
                262 => Right,
                263 => Left,
                264 => Down,
                265 => Up,
                266 => PageUp,
                267 => PageDown,
                268 => Home,
                269 => End,
                280 => Caps,
                281 => ScrollLock,
                282 => NumLock,
                283 => PrintScreen,
                284 => Pause,
                290 => F1,
                291 => F2,
                292 => F3,
                293 => F4,
                294 => F5,
                295 => F6,
                296 => F7,
                297 => F8,
                298 => F9,
                299 => F10,
                300 => F11,
                301 => F12,
                320 => P0,
                321 => P1,
                322 => P2,
                323 => P3,
                324 => P4,
                325 => P5,
                326 => P6,
                327 => P7,
                328 => P8,
                329 => P9,
                330 => PDecimal,
                331 => PDivide,
                332 => PMultiply,
                333 => PSubtract,
                334 => PAdd,
                335 => PEnter,
                336 => PEqual,
                340 => LeftShift,
                341 => LeftControl,
                342 => LeftAlt,
                343 => LeftSuper,
                344 => RightShift,
                345 => RightControl,
                346 => RightAlt,
                347 => RightSuper,
                _ => Unknown
            };
            switch (action)
            {
                case 0:
                    TriggerButtonStop(k);
                    break;
                case 1:
                    TriggerButtonStart(k);
                    break;
            }
        }
    }
}