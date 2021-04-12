using System;
using System.Collections.Generic;
using Castaway.Core;

namespace Castaway.Input
{
    /// <summary>
    /// Contains <see cref="IInputSystem{TEnum}"/> instances for reading
    /// input from various devices.
    /// </summary>
    public static class InputSystem
    {
        internal delegate void ZeroFunc();
        
        public static readonly KeyboardInputSystem Keyboard = new KeyboardInputSystem();

        internal static readonly ZeroFunc[] TickFuncs =
        {
            Keyboard.Tick
        };

        internal static readonly ZeroFunc[] ReadFuncs =
        {
            
        };
    }

    /// <summary>
    /// Module that manages the input systems provided in
    /// <see cref="InputSystem"/>.
    /// </summary>
    public class InputSystemModule : Module
    {
        protected override void PreUpdate()
        {
            base.PreUpdate();
            foreach (var func in InputSystem.ReadFuncs) func();
        }

        protected override void PostUpdate()
        {
            base.PostUpdate();
            foreach (var func in InputSystem.TickFuncs) func();
        }
    }
    
    /// <summary>
    /// Interface class for defining a method of input.
    /// </summary>
    /// <typeparam name="TEnum">Identifier type, does not have to be an enum.
    /// </typeparam>
    public interface IInputSystem<in TEnum>
    {
        /// <summary>
        /// Starts pressing a button.
        /// </summary>
        /// <param name="e">Button to start pressing.</param>
        public void TriggerButtonStart(TEnum e);
        
        /// <summary>
        /// Stops pressing a button.
        /// </summary>
        /// <param name="e">Button to stop pressing.</param>
        public void TriggerButtonStop(TEnum e);
        
        /// <summary>
        /// Checks if a button is currently pressed.
        /// </summary>
        /// <param name="e">Button to check.</param>
        /// <returns><c>true</c> if pressed, <c>false</c> otherwise.</returns>
        public bool IsPressed(TEnum e);
        
        /// <summary>
        /// Checks if a button just started being pressed.
        /// </summary>
        /// <param name="e">Button to check.</param>
        /// <returns><c>true</c> if just pressed, <c>false</c> otherwise.
        /// </returns>
        public bool IsPressedNow(TEnum e);
        
        /// <summary>
        /// Checks if a button just stopped being pressed.
        /// </summary>
        /// <param name="e">Button to check.</param>
        /// <returns><c>true</c> if not pressed anymore, <c>false</c>
        /// otherwise.</returns>
        public bool IsNoLongerPressed(TEnum e);

        /// <summary>
        /// Reads the values from the input device. Does not have to be
        /// implemented.
        /// </summary>
        public virtual void Read() {}
        
        /// <summary>
        /// Ticks this input system. Should be used to clear some flags,
        /// not read input devices.
        /// </summary>
        public void Tick();

        public virtual bool this[TEnum index] => IsPressed(index);
    }
}