using System.Collections.Generic;

namespace Manejador_de_memoria_virtual__Simulador_
{
    public static class Globales
    {
        public static List<string> comandos = new List<string>();
        public static int[] memoriaReal = new int[2048];
        public static int[] memoriaSwapping = new int[4096];
    }
}
