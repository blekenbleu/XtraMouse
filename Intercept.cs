using InputInterceptorNS;
using System;
using System.Collections.Generic;
using System.Windows;
using Context = System.IntPtr;
using Device = System.Int32;

#nullable enable

namespace XtraMouse
{
	/// <summary>
	/// Mouse Interception handling for MainWindow.xaml.cs
	/// </summary>
	internal class Intercept
	{
		public static List<DeviceData>? devices;
		MouseHook Mousehook { get; } = new(MouseCallback);
		public delegate void WriteStatus(string s);
		static WriteStatus Writestring = Console.WriteLine;
		public delegate void ButtonDel(ushort index, bool down);
		static ButtonDel ButtonEvent = Dummy;
		public int Count => (null != devices) ? devices.Count : 0;
		public static short[] Stroke = { 0, 0, 0, 0, 0 };
		public static short Captured = 0;

		static void Dummy(ushort index, bool down) { }

		public Intercept()
		{
		}

		public bool Initialize(Intercept.WriteStatus writeString, Intercept.ButtonDel foo)
		{
			Writestring = writeString;
			ButtonEvent = foo;


			if (!InputInterceptor.Initialized)
			{
				if (!InputInterceptor.CheckDriverInstalled())
					Writestring("Input interception driver not installed.");
				else MessageBox.Show(Application.Current.MainWindow,
								 "Input.Interceptor not initialized;  valid dll probably not found", "Intercept");
				return false;
			}
			return true;
		}

		public void End()
		{
		//	keyboardHook?.Dispose();
			Mousehook?.Dispose();
		}

		// https://learn.microsoft.com/en-us/dotnet/framework/interop/how-to-implement-callback-functions
		private static bool MouseCallback(Device device, ref MouseStroke ms)
		{
			try
			{
				if (null == devices)
					devices = InputInterceptor.GetDeviceList(InputInterceptor.IsMouse);

				if (device != Captured)
				{
					if (0 == Captured)
					{
						Stroke[0] = (short)device;
						string scroll = (0 == (0xC00 & (ushort)ms.State)) ? "" : $" x:{XY(ref ms, 11)}, y:{XY(ref ms, 10)}";
						// Mouse XY coordinates are raw changes
						Writestring($"Device: {device}; MouseStroke: X:{ms.X}, Y:{ms.Y}; S: {(ushort)ms.State}" + scroll);
						if (0 != ms.State)
							Buttons((ushort)ms.State);
					}
					return true;
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine($"MouseStroke: {exception}");
				return true;
			}

			// device == Captured
			try
			{
				// Mouse XY coordinates are raw changes
				if (0 != (0xC00 & (ushort)ms.State))
				{
					Stroke[3] += XY(ref ms, 11);
					Stroke[4] += XY(ref ms, 10);
				}
				Stroke[1] += (short)ms.X;
				Stroke[2] += (short)ms.Y;

				Writestring($"Captured Mouse {Captured}: X:{Stroke[1]}, Y:{Stroke[2]}; Scroll: x:{Stroke[3]}, y:{Stroke[4]}" );
				if (0 != ms.State)
					Buttons((ushort)ms.State);
			}
			catch (Exception exception)
			{
				Console.WriteLine($"MouseStroke: {exception}");
			}

			return false;	// do not pass Captured mouse strokes
		}

		// mouse State is a bitmap of button event pairs (down, up)
		private static void Buttons(ushort state)
		{
			bool down = true, up = false;

			for (ushort mask = 1, j = 0; j < 6; j++)
			{
				if (mask == (mask & state)) {
					ButtonEvent(j, down);
					mask <<= 1;
				} else {
					mask <<= 1;
					if (mask == (mask & state))
						ButtonEvent(j, up);
				}
					
				mask <<= 1;
			}
		}

		// decode scrolling
		private static short XY(ref MouseStroke ms, short s) {
			return (short)((((UInt16)ms.State >> s) & 1) * ((ms.Rolling < 0) ? -1 : 1));
		}

/*
		private static bool KeyboardCallback(Device device, ref KeyStroke keyStroke)
		{
			try
			{
				Writestring($"Device: {device}; Keystroke: {keyStroke.Code} {keyStroke.State} {keyStroke.Information}");
			}
			catch (Exception exception)
			{
				Console.WriteLine($"KeyStroke: {exception}");
			}
		//	Button swap
			keyStroke.Code = keyStroke.Code switch {
				KeyCode.A => KeyCode.B,
				KeyCode.B => KeyCode.A,
				_ => keyStroke.Code,
			};

			return true;
		}
 */

		public bool Devices(short stick)
		{
			for (int dd = 0; dd < devices?.Count; dd++)
			{
				if (0 == stick || stick == devices[dd].Device)
					MessageBox.Show(Application.Current.MainWindow,		// should display on top, regardless of focus
									$"device: {devices[dd].Device}: " + devices[dd].Names[0], "Intercept.Devices");
			}
			return true;
		}
	}
}
