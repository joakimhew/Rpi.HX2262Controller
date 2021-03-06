# Rpi.HX2262Controller

This is a simple Windows 10 IoT controller for the HX2262 chip found in most standard IR electricity controllers.

Eg. of such a device: 

![HX2262 controller and socket](https://www.hempel-online.de/cms/files/images/2013-05-13%2019.19.49.jpg)

To send the waveforms for switching on or off without buying a seperate IR transmitter, simply add a connection from a desired 3V output pin on the RPi to the "DOUT" pin on the HX2262 chip.
Keep in mind that you might need to put this behind a resistor or add your own depending on the circuit. Make sure to check the diagram for the specific device you are "hacking"

Then use the code below:

```c#
public YourMethod()
{
    // 5 is the output pin on your PI, 3600 is the delay betwen each high or low bit. (3600 is standard on most devices). 
    // Repeat is how many times you wish for the waveform to be sent.
    
    SwitchController sc = new SwitchController(5, 3600, int repeat);
    
    // First parameter specifies what group your IR Socket is set too. In this case, only the first pin of the DIP switch is set to "ON".
    // Second parameter specifies the device DIP. In this case, the first and the second pin of the DIP switch is set to "ON".
    sc.SwitchOn("10000", "11000");
}
```

The datasheet that was used to create this library can be found here: 
http://rfelektronik.se/manuals/Datasheets/HX2262.pdf