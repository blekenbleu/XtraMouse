using InputInterceptorNS;
using System;
using System.Windows;
using System.Windows.Controls;
using static XtraMouse.Intercept;

namespace XtraMouse
{
	/// <summary>
	/// Interaction logic for SettingsControl.xaml
	/// </summary>
	public partial class SettingsControl : UserControl
	{
		public DataPlugin Plugin { get; }
		static ushort state = 0;			// pasted from https://github.com/blekenbleu/WPF_XAML/MainWindow.xaml.cs
		static short Selected = 0;

		// callback needs to reference XAML control from a static method
		internal static DataViewModel _mainViewModel = new();

		public SettingsControl()
		{
			InitializeComponent();
		}

		public SettingsControl(DataPlugin plugin) : this()
		{
			this.Plugin = plugin;

			this.DataContext = _mainViewModel;

			try
			{
				// InputInterceptor.Initialize() absolutely must be run before new Intercept()!!
				if (!InputInterceptor.Initialize())  // fails if DLL not linked
				{
					MessageBox.Show(Application.Current.MainWindow, "Invalid or missing DLL;  closing",
									 "InputInterceptor.Initialize()");
					WriteStatus("InputInterceptor.Initialize():  Invalid or missing DLL");
					select.Content = "OK";
					state = 99;
				} else {
	
					this.Plugin.Intermouse = new Intercept();

					if (!this.Plugin.Intermouse.Initialize(WriteStatus, ColorButton))
					{
						MessageBox.Show(Application.Current.MainWindow, "No interception", "Intermouse.Initialize()");
						state = 99;
						this.Plugin.Intermouse?.End();
					}
				}

			} catch(Exception exception) {
				MessageBox.Show(Application.Current.MainWindow, "probably bad: '"
				+ InputInterceptor.DPath + "'\n" + exception,
				"InputInterceptor.Initialize() Exception");
				SHlabel.Content = "InputInterceptor.Initialize():  Invalid or missing DLL";
				select.Content = "OK";
				state = 99;
//				this.Plugin.Intermouse?.End();
			}
		}

		// https://stackoverflow.com/questions/13121155
		public static void WriteStatus(string text)
		{
			_mainViewModel.StatusText = text;	   // _mainViewModel is static
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
			if (99 == state)
			{
				Hooked();
				return;
			}

			if (2 > Intercept.devices.Count)
			{
				SHlabel.Content = "Only one mouse;  none available to capture";
				select.Content = "OK";
				state = 99;
			}
			else if (0 == state)
			{
				SHlabel.Content = $"state {state}:  mouse {Intercept.Stroke[0]} selected";
				Selected = Intercept.Stroke[0];

				select.Content = "Click to deselect";
				capture.Visibility = Visibility.Visible;
				state = 1;
			}
			else	// deselect if state 1 or 2
			{
				Intercept.Captured = 0;
				capture.Visibility = Visibility.Hidden;
				if (1 < Intercept.devices.Count) {
					SHlabel.Content = $"state {state}:  Left-click 'Select' using mouse to be captured for SimHub";
					select.Content = "Select current device";
					capture.Content = "Capture selected device for SimHub use";
					state = 0;
				}
			}
		}

		bool Hooked()	   // what to do when a mouse is hooked; e.g. change callback
		{
			Plugin.Intermouse?.Devices(Intercept.Captured);	// if 0, iterate thru all predicate devices
			return true;
		}

		private void Capture_Click(object sender, RoutedEventArgs e)
		{
			if (2 == state)
			{
				Intercept.Stroke[1] = Intercept.Stroke[2] = Intercept.Stroke[3] = Intercept.Stroke[4] = 0;
//			  SHlabel.Content = _mainViewModel.ButtonColor0+_mainViewModel.ButtonColor1+_mainViewModel.ButtonColor2
//				  +_mainViewModel.ButtonColor3+_mainViewModel.ButtonColor4;
				return;
			}
			else Intercept.Captured = Selected;

			// capture.Visibility = Visibility.Hidden;
			capture.Content = "click to center captured coordinates";
			Hooked();
		}
	}
}
