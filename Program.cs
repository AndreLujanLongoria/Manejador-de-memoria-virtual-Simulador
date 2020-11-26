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
            int numProceso = 1;
            // Proceso princial que ejecuta comando por comando
            foreach(string comando in Globales.comandos)
            {
                switch(comando[0])
                {
                    // COMANDO PROCESAR - Cargar a memoria un proceso
                    case 'P':
                        string[] elementos = comando.Split(' ');
                        Console.WriteLine(elementos[0] + ' ' + elementos[1] + ' ' + elementos[2]);

                        Comandos.procesarP(int.Parse(elementos[1]), int.Parse(elementos[2]));
                        break;

                    // COMANDO ACCEDER - Acceder o modificar un proceso en memoria
                    case 'A':
                        break;

                    // COMANDO LIBERAR - Liberar un proceso en memoria
                    case 'L':
                        break;

                    // COMANDO COMENTARIO - Solo se despliega el comentario
                    case 'C':
                        break;

                    // COMANDO FINALIZAR - Se finaliza una seccion de los comandos (Se despliegan resultado de analiticas)
                    case 'F':
                        break;

                    // COMANDO TERMINAR - Se finaliza el programa
                    case 'E':
                        break;
                    
                    // EL COMANDO ES ERRONEO
                    default:
                        // Error: commando no reconocible
                        Console.WriteLine($"ERROR: Proceso #{numProceso} no es valido.");
                        break;
                }
                numProceso++;
            }
        }

        static void obtenerComandos()
        {
            try
            {
                // TODO al finalizar el programa
                // Obtener el archivo txt en base al folder relativo donde se ejecuto el programa
                // Utilizar al final cuando se tenga que obtener archivo txt relativo al exe
                //Console.WriteLine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));

                // Obtener lista de comandos del archivo txt
                StreamReader archivo = new StreamReader(@"c:/../../../../ArchivoTrabajo.txt");

                // Verificar que el archivo no esta vacio
                if (archivo.Peek() == -1)
                {
                    throw new Exception();
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
