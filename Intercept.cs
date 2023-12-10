using InputInterceptorNS;
using System;
using System.Collections.Generic;
using Device = System.Int32;

#nullable enable

namespace XtraMouse
{
    internal class Intercept
    {
        public static List<DeviceData>? devices;
        MouseHook Mousehook { get; } = new(MouseCallback);
        public static short Captured = 0;
        public delegate void WriteStatus(string s);
        static WriteStatus Writestring = Console.WriteLine;
        public delegate void ButtonDel(ushort index, bool down);
        static ButtonDel ButtonEvent = Dummy;
        public static short[] Stroke = { 0, 0, 0, 0, 0 };


        static void Dummy(ushort index, bool down) { }

        public Intercept()
        {
        }

        // https://learn.microsoft.com/en-us/dotnet/framework/interop/how-to-implement-callback-functions
        private static bool MouseCallback(Device device, ref MouseStroke ms)
        {
            return false;
        }
    }
}
