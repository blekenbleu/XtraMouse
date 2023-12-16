using InputInterceptorNS;
using System;
using System.Windows;
using System.Windows.Controls;

namespace blekenbleu
{
	/// <summary>
	/// Interaction logic for Control.xaml
	/// </summary>
	public partial class Control : UserControl
	{
		public XtraMouse Plugin { get; }
		internal static short Selected = 0;
		string[] oops = { "Only one mouse;  none available to capture",
						"null Intercept.devices.Count",
						"null Intercept.devices",
						"Intercept.Initialize():  failed" };

		// callback needs to reference XAML control from a static method
		internal static DataViewModel _mainViewModel = new();

		public Control()
		{
			InitializeComponent();
		}

		public Control(XtraMouse plugin) : this()
		{
			this.Plugin = plugin;

			this.DataContext = _mainViewModel;

			if (0 == Intercept.code) {
				plugin.Intermouse.Initialize(WriteStatus, ColorButton);
				if (2 > Intercept.devices.Count)
					None();
				else if (2 == plugin.Settings.state && plugin.Settings.Device == plugin.Intermouse.Devices(plugin.Settings.Selected))
				{
					plugin.state = plugin.Settings.state;
					Intercept.Captured = Intercept.Stroke[0] = Selected = plugin.Settings.Selected;
					capture.Content = "click to center captured coordinates";
					capture.Visibility = Visibility.Visible;
					SHlabel.Content = plugin.Settings.Device;
					select.Content = "Click to deselect";
				} else {
					plugin.state =  0;
					Intercept.Captured = Intercept.Stroke[0] = Selected = 0;
				}
			} else {
				select.Content = "OK";
				SHlabel.Content = oops[plugin.state - 96];
			}
		}

		// https://stackoverflow.com/questions/13121155
		public static void WriteStatus(string text)
		{
			_mainViewModel.StatusText = text;		// _mainViewModel is static
		}

		public static void ColorButton (ushort index, bool down)
		{
			switch (index) {
				case 0:
					_mainViewModel.ButtonColor0 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
				case 1:
					_mainViewModel.ButtonColor1 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
				case 2:
					_mainViewModel.ButtonColor2 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
				case 3:
					_mainViewModel.ButtonColor3 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
				case 4:
					_mainViewModel.ButtonColor4 = down ? _mainViewModel.red : _mainViewModel.white;
					break;
				default:
					break;
			}
		}

		private void Select_Click(object sender, RoutedEventArgs e)
		{
			if (99 == Intercept.code)
				return;

			if (2 > Intercept.devices.Count)
				None();
			else if (0 == Plugin.state)
			{
//				SHlabel.Content = $"state {Plugin.state}:  mouse {Intercept.Stroke[0]} selected";
				SHlabel.Content = $"Mouse {Intercept.Stroke[0]} selected";
				Selected = Intercept.Stroke[0];

				select.Content = "Click to deselect";
				capture.Visibility = Visibility.Visible;
				Plugin.state = 1;
			}
			else	// deselect if state 1 or 2
			{
				Intercept.Captured = 0;
				capture.Visibility = Visibility.Hidden;
//				SHlabel.Content = $"state {Plugin.state}:  Left-click 'Select' using mouse to be captured for SimHub";
				SHlabel.Content = "Left-click 'Select' using mouse to be captured for SimHub";
				select.Content = "Select current device";
				capture.Content = "Capture selected device for SimHub use";
				Plugin.state = 0;
			}
		}

		void None()
		{
			SHlabel.Content = oops[0];
			select.Content = "OK";
			Status.Visibility = Visibility.Hidden;
			Plugin.state = 0;
		}

		bool Hooked()		// callback filters on Intercept.Captured
		{
			string foo = Plugin.Intermouse?.Devices(Intercept.Captured);
			if (null != foo && "" != foo)
			{
				SHlabel.Content = foo;
				if (1 == Plugin.state)
					Plugin.state++;
				return true;
			}
			return false;
		}

		private void Capture_Click(object sender, RoutedEventArgs e)
		{
			if (2 > Intercept.devices.Count)
				None();
			else if (2 == Plugin.state)
			{
				Intercept.Stroke[1] = Intercept.Stroke[2] = Intercept.Stroke[3] = Intercept.Stroke[4] = 0;
//				SHlabel.Content = _mainViewModel.ButtonColor0+_mainViewModel.ButtonColor1+_mainViewModel.ButtonColor2
//					+_mainViewModel.ButtonColor3+_mainViewModel.ButtonColor4;
				return;
			}
			else 
			{
				Intercept.Captured = Selected;

//				capture.Visibility = Visibility.Hidden;
				capture.Content = "click to center captured coordinates";
				Hooked();
			}
		}
	}
}
