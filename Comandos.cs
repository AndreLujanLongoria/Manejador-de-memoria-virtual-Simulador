using System;
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
            // Dos escenarios: Hay espacio y no hay espacio en memoria
            double numPagNecesarias = n / Globales.TAM_PAGINA;
            numPagNecesarias = Math.Ceiling(numPagNecesarias);

            // Escenario 1: Hay espacio en memoria
            if(Globales.memoria.paginasLibres >= numPagNecesarias) {
                // Buscar espacios disponibles y asignarlos
                foreach(Pagina pagina in Globales.memoria.memoriaReal) {
                    if(numPagNecesarias <= 0) break; // Todas las paginas fueron asignadas
                    
                    // Si la pagina esta disponble, asignar
                    if(pagina.idProceso != -1) {
                        pagina.idProceso = p;
                        numPagNecesarias--;
                    }
                }
            }
            // Escenario 2: NO hay espacio en memoria
            else {
                // TODO swapout
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

        }

        /// <summary>
        /// Funcion para procesar el comando F (Finalizar seccion)
        /// </summary>
        public static void procesarF()
        {

        }

        /// <summary>
        /// Funcion para procesar el comando E (Terminar)
        /// </summary>
        public static void procesarE()
        {

        }

    }
}
