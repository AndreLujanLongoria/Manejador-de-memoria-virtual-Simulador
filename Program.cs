using System;
using System.IO;

namespace Manejador_de_memoria_virtual__Simulador_
{
    class Program
    {        
        static void Main(string[] args)
        {
            // Si se obtuvo los comandos correctamente del archivo, procesarlos
            if(obtenerComandos()) 
            {
                procesar(Estrategia.FIFO);
                //procesar(Estrategia.LRU);
            }

            Console.Read(); //Espera a que apretemos una tecla -----Para que el resultado salga claro----
        }

        static void procesar(Estrategia estra)
        {
            Globales.estrategia = estra;

            int numProceso = 1;
            // Proceso princial que ejecuta comando por comando
            string[] elementos;
            foreach(string comando in Globales.comandos)
            {
                switch(comando[0])
                {
                    // COMANDO PROCESAR - Cargar a memoria un proceso
                    case 'P':
                        elementos = comando.Split();
                        Console.WriteLine(elementos[0] + ' ' + elementos[1] + ' ' + elementos[2]);
                        Comandos.procesarP(int.Parse(elementos[1]), int.Parse(elementos[2]));
                        break;

                    // COMANDO ACCEDER - Acceder o modificar un proceso en memoria
                    case 'A':
                        // elementos = comando.Split();
                        // Console.WriteLine(elementos[0] + ' ' + elementos[1] + ' ' + elementos[2] + elementos[3]);
                        // Comandos.procesarA(int.Parse(elementos[1]), int.Parse(elementos[2]), int.Parse(elementos[3]));
                        break;

                    // COMANDO LIBERAR - Liberar un proceso en memoria
                    case 'L':
                        // elementos = comando.Split(' ');
                        // Console.WriteLine(elementos[0] + ' ' + elementos[1]);
                        // Comandos.procesarL(int.Parse(elementos[1]));
                        break;

                    // COMANDO COMENTARIO - Solo se despliega el comentario
                    case 'C':                        
                        // Console.WriteLine(comando);
                        break;

                    // COMANDO FINALIZAR - Se finaliza una seccion de los comandos (Se despliegan resultado de analiticas)
                    case 'F':
                        // Console.WriteLine(comando);
                        // Comandos.procesarF();
                        break;

                    // COMANDO TERMINAR - Se finaliza el programa
                    case 'E':
                        // Console.WriteLine(comando);
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

        static bool obtenerComandos()
        {
            try
            {
                // TODO al finalizar el programa
                // Obtener el archivo txt en base al folder relativo donde se ejecuto el programa
                // Utilizar al final cuando se tenga que obtener archivo txt relativo al exe
                //Console.WriteLine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));

                // Obtener lista de comandos del archivo txt
                // StreamReader archivo = new StreamReader(@"c:/../../../../ArchivoTrabajo.txt");
                StreamReader archivo = new StreamReader(Directory.GetCurrentDirectory() + "/ArchivoTrabajo.txt");

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
                return true;
            }
            catch // Error cargando archivo
            {
                Console.WriteLine("ERROR: El archivo esta vacio o no existe.");
                return false;
            }
        }
    }
}
