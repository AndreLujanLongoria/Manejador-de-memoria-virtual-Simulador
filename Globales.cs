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
        public static List<string> comandos = new List<string>();
        public static int[] memoriaReal = new int[2048];
        public static int[] memoriaSwapping = new int[4096];
        public static double timestamp = 0.0;
        public static Estrategia estrategia;
    }
}
