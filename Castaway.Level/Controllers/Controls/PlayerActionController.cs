using System.Collections.Generic;
using Castaway.Core;
using Castaway.Input;

namespace Castaway.Levels.Controllers.Controls
{
    [ControllerInfo(Name = "Player Action Reactor")]
    public class PlayerActionController : Controller
    {
        public delegate void KeyPressAction(LevelObject obj, Keys key);

        private readonly Dictionary<Keys, KeyPressAction> _keyPressActions = new Dictionary<Keys, KeyPressAction>();
        private readonly Dictionary<Keys, KeyPressAction> _keyPressNowActions = new Dictionary<Keys, KeyPressAction>();
        private readonly Dictionary<Keys, KeyPressAction> _keyUnpressedActions = new Dictionary<Keys, KeyPressAction>();

        public Dictionary<Keys, object> KeyPressed = new Dictionary<Keys, object>();
        public Dictionary<Keys, object> KeyPressedNow = new Dictionary<Keys, object>();
        public Dictionary<Keys, object> KeyUnpressed = new Dictionary<Keys, object>();

        public PlayerActionController()
        {
            _keyPressActions[Keys.W] = (m, _) => m.Position.Z += .075f;
            _keyPressActions[Keys.S] = (m, _) => m.Position.Z -= .075f;
            _keyPressActions[Keys.D] = (m, _) => m.Position.X += .075f;
            _keyPressActions[Keys.A] = (m, _) => m.Position.X -= -.075f;
        }

        public void Reset()
        {
            _keyPressActions.Clear();
            _keyPressNowActions.Clear();
            _keyUnpressedActions.Clear();
        }

        public void OnPressed(Keys key, KeyPressAction a) => _keyPressActions[key] = a;
        public void OnPressedNow(Keys key, KeyPressAction a) => _keyPressNowActions[key] = a;
        public void OnUnpressed(Keys key, KeyPressAction a) => _keyUnpressedActions[key] = a;

        public override void OnBegin()
        {
            base.OnBegin();
            foreach (var (key, act) in KeyPressed)
            {
                switch (act)
                {
                    case string actionRef:
                    {
                        var parts = actionRef.Split("::");
                        var type = CastawayCore.StartupAssembly.GetType(parts[0]);
                        var method = type!.GetMethod(parts[1], new[] {typeof(LevelObject), typeof(Keys)});
                        OnPressed(key, method!.CreateDelegate(typeof(KeyPressAction)) as KeyPressAction);
                        break;
                    }
                    case KeyPressAction keyPressAction:
                        OnPressed(key, keyPressAction);
                        break;
                }
            }
            foreach (var (key, act) in KeyPressedNow)
            {
                switch (act)
                {
                    case string actionRef:
                    {
                        var parts = actionRef.Split("::");
                        var type = CastawayCore.StartupAssembly.GetType(parts[0]);
                        var method = type!.GetMethod(parts[1], new[] {typeof(LevelObject), typeof(Keys)});
                        OnPressedNow(key, method!.CreateDelegate(typeof(KeyPressAction)) as KeyPressAction);
                        break;
                    }
                    case KeyPressAction keyPressAction:
                        OnPressedNow(key, keyPressAction);
                        break;
                }
            }
            foreach (var (key, act) in KeyUnpressed)
            {
                switch (act)
                {
                    case string actionRef:
                    {
                        var parts = actionRef.Split("::");
                        var type = CastawayCore.StartupAssembly.GetType(parts[0]);
                        var method = type!.GetMethod(parts[1], new[] {typeof(LevelObject), typeof(Keys)});
                        OnUnpressed(key, method!.CreateDelegate(typeof(KeyPressAction)) as KeyPressAction);
                        break;
                    }
                    case KeyPressAction keyPressAction:
                        OnUnpressed(key, keyPressAction);
                        break;
                }
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            foreach (var (key, action) in _keyPressActions)
                if (InputSystem.Keyboard.IsPressed(key)) action(parent, key);
            foreach (var (key, action) in _keyPressNowActions)
                if (InputSystem.Keyboard.IsPressedNow(key)) action(parent, key);
            foreach (var (key, action) in _keyUnpressedActions)
                if (InputSystem.Keyboard.IsNoLongerPressed(key)) action(parent, key);
        }
    }
}