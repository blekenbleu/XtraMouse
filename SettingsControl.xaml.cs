using System.Windows.Controls;

namespace XtraMouse
{
	/// <summary>
	/// Interaction logic for SettingsControl.xaml
	/// </summary>
	public partial class SettingsControl : UserControl
	{
		public DataPlugin Plugin { get; }

		public SettingsControl()
		{
			InitializeComponent();
		}

		public SettingsControl(DataPlugin plugin) : this()
		{
			this.Plugin = plugin;
		}
	}
}
