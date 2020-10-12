using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace ProgramadorAVRArduino
{
    class Program
    {
        public static string[] animation = {"[-]","[\\]","[|]","[/]"};
        public static int animationIndex = 0 ;
        public static List<string> portsProgrammed ;
        public static int animChars = 3 ;

        static void Main(string[] args)
        {
            portsProgrammed = new List<string>();

            Console.Write("Waiting for COM...{0}", animation[animationIndex]);
            while( true )
            {
                Thread.Sleep( 500 );
                Animation();
                string[] ports = SerialPort.GetPortNames();
                var portsNotProgrammed = Array.FindAll( ports, port => !portsProgrammed.Exists( p => port == p ) );

                Array.ForEach( portsNotProgrammed, p => {
                    Console.SetCursorPosition( 0, Console.CursorTop );
                    Console.WriteLine("Port {0} found; about to programm...", p );
                    ProgramPort( p );
                    Console.Write("Waiting for COM...{0}", animation[animationIndex]);
                });
            }
        }

        public static void ProgramPort( string port )
        {
            Process avrWriter = new Process();
            try
            {
                avrWriter.StartInfo.FileName = string.Format("{0}/AVRdude/avrdude.exe", Environment.CurrentDirectory );
                avrWriter.StartInfo.Arguments = string.Format("-C \"{0}/AVRdude/avrdude.conf\" -v -p atmega328p -c arduino -P//./{1} -b 115200 -D -U flash:w:\"{0}/AVRdude/TEST/Test_LESD.hex\":i",
                                                            Environment.CurrentDirectory, port );
                avrWriter.Start();
                avrWriter.WaitForExit();
            }
            catch( Exception e)
            {
                Console.WriteLine( e.Message );
            }
            finally
            {
                avrWriter.Dispose();
                portsProgrammed.Add( port );
                Console.WriteLine("Port {0} programmed.", port );
            }
        }

        public static void Animation()
        {  
            animationIndex = (animationIndex + 1)  % 3 ;
            Console.SetCursorPosition( Console.CursorLeft - animChars, Console.CursorTop );
            Console.Write( animation[animationIndex] );
        }
    }
}
