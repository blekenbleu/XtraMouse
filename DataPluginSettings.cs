using SimHub.Plugins;
using System;
using System.ComponentModel;

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

		public string ButtonColor4  // 5 mouse buttons max known to work
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
    }       // class DataViewModel
}
