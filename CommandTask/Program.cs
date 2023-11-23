using System.Runtime.ConstrainedExecution;
using System.Text;
namespace command
{

    /// <summary>
    /// To use the remote control type command 0-8. After enabled it should show the status on the bottom as ON/OFF.
    /// If you want to turn off the command you can type "undo". This sets last command status to OFF.
    /// First of all you have to insert the previous command number (0-8) then after insert "undo".
    /// </summary>
    public class RemoteLoader
    {
        public static void Main(String[] args)
        {
            RemoteControl remoteControl = new RemoteControl();

            Light livingRoomLight = new Light("Living Room");
            Light kitchenLight = new Light("Kitchen");
            Thermostat thermostat = new Thermostat("Living Room");

            LightOnCommand livingRoomLightOn = new LightOnCommand(livingRoomLight);
            LightOffCommand livingRoomLightOff = new LightOffCommand(livingRoomLight);

            LightOnCommand kitchenLightOn = new LightOnCommand(kitchenLight);
            LightOffCommand kitchenLightOff = new LightOffCommand(kitchenLight);

            ThermostatOnCommand thermostatOn = new ThermostatOnCommand(thermostat);
            ThermostatOffCommand thermostatOff = new ThermostatOffCommand(thermostat);

            ThermostatIncreaseCommand thermostatIncrease = new ThermostatIncreaseCommand(thermostat, 10.0, 70.0);
            ThermostatDecreaseCommand thermostatDecrease = new ThermostatDecreaseCommand(thermostat, 10.0);

            // Macro setup for a slot

            Command[] backHome = { livingRoomLightOn, kitchenLightOn };
            Command[] outOfTheHome = { livingRoomLightOff, kitchenLightOff };

            MacroCommand backHomeMacro = new MacroCommand(backHome);
            MacroCommand outOfTheHomeMacro = new MacroCommand(outOfTheHome);


            // Seperate controls for each slot 
            remoteControl.SetCommand(0, livingRoomLightOn, livingRoomLightOff);
            remoteControl.SetCommand(1, livingRoomLightOff, livingRoomLightOn);
            remoteControl.SetCommand(2, kitchenLightOn, kitchenLightOff);
            remoteControl.SetCommand(3, kitchenLightOff, kitchenLightOn);
            remoteControl.SetCommand(4, thermostatOn, thermostatOff);
            remoteControl.SetCommand(5, thermostatOff, thermostatOn);
            remoteControl.SetCommand(6, thermostatIncrease, thermostatDecrease);
            remoteControl.SetCommand(7, thermostatDecrease, thermostatIncrease);

            // Macro control
            remoteControl.SetCommand(8, backHomeMacro, outOfTheHomeMacro);


            /// Output in the console
            Console.WriteLine(remoteControl);
            remoteControl.PrintCurrentStatus();

            Console.WriteLine("Enter command (0-8) or 'undo':");

            while (true)
            {
                string input = Console.ReadLine();

                if (input.ToLower() == "undo")
                {
                    remoteControl.undoButtonWasPushed();
                    remoteControl.PrintCurrentStatus();
                }
                else if (int.TryParse(input, out int commandNumber) && commandNumber >= 0 && commandNumber <= 8)
                {
                    remoteControl.OnButtonWasPushed(commandNumber);
                    Console.WriteLine(remoteControl);
                    remoteControl.PrintCurrentStatus();
                    Console.Clear();
                    Console.WriteLine(remoteControl);
                    remoteControl.PrintCurrentStatus();
                    remoteControl.OnButtonWasPushed(commandNumber);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number (0-8) or 'undo'.");
                }
            }
        }
    }
    /// <summary>
    /// Class for Light
    /// </summary>
    public class Light
    {
        private string roomName;
        private bool isOn;

        public Light(string roomName)
        {
            this.roomName = roomName;
            this.isOn = false;
        }

        public void On()
        {
            isOn = true;
            Console.WriteLine($"{roomName} light is ON");
        }

        public void Off()
        {
            isOn = false;
            Console.WriteLine($"{roomName} light is OFF");
        }

        public bool IsOn()
        {
            return isOn;
        }
    }

    /// <summary>
    /// Class for thermostat
    /// </summary>
    public class Thermostat
    {
        private string location;
        private double temperature;
        private bool isOn;

        public Thermostat(string location)
        {
            this.location = location; // Location of the house
            this.temperature = 0.0;   // Default temperature
            this.isOn = false;        // Thermostat is initially off
        }

        public void TurnOn()
        {
            isOn = true;
            Console.WriteLine($"{location} thermostat is turned on");
        }

        public void TurnOff()
        {
            isOn = false;
            Console.WriteLine($"{location} thermostat is turned off");
            temperature = 0.0;
        }

        public bool IsOn()
        {
            return isOn;
        }

        public void SetTemperature(double temperature)
        {
            if (isOn)
            {
                this.temperature = temperature;
                Console.WriteLine($"{location} thermostat is set to {temperature} degrees");
            }
            else
            {
                Console.WriteLine($"{location} thermostat is off. Cannot set temperature.");
            }
        }

        public double GetTemperature()
        {
            return temperature;
        }
    }

    /// <summary>
    /// Interface of Command
    /// </summary>
    public interface Command
    {
        public void Execute();
        public void undo();
    }

    /// <summary>
    /// No command class
    /// </summary>
    public class NoCommand : Command
    {
        public void Execute()
        {
            //Do nothing
        }
        public void undo()
        {
            //Do nothing
        }
    }

    /// <summary>
    /// Class for macro commands
    /// </summary>
    public class MacroCommand : Command
    {
        Command[] commands;

        public MacroCommand(Command[] commands)
        {
            this.commands = commands;
        }

        public void Execute()
        {
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i].Execute();
            }
        }

        public void undo()
        {
            // Undo commands in reverse order
            for (int i = commands.Length - 1; i >= 0; i--)
            {
                commands[i].undo();
            }
        }
    }

    /// <summary>
    /// Class of turning light on command
    /// </summary>
    public class LightOnCommand : Command
    {
        private Light light;

        public LightOnCommand(Light light)
        {
            this.light = light;
        }

        public void Execute()
        {
            light.On();
        }

        public void undo()
        {
            light.Off();
        }

        public bool GetStatus()
        {
            return light.IsOn();
        }
    }

    /// <summary>
    /// Class of turning light off command
    /// </summary>
    public class LightOffCommand : Command
    {
        private Light light;

        public LightOffCommand(Light light)
        {
            this.light = light;
        }

        public void Execute()
        {
            light.Off();
        }

        public void undo()
        {
            light.On();
        }

        public bool GetStatus()
        {
            return !light.IsOn();
        }
    }

    /// <summary>
    /// Class of turning thermostat on command
    /// </summary>
    public class ThermostatOnCommand : Command
    {
        private Thermostat thermostat;

        public ThermostatOnCommand(Thermostat thermostat)
        {
            this.thermostat = thermostat;
        }

        public void Execute()
        {
            thermostat.TurnOn();
        }

        public void undo()
        {
            thermostat.TurnOff();
        }

        public Thermostat GetThermostat()
        {
            return thermostat;
        }
    }

    /// <summary>
    /// Class of turining thermostat off command
    /// </summary>
    public class ThermostatOffCommand : Command
    {
        private Thermostat thermostat;
        private double prevTemperature;

        public ThermostatOffCommand(Thermostat thermostat)
        {
            this.thermostat = thermostat;
        }

        public void Execute()
        {
            prevTemperature = thermostat.GetTemperature();
            thermostat.TurnOff();
        }

        public void undo()
        {
            thermostat.SetTemperature(prevTemperature);
        }
    }

    /// <summary>
    /// Class of Thermostat temperature increase command
    /// </summary>
    public class ThermostatIncreaseCommand : Command
    {
        private Thermostat thermostat;
        private double increment;
        private double maxTemperature;

        public ThermostatIncreaseCommand(Thermostat thermostat, double increment, double maxTemperature)
        {
            this.thermostat = thermostat;
            this.increment = increment;
            this.maxTemperature = maxTemperature;
        }

        public void Execute()
        {
            double newTemperature = thermostat.GetTemperature() + increment;
            thermostat.SetTemperature(Math.Min(newTemperature, maxTemperature));
        }

        public void undo()
        {
            double newTemperature = thermostat.GetTemperature() - increment;
            thermostat.SetTemperature(Math.Max(newTemperature, 0.0));
        }
    }

    /// <summary>
    /// Class of Thermostat temperature decrease command
    /// </summary>
    public class ThermostatDecreaseCommand : Command
    {
        private Thermostat thermostat;
        private double decrement;

        public ThermostatDecreaseCommand(Thermostat thermostat, double decrement)
        {
            this.thermostat = thermostat;
            this.decrement = decrement;
        }

        public void Execute()
        {
            double newTemperature = thermostat.GetTemperature() - decrement;
            thermostat.SetTemperature(Math.Max(newTemperature, 0.0));
        }

        public void undo()
        {
            double newTemperature = thermostat.GetTemperature() + decrement;
            thermostat.SetTemperature(Math.Min(newTemperature, 70.0));
        }
    }

    /// <summary>
    /// Main remote control of the program
    /// </summary>
    public class RemoteControl
    {
        Command[] onCommands;
        Command[] offCommands;
        Command undoCommand;
        private bool isThermostatOn;
        private bool[] commandStatus;

        public RemoteControl()
        {
            onCommands = new Command[9];
            offCommands = new Command[9];
            isThermostatOn = false;

            Command noCommand = new NoCommand();
            for (int i = 0; i < 9; i++)
            {
                onCommands[i] = noCommand;
                offCommands[i] = noCommand;
            }
            undoCommand = noCommand;

        }

        public void SetCommandStatus(int slot, bool status)
        {
            commandStatus[slot] = status;
        }

        public void SetCommand(int slot, Command onCommand, Command offCommand)
        {
            onCommands[slot] = onCommand;
            offCommands[slot] = offCommand;
        }

        public void OnButtonWasPushed(int slot)
        {
            onCommands[slot].Execute();

            // Check if thermostat is turned on
            if (onCommands[slot] is ThermostatOnCommand)
            {
                isThermostatOn = true;
            }

            undoCommand = onCommands[slot];

        }

        public void OffButtonWasPushed(int slot)
        {
            offCommands[slot].Execute();
            

            // Check if thermostat is turned off
            if (offCommands[slot] is ThermostatOffCommand)
            {
                isThermostatOn = false;
            }

            undoCommand = offCommands[slot];

        }

        public void undoButtonWasPushed()
        {
            undoCommand.undo();

            Console.WriteLine();
        }

        public void PrintCurrentStatus()
        {
            Console.WriteLine("------ Current Status ------");

            // Living Room Light
            Console.WriteLine($"Living Room Light: {(onCommands[0] is LightOnCommand ? (onCommands[0] as LightOnCommand).GetStatus() ? "ON" : "OFF" : "OFF")}");

            // Kitchen Light
            Console.WriteLine($"Kitchen Light: {(onCommands[2] is LightOnCommand ? (onCommands[2] as LightOnCommand).GetStatus() ? "ON" : "OFF" : "OFF")}");

            // Thermostat
            if (onCommands[4] is ThermostatOnCommand thermostatOnCommand)
            {
                Thermostat thermostat = thermostatOnCommand.GetThermostat();
                Console.WriteLine($"Thermostat: {(thermostat.IsOn() ? "ON" : "OFF")}, Temperature: {thermostat.GetTemperature()} degrees");
            }
            else
            {
                Console.WriteLine("Thermostat: OFF");
            }

            Console.WriteLine("-----------------------------------");
        }


        public override string ToString()
        {
            StringBuilder stringBuff = new StringBuilder();
            stringBuff.Append("\n------- Remote Control -------\n");
            for (int i = 0; i < onCommands.Length; i++)
            {
                stringBuff.Append("[slot " + i + "] " + onCommands[i].GetType().Name + "\n");
            }
            stringBuff.Append("[undo] " + undoCommand.GetType().Name + "\n");
            return stringBuff.ToString();
        }
    }
}