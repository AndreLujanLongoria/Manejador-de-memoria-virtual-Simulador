using System.Collections.Generic;

namespace Manejador_de_memoria_virtual__Simulador_
{
    public enum Estrategia
    {
        FIFO,
        LRU
    }

    public static class Globales
    {
        // Constantes
        public const int TAM_PAGINA = 16;

        public static List<string> comandos = new List<string>();
        public static Memoria memoria = new Memoria();
        public static double timestamp = 0.0;
        public static Estrategia estrategia;
        public static int numSwaps = 0;
        public static int pageFaults = 0;
        public static List<Proceso> procesos;
        public static Queue<Proceso> filaProcesos; // Queue para FIFO

    }
}
