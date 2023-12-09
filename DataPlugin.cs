using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Windows.Media;
using System.ComponentModel;

namespace XtraMouse
{
	// define a class with Model-view-viewmodel pattern
	public class DataViewModel : INotifyPropertyChanged
	{
		// One event handler for all property changes
		public event PropertyChangedEventHandler PropertyChanged;
		public readonly string red = "Red", white = "White";
		private string _statusText;
		// PropertyChanged does not work for array elements
		private string _button0, _button1, _button2, _button3, _button4;

		public string ButtonColor0
		{
			get
			{
				return _button0;
			}

			set
			{
				if (value == _button0)
					return;

				_button0 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor0"));
			}
		}

		public string ButtonColor1
		{
			get
			{
				return _button1;
			}

			set
			{
				if (value == _button1)
					return;

				_button1 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor1"));
			}
		}

		public string ButtonColor2
		{
			get
			{
				return _button2;
			}

			set
			{
				if (value == _button2)
					return;

				_button2 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor2"));
			}
		}

		public string ButtonColor3
		{
			get
			{
				return _button3;
			}

			set
			{
				if (value == _button3)
					return;

				_button3 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor3"));
			}
		}

		public string ButtonColor4	// 5 mouse buttons max known to work
		{
			get
			{
				return _button4;
			}

			set
			{
				if (value == _button4)
					return;

				_button4 = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonColor4"));
			}
		}

		public string StatusText
		{
			get
			{
				return _statusText;
			}

			set
			{
				if (value == _statusText)
					return;

				_statusText = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StatusText"));
			}
		}
	}

	[PluginDescription("intercept events from an extra mouse")]
	[PluginAuthor("blekenbleu")]
	[PluginName("XtraMouse")]
	public class DataPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
	{
		public DataPluginSettings Settings;

		// callback needs to reference XAML control from a static method
		private static DataViewModel _mainViewModel = new DataViewModel();

		/// <summary>
		/// Instance of the current plugin manager
		/// </summary>
		public PluginManager PluginManager { get; set; }

		/// <summary>
		/// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
		/// </summary>
		public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);

		/// <summary>
		/// Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName attribute.
		/// </summary>
		public string LeftMenuTitle => "XtraMouse";

		/// <summary>
		/// Called one time per game data update, contains all normalized game data,
		/// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
		///
		/// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
		///
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <param name="data">Current game data, including current and previous data frame.</param>
		public void DataUpdate(PluginManager pluginManager, ref GameData data)
		{
			// Define the value of our property (declared in init)
			if (data.GameRunning)
			{
				if (data.OldData != null && data.NewData != null)
				{
					if (data.OldData.SpeedKmh < Settings.SpeedWarningLevel && data.OldData.SpeedKmh >= Settings.SpeedWarningLevel)
					{
						// Trigger an event
						this.TriggerEvent("SpeedWarning");
					}
				}
			}
		}

		/// <summary>
		/// Called at plugin manager stop, close/dispose anything needed here !
		/// Plugins are rebuilt at game change
		/// </summary>
		/// <param name="pluginManager"></param>
		public void End(PluginManager pluginManager)
		{
			// Save settings
			this.SaveCommonSettings("GeneralSettings", Settings);
		}

		/// <summary>
		/// Returns the settings control, return null if no settings control is required
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <returns></returns>
		public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			return new SettingsControl(this);
		}

		/// <summary>
		/// Called once after plugins startup
		/// Plugins are rebuilt at game change
		/// </summary>
		/// <param name="pluginManager"></param>
		public void Init(PluginManager pluginManager)
		{
			SimHub.Logging.Current.Info("Starting plugin");

			// Load settings
			Settings = this.ReadCommonSettings<DataPluginSettings>("GeneralSettings", () => new DataPluginSettings());

			// Declare a property available in the property list, this gets evaluated "on demand" (when shown or used in formulas)
			this.AttachDelegate("CurrentDateTime", () => DateTime.Now);

			// Declare an event
			this.AddEvent("SpeedWarning");

			// Declare an action which can be called
			this.AddAction("IncrementSpeedWarning",(a, b) =>
			{
				Settings.SpeedWarningLevel++;
				SimHub.Logging.Current.Info("Speed warning changed");
			});

			// Declare an action which can be called
			this.AddAction("DecrementSpeedWarning", (a, b) =>
			{
				Settings.SpeedWarningLevel--;
			});
		}
	}
}
