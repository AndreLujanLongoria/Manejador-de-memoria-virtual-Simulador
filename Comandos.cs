using System;
using System.Collections.Generic;

namespace Manejador_de_memoria_virtual__Simulador_
{
    public static class Comandos
    {
        /// <summary>
        /// Funcion para procesar el comando P (Poner)
        /// </summary>
        /// <param name="n">Int con el tamaño del proceso a ingresar a memoria</param>
        /// <param name="p">Int con el numero de proceso</param>
        public static void procesarP(int n, int p)
        {
            // Validacion
            if(n <= 0) {
                Console.WriteLine("El proceso #" + p + " no entra en memoria.");
                return;
            } 
            if (n > 2048) {
                Console.WriteLine("El proceso #" + p + " no cabe en memoria.");
                return;
            }

            // Desplegar comando
            Console.WriteLine($"Asignar {n} bytes al proceso #{p}.");
            
            // Agregar nuevo proceso a la lista
            Proceso nuevoProceso = new Proceso(p, n, 0, Globales.timestamp, -1);
            Globales.procesos.Add(nuevoProceso.id, nuevoProceso);
            
            // FIFO
            if(Globales.estrategia == Estrategia.FIFO) {
                Globales.filaProcesos.Enqueue(p);
            } 
            // LRU
            else {
                Globales.lruProcesos.Add(p, Globales.timestamp);
            }

            // Dos escenarios: Hay espacio y no hay espacio en memoria
            double numPagNecesarias = n / Globales.TAM_PAGINA;
            numPagNecesarias = Math.Ceiling(numPagNecesarias);

            // Escenario 1: Hay espacio en memoria
            if(Globales.memoria.GetPaginasLibres() >= numPagNecesarias) {
                Globales.memoria.AsignarPaginas(nuevoProceso);
            }
            // Escenario 2: NO hay espacio en memoria
            else {
                Globales.memoria.SwapOut(numPagNecesarias, nuevoProceso);
            }

        }

        /// <summary>
        /// Funcion para procesar el comando A (Acceder)
        /// </summary>
        /// <param name="d">Int con la direccion virtual del proceso</param>
        /// <param name="p">Int con el numero de proceso</param>
        /// <param name="m">Int con indicador de acceso (0 = Accesar, 1 = Modificar)</param>
        public static void procesarA(int d, int p, int m)
        {

        }

        /// <summary>
        /// Funcion para procesar el comando L (Liberar)
        /// </summary>
        /// <param name="p">Int con el numero de proceso</param>
        public static void procesarL(int p)
        {
            Proceso proceso = Globales.procesos[p];
            Globales.memoria.liberarProceso(proceso);
            Globales.timestamp = Globales.timestamp + 0.1;
        }


        /// <summary>
        /// Funcion para procesar el comando F (Finalizar seccion)
        /// </summary>
        public static void procesarF()
        {
            int contadorProcesos = 0;
            double turnaroundTotal = 0.0;
            double tiempo = 0.0;
            
            // Desplegar Turnaround Time por Proceso
            Console.WriteLine("Turnaround Time: ");
            foreach(KeyValuePair<int, Proceso> proceso in Globales.procesos) {
                tiempo = proceso.Value.tiempoInicio - proceso.Value.tiempoFinal;
                Console.WriteLine($"  P# {proceso.Key}: {tiempo}");
                turnaroundTotal += tiempo;
                contadorProcesos++;
            }

            // Desplegar Turnaround Promedio
            Console.WriteLine($"Turnaround Promedio: {turnaroundTotal / contadorProcesos}");

            // Desplegar Page Faults por Proceso
            Console.WriteLine("Page Faults: ");
            foreach(KeyValuePair<int, Proceso> proceso in Globales.procesos) {
                Console.WriteLine($"  P# {proceso.Key}: {proceso.Value.numPageFaults}");
            }

            // Desplegar número total de operaciones de swap-out y swap-in
            Console.WriteLine($" Número total de operaciones de swap-out y swap-in: {Globales.contadorSwaps}");

            // Reiniciar variables
            Globales.timestamp = 0.0;
            Globales.memoria = new Memoria();
            Globales.contadorSwaps = 0;
            Globales.procesos = new Dictionary<int, Proceso>();
            Globales.filaProcesos = new Queue<int>();
            //Globales.stackProcesos = new Stack<int>();
            
        }

        /// <summary>
        /// Funcion para procesar el comando E (Terminar)
        /// </summary>
        public static void procesarE() 
        {
            System.Environment.Exit(0); // Is this okay?
        }

    }
}