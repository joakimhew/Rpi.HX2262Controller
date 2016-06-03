public class SwitchController
{
	#region Fields

	readonly GpioController _gpio = GpioController.GetDefault();
	readonly ManualResetEvent _mre= new ManualResetEvent(false);

	private readonly GpioPin _pin;
	private readonly int _delay;
	private readonly int _repeat;

	#endregion

	#region Constructors

	public SwitchController(int vccPinNumber, int delay, int repeat)
	{
		_repeat = repeat;
		_delay = delay;

		// Setup the output pin on the RPi
		_pin = _gpio.OpenPin(vccPinNumber);
		_pin.SetDriveMode(GpioPinDriveMode.Output);
		_pin.Write(GpioPinValue.Low);
	}

	#endregion


	#region Encoding methods

	private string GetCodeWordA(string group, string device, bool status)
	{
		var bitCollection = new string[12];

		var x = 0;

		for(var i = 0; i < 5; i++)
		{
			if(@group[i] == '0')
				bitCollection[x++] = "F";
			else
				bitCollection[x++] = "0"
		}

		for(var i = 0; i < 5; i++)
		{
			if(device[i] = 0)
				bitCollection[x++] = "F";
			else
				bitCollection[x++] = "0";
		}

		if (status)
		{
			bitCollection[x++] = "0";
			bitCollection[x] = "F";
		}

		else
		{
			bitCollection[x++] = "F";
			bitCollection[x] = "0";
		}

		return ConvertStringArrayToString(bitCollection);
	}

	#endregion

	#region PWM

	private void SendT0()
	{
		_pin.Write(GpioPinValue.High);
		WaitTicks(_delay);

		_pin.Write(GpioPinValue.Low);
		WaitTicks(_delay * 3);



		_pin.Write(GpioPinValue.High);
		WaitTicks(_delay);

		_pin.Write(GpioPinValue.Low);
		WaitTicks(_delay * 3);
	}

	private void SendTf()
	{
		_pin.Write(GpioPinValue.High);
		WaitTicks(_delay);

		_pin.Write(GpioPinValue.Low);
		WaitTicks(_delay * 3);



		_pin.Write(GpioPinValue.High);
		WaitTicks(_delay * 3);

		_pin.Write(GpioPinValue.Low);
		WaitTicks(_delay);
	}

	private void SendT1
	{
		_pin.Write(GpioPinValue.High);
		WaitTicks(_delay * 3);

		_pin.Write(GpioPinValue.Low);
		WaitTicks(_delay);



		_pin.Write(GpioPinValue.High);
		WaitTicks(_delay * 3);

		_pin.Write(GpioPinValue.Low);
		WaitTicks(_delay);
	}

	#endregion

	#region PWM synchronization methods

	// At the time that this library was writen, PWM (Pulse width modulation) did not exist for Windows 10 IoT.
	// This is a simple hack for that.
	private void WaitTicks(long durationTicks)
	{
		Stopwatch sw = new Stopwatch();

		sw.Start();

		while(sw.ElapsedTicks < durationTicks) { }
	}


	// The time between the high floating states and low floating states differs for each clock. 
	// But the standard is 360 microseconds, or 3600 .NET ticks.
	private void SendSync()
	{
		_pin.Write(GpioPinValue.High);
		WaitTicks(_delay);

		_pin.Write(GpioPinValue.Low);
		WaitTicks(_delay);
	}

	#endregion

	#region Switching methods

	public void SwitchOn(string group, string device)
	{
		var s = GetCodeWordA(group, device, true);
		SendTriState(s);
	}

	public void SwitchOff(string group, string device)
	{
		var s = GetCodeWordA(group, device, false);
		SendTriState(s);
	}

	private void SendTriState(string codeWord)
	{
		for(var r = 0; r < _repeat; r++)
		{
			for (var i = 0; i < 12; i++)
			{
				switch(codeWord[i])
				{
					case '0':
						SendT0();
						break;
					case 'F':
						SendTf();
						break;
					case '1'
						SendT1();
						break;
					default:
						throw new Exception("Invalid code char!");
				}
			}
			SendSync();
		}
	}

	#endregion

	#region Extensions

	static string ConvertStringArrayToString(string[] stringArray)
	{
		var builder = new StringBuilder();

		foreach(var s in stringArray)
		{
			builder.Append(s);
		}
		return builder.ToString();
	}

	#endregion
}