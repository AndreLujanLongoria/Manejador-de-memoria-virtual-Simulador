// Nombre del Programa: Simulador de Manejador de Memoria Virtual 
// Autores:
// // Rodrigo Bilbao Arriet A01194189
// // Isabella Canales Backhoff A01194195
// // Andre Lujan A01540245
// // Gerardo Peart A01194337
// Descripcion: El programa sirve como un simulador a un manejador de memoria virtual con estrategias de reemplazo FIFO (First In First Out) y LRU (Least Recently Used) con memoria real y memoria de swapping.




using System;
using System.IO;
using System.Collections.Generic;

namespace Manejador_de_memoria_virtual__Simulador_
{
    class Program
    {
        static void Main(string[] args)
        {
            // Si se obtuvo los comandos correctamente del archivo, procesarlos
            if (obtenerComandos())
            {
                procesar(Estrategia.FIFO);
                procesar(Estrategia.LRU);
                Console.WriteLine("[+] ------ [Programa terminado] ------ [+]");
            }
            Console.WriteLine("Presione ENTER para salir del programa... ");
            Console.Read(); //Espera a que apretemos una tecla -----Para que el resultado salga claro----
        }

        static void procesar(Estrategia estra)
        {
            bool termino = false;
            string estraNombre = estra == Estrategia.FIFO ? "FIFO" : "LRU";
            Console.WriteLine($"[+] ------ [ Estrategia: {estraNombre}] ------ [+]");
            Globales.estrategia = estra;

            int numProceso = 1;
            // Proceso princial que ejecuta comando por comando
            string[] elementos;
            foreach (string comando in Globales.comandos)
            {
                if(termino) break;

                switch (comando[0])
                {
                    // COMANDO PROCESAR - Cargar a memoria un proceso
                    case 'P':
                        elementos = comando.Split();
                        Console.WriteLine(elementos[0] + ' ' + elementos[1] + ' ' + elementos[2]);
                        Comandos.procesarP(int.Parse(elementos[1]), int.Parse(elementos[2]));
                        break;

                    // COMANDO ACCEDER - Acceder o modificar un proceso en memoria
                    case 'A':
                        elementos = comando.Split();
                        elementos = verificarEliminacionEspacios(elementos);
                        Console.WriteLine(elementos[0] + ' ' + elementos[1] + ' ' + elementos[2] + ' ' + elementos[3]);
                        Comandos.procesarA(int.Parse(elementos[1]), int.Parse(elementos[2]), int.Parse(elementos[3]));
                        break;

                    // COMANDO LIBERAR - Liberar un proceso en memoria
                    case 'L':
                        elementos = comando.Split(' ');
                        Console.WriteLine(elementos[0] + ' ' + elementos[1]);
                        Comandos.procesarL(int.Parse(elementos[1]));
                        break;

                    // COMANDO COMENTARIO - Solo se despliega el comentario
                    case 'C':
                        Console.WriteLine(comando);
                        break;

                    // COMANDO FINALIZAR - Se finaliza una seccion de los comandos (Se despliegan resultado de analiticas)
                    case 'F':
                        Console.WriteLine(comando);
                        Console.WriteLine("Fin. Reporte de salida: ");
                        Comandos.procesarF();
                        break;

                    // COMANDO TERMINAR - Se finaliza el programa
                    case 'E':
                        Console.WriteLine(comando);
                        Console.WriteLine("\n");
                        termino = true;
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
                // TODO OPCION B
                // Obtener el archivo txt en base al folder relativo donde se ejecuto el programa
                // Utilizar al final cuando se tenga que obtener archivo txt relativo al exe
                //Console.WriteLine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));

                // Obtener lista de comandos del archivo txt
                //StreamReader archivo = new StreamReader(@"c:/../ArchivoTrabajo.txt");
                string nombreArchivo;
                Console.Write("Nombre del Archivo: ");
                nombreArchivo = Console.ReadLine();
                StreamReader archivo = new StreamReader(@"c:/../" + nombreArchivo);
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
                Console.WriteLine("\t- El archivo deberia estar en: " + Directory.GetCurrentDirectory());
                return false;
            }
        }

        public static string[] verificarEliminacionEspacios(string[] arreglo) {
            List<string> nuevoArreglo = new List<string>();
            foreach(string s in arreglo) {
                if(s != "") {
                    nuevoArreglo.Add(s);
                }
            }

            return nuevoArreglo.ToArray();
        }
    }
}
