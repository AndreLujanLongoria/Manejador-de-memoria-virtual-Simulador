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
            double numPagNecesarias = (double)n / Globales.TAM_PAGINA;
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
            // Verificaciones
            // Checar que el proceso este en memoria
            if(!Globales.procesos.ContainsKey(p)) {
                Console.WriteLine($"ERROR: El proceso #{p} no existe.");
                return;
            }
            // Checar que el proceso no se haya liberado
            else if(Globales.procesos[p].tiempoFinal != -1) {
                Console.WriteLine($"ERROR: El proceso #{p} ya se libero y no esta en memoria.");
                return;
            }

            // Checar que d (direccion virtual) sea valido (no mayor al tam del proceso)
            else if(Globales.procesos[p].tam <  d){
                Console.WriteLine($"ERROR: Comando no es valido: Direccion virtual {d} no existe.");
                return;
            }
            
            // Desplegar Formato especificado
            Console.WriteLine($"Obtener la dirección real correspondiente a la dirección virtual {d} del proceso {p}");

            int indicePaginaDelProceso = d / 16;
            // Proceso se encuentra en memoriaSwap
            if(Globales.memoria.isProcesoEnMemoriaReal(d, p) == -1) {
                // Checar si hay espacio 
                if(Globales.memoria.paginasLibres <= 0) {
                    // Librar una pagina en memoriaReal y hacer SwapIn
                    Globales.procesos[p].numPageFaults++;
                    Globales.memoria.swapUnaPagina();
                }
                Globales.memoria.swapIn(p, indicePaginaDelProceso);

                // Actualizar metricas
                Globales.lruProcesos[p] = Globales.timestamp;
            }

            // Desplegar resultados
            Globales.timestamp += 0.1; // Accesar o modificar toma 0.1 segundos
            
            // Se va a modificar la direccion de memoria
            if(m == 1) {
                Console.WriteLine($"Página #{indicePaginaDelProceso} del proceso #{p} modificada.");
            }

            // Desplegar direcciones de memoria virtual y real
            int dirReal = Globales.memoria.GetDirReal(d, p);
            Console.WriteLine($"Dirección virtual: {d}. Dirección real: {dirReal}");


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
                tiempo = proceso.Value.tiempoFinal - proceso.Value.tiempoInicio;
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
            Console.WriteLine($" Timestamp: {Globales.timestamp}");
            
            // Reiniciar variables
            Globales.timestamp = 0.0;
            Globales.memoria = new Memoria();
            Globales.contadorSwaps = 0;
            Globales.procesos = new Dictionary<int, Proceso>();
            Globales.filaProcesos = new Queue<int>();
            Globales.lruProcesos = new SortedList<int, double>();
            contadorProcesos = 0;
            turnaroundTotal = 0.0;
            tiempo = 0.0;
        }
    }
}