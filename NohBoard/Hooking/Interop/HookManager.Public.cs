﻿/*
Copyright (C) 2016 by Eric Bataille <e.c.p.bataille@gmail.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace ThoNohT.NohBoard.Hooking.Interop
{
    using SharpDX.DirectInput;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Threading;
    using static Defines;
    using static FunctionImports;

    /// <summary>
    /// The public interface for the <see cref="HookManager"/> class.
    /// </summary>
    public static partial class HookManager
    {
        #region Properties

        /// <summary>
        /// This property is set from outside this class. If <c>true</c>, <see cref="TrapToggleKeyCode"/> can toggle
        /// the mouse trap.
        /// </summary>
        public static bool TrapMouse { get; set; }

        /// <summary>
        /// This property is set from outside this class. If <c>true</c>, <see cref="TrapToggleKeyCode"/> key can
        /// toggle the keyboard trap.
        /// </summary>
        public static bool TrapKeyboard { get; set; }

        /// <summary>
        /// If this property is set, every key-code processed from the keyboard will be passed through this function.
        /// If the function returns true, the keycode is then trapped.
        /// </summary>
        public static Func<int, bool> KeyboardInsert = null;

        /// <summary>
        /// If this property is set, every button processed from the joystick will be passed through this function.
        /// If the function returns true, the keycode is then trapped.
        /// </summary>
        public static Func<int, bool> DirectInputButtonInsert = null;

        /// <summary>
        /// If this property is set, every axis used from the joystick will be passed through this function.
        /// If the function returns true, the keycode is then trapped.
        /// </summary>
        public static Func<string, bool> DirectInputAxisInsert = null;

        /// <summary>
        /// If this property is set, every dpad used from the joystick will be passed through this function.
        /// If the function returns true, the keycode is then trapped.
        /// </summary>
        public static Func<int, bool> DirectInputDpadInsert = null;

        /// <summary>
        /// The GUID of the currently selected device
        /// </summary>
        public static Guid CurrentDevideGuid = Guid.Empty;

        /// <summary>
        /// The keycode that toggles the mouse and or keyboard traps. Default is Scroll Lock.
        /// </summary>
	    public static int TrapToggleKeyCode { get; set; } = VK_SCROLL;

        /// <summary>
        /// The time in milliseconds to hold the scroll key.
        /// </summary>
        public static int ScrollHold { get; set; } = 50;

        /// <summary>
        /// The minimum time in milliseconds to hold key presses.
        /// </summary>
        public static int PressHold { get; set; } = 0;

        /// <summary>
        /// The amount of milliseconds between each call of the Direct Input polling timer
        /// </summary>
        public const int DIRECT_INPUT_POLLING_INTERVAL = 40;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Enables the mouse hook.
        /// </summary>
        public static void EnableMouseHook()
        {
            if (mouseHookHandle != 0) return;

            mouseDelegate = MouseHookProc;
            mouseHookHandle = SetWindowsHookEx(WH_MOUSE_LL, mouseDelegate, IntPtr.Zero, 0);

            if (mouseHookHandle != 0) return;

            // If subscription failed, throw an exception with the error that occurred during the hook process.
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Disables the mouse hook.
        /// </summary>
        public static void DisableMouseHook()
        {
            if (mouseHookHandle == 0) return;

            var result = UnhookWindowsHookEx(mouseHookHandle);
            mouseHookHandle = 0;
            mouseDelegate = null;

            if (result != 0) return;

            // If unsubscription failed, throw an exception with the error that occurred during the hook process.
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Enables the keyboard hook.
        /// </summary>
        public static void EnableKeyboardHook()
        {
            if (keyboardHookHandle != 0) return;

            keyboardDelegate = KeyboardHookProc;
            keyboardHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardDelegate, IntPtr.Zero, 0);

            if (keyboardHookHandle != 0) return;

            // If subscription failed, throw an exception with the error that occurred during the hook process.
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Disables the keyboard hook.
        /// </summary>
        public static void DisableKeyboardHook()
        {
            if (keyboardHookHandle == 0) return;

            var result = UnhookWindowsHookEx(keyboardHookHandle);
            keyboardHookHandle = 0;
            keyboardDelegate = null;

            if (result != 0) return;

            // If unsubscription failed, throw an exception with the error that occurred during the hook process.
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Acquires the currently connected DirectInput devies
        /// </summary>
        public static void AcquireJoysticks() {
            Devices.Clear();

            if (DirectInput != null) {
                foreach (var device in DirectInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)) {
                    var joystick = new Joystick(DirectInput, device.ProductGuid);
                    Console.WriteLine(string.Format("Device Acquired! {0}", device.ProductGuid.ToString()));
                    joystick.Acquire();

                    Devices.Add(joystick);
                    DirectInputState.AddDirectInputDevice(device.ProductGuid);
                }
                foreach (var device in DirectInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices)) {
                    var joystick = new Joystick(DirectInput, device.ProductGuid);
                    Console.WriteLine(string.Format("Device Acquired! {0}", device.ProductGuid.ToString()));
                    joystick.Acquire();

                    Devices.Add(joystick);
                    DirectInputState.AddDirectInputDevice(device.ProductGuid);
                }
                foreach (var device in DirectInput.GetDevices(DeviceType.Driving, DeviceEnumerationFlags.AllDevices)) {
                    var joystick = new Joystick(DirectInput, device.ProductGuid);
                    Console.WriteLine(string.Format("Device Acquired! {0}", device.ProductGuid.ToString()));
                    joystick.Acquire();

                    Devices.Add(joystick);
                    DirectInputState.AddDirectInputDevice(device.ProductGuid);
                }
                foreach (var device in DirectInput.GetDevices(DeviceType.Flight, DeviceEnumerationFlags.AllDevices)) {
                    var joystick = new Joystick(DirectInput, device.ProductGuid);
                    Console.WriteLine(string.Format("Device Acquired! {0}", device.ProductGuid.ToString()));
                    joystick.Acquire();

                    Devices.Add(joystick);
                    DirectInputState.AddDirectInputDevice(device.ProductGuid);
                }
            }
        }

        /// <summary>
        /// Enables the joystick hook
        /// </summary>
        public static void EnableDirectInputTimer() {
            directInputDelegate = DirectInputTimerProc;

            directInputTimerHandle = new Timer(
                new TimerCallback(directInputDelegate),
                null,
                DIRECT_INPUT_POLLING_INTERVAL,
            DIRECT_INPUT_POLLING_INTERVAL);

            if (directInputTimerHandle != null) return;

            // If subscription failed, throw an exception with the error that occurred during the hook process.
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static void DisableDirectInputTimer() {
            if (directInputTimerHandle == null) return;

            directInputTimerHandle.Change(
                Timeout.Infinite,
                Timeout.Infinite);
        }

        public static List<Joystick> GetCurrentlyActiveJoysticks() {
            return HookManager.Devices;
        }

        #endregion Methods
    }
}