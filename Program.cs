using System;
using System.IO;

namespace Manejador_de_memoria_virtual__Simulador_
{
    class Program
    {        
        static void Main(string[] args)
        {
            obtenerComandos();
            procesar();

            Console.ReadKey(); //Espera a que apretemos una tecla -----Para que el resultado salga claro----
        }

        static void procesar()
        {

        }

        static void obtenerComandos()
        {
            try
            {
                // Obtener el archivo txt en base al folder relativo donde se ejecuto el programa
                // Utilizar al final cuando se tenga que obtener archivo txt relativo al exe
                //Console.WriteLine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));

                // Obtener lista de comandos del archivo txt
                
                StreamReader archivo = new StreamReader(@"c:/../../../../ArchivoTrabajo.txt");

                if (archivo.Peek() == -1)
                {
                    throw new Exception(); // Archivo vacio (invalido)
                }

                string linea;
                while ((linea = archivo.ReadLine()) != null)
                {
                    Globales.comandos.Add(linea.Trim());
                }

                archivo.Close();
            }
            catch // Error cargando archivo
            {
                Console.WriteLine("ERROR: El archivo esta vacio o no existe.");
            }
        }
    }
}
