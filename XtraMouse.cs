using InputInterceptorNS;
using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;

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
		internal ushort[] button = {0, 0, 0, 0, 0};

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

				if (red == value)
				{
					button[0] = 127;
					that.TriggerEvent("Button0");
				} else button[0] = 0;

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

				if (red == value)
				{
					button[1] = 127;
					that.TriggerEvent("Button1");
				} else button[1] = 0;

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

				if (red == value)
				{
					button[2] = 127;
					that.TriggerEvent("Button2");
				} else button[2] = 0;

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

				if (red == value)
				{
					button[3] = 127;
					that.TriggerEvent("Button3");
				} else button[3] = 0;

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

				if (red == value)
				{
					button[4] = 127;
					that.TriggerEvent("Button4");
				} else button[4] = 0;

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

		XtraMouse that;

		internal void ThisSet(XtraMouse xm)
		{
			that = xm;
		}
	}		// class DataViewModel

	[PluginDescription("intercept events from an extra mouse")]
	[PluginAuthor("blekenbleu")]
	[PluginName("XtraMouse")]
	public class XtraMouse : IPlugin, IDataPlugin, IWPFSettingsV2
	{
		public DataPluginSettings Settings;
		internal Intercept Intermouse;		// instanced in SettingsControl()
		internal ushort state = 99;			// assume the worst

		/// <summary>
		/// Instance of the current plugin manager
		/// </summary>
		public PluginManager PluginManager { get; set; }

		/// <summary>
		/// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
		/// </summary>
		public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);

		/// <summary>
		/// A short plugin title to show in left menu. Return null to use the title defined in PluginName attribute.
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
			if (null != Intermouse)
			{
				Settings.Count = Intermouse.Count;
				Settings.Device = Intermouse.Devices(Intercept.Captured);
			}
			Settings.state = SettingsControl.state;
			Settings.Selected = SettingsControl.Selected;
			Settings.Stroke = Intercept.Stroke;
			Settings.EndTime = DateTime.Now;
			this.SaveCommonSettings("GeneralSettings", Settings);
			Intermouse?.End();
		}

		/// <summary>
		/// Returns the settings control, return null if no settings control is required
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <returns></returns>
		public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			return new SettingsControl(this);		// seemingly invoked *after* Init()
		}

		/// <summary>
		/// Called once after plugins startup
		/// Plugins are rebuilt at game change
		/// </summary>
		/// <param name="pluginManager"></param>
		public void Init(PluginManager pluginManager)
		{
			SimHub.Logging.Current.Info("XtraMouse Init()");

			// setting this at Init() works for Events during DataUpdate()
			SettingsControl._mainViewModel.ThisSet(this);

			// Load settings
			Settings = this.ReadCommonSettings<DataPluginSettings>("GeneralSettings", () => new DataPluginSettings());

			// Declare a property available in the property list, this gets evaluated "on demand" (when shown or used in formulas)
			this.AttachDelegate("CurrentDateTime", () => DateTime.Now);
			this.AttachDelegate("SpeedWarning", () => Settings.SpeedWarningLevel);

			try
            {
                // InputInterceptor.Initialize() absolutely must be run before new Intercept()!!
                if (InputInterceptor.Initialize())  // fails if DLL not linked
				{
					Intermouse = new Intercept();
					state = Intermouse.Initialize();
				}
			} catch(Exception exception) {
				MessageBox.Show(Application.Current.MainWindow, "probably bad: '"
    					      	+ InputInterceptor.DPath + "'\n" + exception,
                				"InputInterceptor.Initialize() Exception");
			}

			if (0 == state)
			{
				this.AttachDelegate("Mouse_X", () => Intercept.Stroke[1]);
				this.AttachDelegate("Mouse_Y", () => Intercept.Stroke[2]);
				this.AttachDelegate("Scroll_x", () => Intercept.Stroke[3]);
				this.AttachDelegate("Scroll_y", () => Intercept.Stroke[4]);
				this.AttachDelegate("button0", () => SettingsControl._mainViewModel.button[0]);
				this.AttachDelegate("button1", () => SettingsControl._mainViewModel.button[1]);
				this.AttachDelegate("button2", () => SettingsControl._mainViewModel.button[2]);
				this.AttachDelegate("button3", () => SettingsControl._mainViewModel.button[3]);
				this.AttachDelegate("button4", () => SettingsControl._mainViewModel.button[4]);

				// Declare events
				this.AddEvent("Button0");
				this.AddEvent("Button1");
				this.AddEvent("Button2");
				this.AddEvent("Button3");
				this.AddEvent("Button4");
			}

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
