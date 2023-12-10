using InputInterceptorNS;
using System.Windows.Controls;
using System.Windows;

namespace XtraMouse
{
	/// <summary>
	/// Interaction logic for SettingsControl.xaml
	/// </summary>
	public partial class SettingsControl : UserControl
	{
		public DataPlugin Plugin { get; }
		XtraMouse.Intercept Intermouse;

		public SettingsControl()
		{
			InitializeComponent();
		}

		public SettingsControl(DataPlugin plugin) : this()
		{
			this.Plugin = plugin;
		}

		private void Select_Click(object sender, RoutedEventArgs e)
		{
		}

		private void Capture_Click(object sender, RoutedEventArgs e)
		{
		}
	}
}
