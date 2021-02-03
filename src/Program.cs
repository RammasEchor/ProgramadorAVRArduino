using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Runtime.InteropServices;

namespace ProgramadorAVRArduino
{
    class Program
    {
        public static string[] animation = { "[-]", "[\\]", "[|]", "[/]" };
        public static int animationIndex = 0;
        public static List<string> portsProgrammed;
        public static int animChars = 3;

        static void Main(string[] args)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // If we're in linux, avrdude should be in PATH.
                processInfo.FileName = "avrdude";
                // The file to programm (.hex) should be in the current directory.
                processInfo.Arguments = $@"-v -p atmega328p -c arduino -P ~PORT~ -b 115200 -D 
                -U flash:w:""{Environment.CurrentDirectory}/Test_LESD.hex"":i";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // If we're in win, the .exe and the .conf should be in currDir/AVRdude/
                processInfo.FileName = $"{Environment.CurrentDirectory}/AVRdude/avrdude.exe";
                // The file to programm (.hex) should be in the same directory as the .exe
                processInfo.Arguments = $@"-C ""{Environment.CurrentDirectory}/AVRdude/avrdude.conf"" 
                -v -p atmega328p -c arduino -P//./~PORT~ -b 115200 -D 
                -U flash:w:""{Environment.CurrentDirectory}/Test_LESD.hex"":i";
            }

            // We clean the c# multiline string
            processInfo.Arguments = processInfo.Arguments.Replace("\n", null);
            processInfo.Arguments = processInfo.Arguments.Replace("\r", null);
            // Set up the port substitution
            string arguments = processInfo.Arguments.Replace("~PORT~", "{0}");

            portsProgrammed = new List<string>();

            Console.Write("Waiting for ports...{0}", animation[animationIndex]);
            while (true)
            {
                Thread.Sleep(500);
                Animation();
                string[] ports = SerialPort.GetPortNames();
                var portsNotProgrammed = Array.FindAll(ports, port => !portsProgrammed.Exists(p => port == p));

                Array.ForEach(portsNotProgrammed, p =>
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.WriteLine("Port {0} found; about to programm...", p);
                    // Port name substitution: {0} => Port name
                    processInfo.Arguments = string.Format(arguments, p);
                    int exitCode = ProgramPort(processInfo);
                    portsProgrammed.Add(p);
                    Console.WriteLine("Port {0} {1} programmed, exit code: {2}", p, exitCode != 0 ? "not": "", exitCode );
                    Console.Write("Waiting for COM...{0}", animation[animationIndex]);
                });
            }
        }

        public static int ProgramPort(ProcessStartInfo processInfo)
        {
            int exitCode = -1;

            Process avrWriter = new Process()
            {
                StartInfo = processInfo
            };

            try
            {
                avrWriter.Start();
                avrWriter.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                exitCode = avrWriter.ExitCode;
                avrWriter.Dispose();
            }

            return exitCode;
        }

        public static void Animation()
        {
            animationIndex = (animationIndex + 1) % 3;
            Console.SetCursorPosition(Console.CursorLeft - animChars, Console.CursorTop);
            Console.Write(animation[animationIndex]);
        }
    }
}
