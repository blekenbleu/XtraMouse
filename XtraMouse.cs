using InputInterceptorNS;
using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;

namespace XtraMouse
{
	[PluginDescription("intercept events from an extra mouse")]
	[PluginAuthor("blekenbleu")]
	[PluginName("XtraMouse")]
	public class XtraMouse : IPlugin, IDataPlugin, IWPFSettingsV2
	{
		private XtraMouseSettings Settings;
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
			Settings.state = state;
			Settings.Selected = SettingsControl.Selected;
			if (null != Intermouse)
			{
				MessageBox.Show(Application.Current.MainWindow,
    					      	$"Intermouse.Devices({Intercept.Captured}):  " + Intermouse.Devices(Intercept.Captured),
                				"XtraMouse.End()");
				Settings.Count = Intermouse.Count;
				Settings.Device = Intermouse.Devices(Intercept.Captured);
				Settings.Stroke = Intercept.Stroke;
				Intermouse.End();
			}
			Settings.EndTime = DateTime.Now;
			this.SaveCommonSettings("GeneralSettings", Settings);
		}

		void Resume()
		{
			if (Intermouse.Count == Settings.Count
			 && Intermouse.Devices(Settings.Selected) == Settings.Device)
			{
				Intercept.Captured = SettingsControl.Selected = Settings.Selected;
				state = Settings.state;
				Intercept.Stroke = Settings.Stroke;
			}
		}

		/// <summary>
		/// Returns the settings control, return null if no settings control is required
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <returns></returns>
		public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			return new SettingsControl(this);		// invoked *after* Init()
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
			Settings = this.ReadCommonSettings<XtraMouseSettings>("GeneralSettings", () => new XtraMouseSettings());

			// Declare a property available in the property list, this gets evaluated "on demand" (when shown or used in formulas)
			this.AttachDelegate("CurrentDateTime", () => DateTime.Now);
			this.AttachDelegate("Settings.EndTime", () => Settings.EndTime);
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

			if (4 > state)
			{
				Resume();
				this.AttachDelegate("Mouse_X", () => Intercept.Stroke[1]);
				this.AttachDelegate("Mouse_Y", () => Intercept.Stroke[2]);
				this.AttachDelegate("Scroll_x", () => Intercept.Stroke[3]);
				this.AttachDelegate("Scroll_y", () => Intercept.Stroke[4]);
				this.AttachDelegate("button0", () => SettingsControl._mainViewModel.button[0]);
				this.AttachDelegate("button1", () => SettingsControl._mainViewModel.button[1]);
				this.AttachDelegate("button2", () => SettingsControl._mainViewModel.button[2]);
				this.AttachDelegate("button3", () => SettingsControl._mainViewModel.button[3]);
				this.AttachDelegate("button4", () => SettingsControl._mainViewModel.button[4]);

				this.AttachDelegate("Intermouse.Count", () => Intermouse.Count);
				this.AttachDelegate("Settings.Count", () => Settings.Count);
				this.AttachDelegate("Settings.Selected", () => Settings.Selected);
				this.AttachDelegate("SettingsControl.Selected", () => SettingsControl.Selected);
				this.AttachDelegate("Intercept.Captured", () => Intercept.Captured);
				this.AttachDelegate("Settings.Device", () => Settings.Device);
				this.AttachDelegate("Intermouse.Devices(Settings.Selected)", () => Intermouse.Devices(Settings.Selected));

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
