using System.Collections.Generic;
using Castaway.Core;

namespace Castaway.Input
{
    public static class InputSystem
    {
        public static readonly KeyboardInputSystem Keyboard = new KeyboardInputSystem();
    }

    public class InputSystemModule : Module
    {
        private List<int> _waitingKeyPresses = new List<int>();
        
        protected override void PreUpdate()
        {
            base.PreUpdate();
            foreach (var press in _waitingKeyPresses)
                InputSystem.Keyboard.TriggerButtonStart((Keys) press);
        }

        protected override void PostUpdate()
        {
            base.PostUpdate();
            InputSystem.Keyboard.Tick();
        }
    }
    
    public interface IInputSystem<in TEnum>
    {
        void TriggerButtonStart(TEnum e);
        void TriggerButtonStop(TEnum e);
        bool IsPressed(TEnum e);
        bool IsPressedNow(TEnum e);
        bool IsNoLongerPressed(TEnum e);
        void Tick();

        public virtual bool this[TEnum index] => IsPressed(index);
    }
}