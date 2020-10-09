using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace ProgramadorAVRArduino
{
    class Program
    {
        public static char[] animation = {'/','|','\\'};
        public static int animationIndex = 0 ;
        public static List<string> portsProgrammed ;

        static void Main(string[] args)
        {
            portsProgrammed = new List<string>();

            while( true )
            {
                Thread.Sleep( 500 );
                string[] ports = SerialPort.GetPortNames();
                var portsNotProgrammed = Array.FindAll( ports, port => !portsProgrammed.Exists( p => port == p ) );

                Array.ForEach( portsNotProgrammed, p => ProgramPort( p ));
            }
        }

        public static string WaitingForIOAnimation()
        {
            animationIndex = (animationIndex + 1) % 3 ;
            return("Waiting for IO..." + animation[animationIndex]);
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
            }
        }
    }
}
