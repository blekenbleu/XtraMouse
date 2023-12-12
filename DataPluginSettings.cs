using System;

namespace XtraMouse
{
	/// <summary>
	/// Settings class, make sure it can be correctly serialized using JSON.net
	/// </summary>
	public class DataPluginSettings
	{
		internal int SpeedWarningLevel = 100;
		internal DateTime EndTime;
		internal ushort state = 0;		// selected or captured?
		internal short Selected = 0;	// mouse device number
		internal int Count = 0;			// how many mice detected?
		internal short[] Stroke = {0, 0, 0, 0, 0};
		internal string Device = "";
	}
}
