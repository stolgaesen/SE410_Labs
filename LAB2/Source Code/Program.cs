using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SmartHomeSystem
{
    public delegate void DeviceStatusChange(SmartDevice device, string msg);

    public abstract class SmartDevice {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool isON { get; set; }
        public event DeviceStatusChange OnStatusChange;
        public void ToggleStatus()
        {
            isON = !isON;
            OnStatusChange?.Invoke(this, $"{Name} now is {(isON ? "ON" : "OFF")}");
        }
        protected void TriggerStatusChange(string message)
        {
            OnStatusChange?.Invoke(this, message);
        }
    }
    public class Light : SmartDevice
    {
        public Light(string name)
        {
            Name = name;
            Type = "Light";
        }

    }
    public class Thermostat : SmartDevice
    {
        public double Temperature { get; set; }
        public Thermostat(string name,double temp)
        {
            this.Name = name;
            Type = "Thermostat";
            Temperature = temp;
        }
        public void setTemp(double temp)
        {
            Temperature = temp;
            if (Temperature > 25)
            {
                TriggerStatusChange($"{Name}: Temperatur is high! ({Temperature}°C)");
            }
        }
    }
    public class Camera : SmartDevice
    {
        public Camera(string name)
        {
            this.Name = name;
            Type = "Camera";

        }
    }
    class Program
    {
        static void Main()
        {
            var cam = new Camera("Enterance Cam");
            var light = new Light("Bath Lamp");
            var thermostat = new Thermostat("Kitchen Thermostat", 24);
            var thermo2 = new Thermostat("Child's Room Thermostat",21);
            var thermo3 = new Thermostat("Bathroom Thermostat", 27);
            var cam2 = new Camera("Security Cam");
            var light2 = new Light("Enterance Lamp");


            List<SmartDevice> deviceList = new List<SmartDevice> { cam,light,thermostat,thermo2,thermo3,cam2,light2 };

            foreach(var device in deviceList)
            {
                device.OnStatusChange += (device, msg) =>
                {
                    Console.WriteLine($"Notification: {msg}");
                };
            }

            Console.WriteLine("\n--------------Device status-------------");
            light.ToggleStatus();
            cam2.ToggleStatus();
            thermostat.setTemp(29);

            Console.WriteLine("\n-------------Device that ON-------------");
            var ONdevices = deviceList.Where(c => c.isON).ToList();
            foreach (var c in ONdevices)
            {
                Console.WriteLine($"{c.Name} - {c.Type} - ON");
            }

            Console.WriteLine("\n--------Thermostats that is High--------");
            var hightTherms = deviceList
                .OfType<Thermostat>()
                .Where(t => t.Temperature > 22)
                .ToList();
            
            foreach(var highT in hightTherms)
            {
                Console.WriteLine($"{highT.Name} - {highT.Temperature}°C");
            }

            Console.WriteLine("\n----All devices in the SmartHomeSystem----");
            foreach(var device in deviceList)
            {
                string status = device.isON ? "ON" : "OFF";
                Console.Write($"{device.Name} - {device.Type} - {status}");
                if (device is Thermostat termostat)
                {
                    Console.Write($" - Temperature: {termostat.Temperature}°C");
                }
                Console.WriteLine();
            }

        }
    }

}
