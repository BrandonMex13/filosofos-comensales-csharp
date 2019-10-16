using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Filo
{
    class Program
    {
        static void Main()
        {
            Filosofos.num_filosofos = 5;
            Filosofos.rand_min = 2;
            Filosofos.rand_max = 5;

            Filosofos.filosofo_estados = new Filosofos.estados[Filosofos.num_filosofos];
            Filosofos.tenedores = new object[Filosofos.num_filosofos];

            Thread[] filosofos = new Thread[Filosofos.num_filosofos];
            for (int i = 0; i < Filosofos.num_filosofos; i++)
            {
                filosofos[i] = new Thread(new ThreadStart(Filosofos.correr));
                filosofos[i].Start();
            }
        }

        // Se crea el filosofo
        class Filosofos
        {

            public static int num_filosofos;
            public enum estados { pensando, hambriento, comiendo, satisfecho };
            public static object[] tenedores;
            private static Object tenedores_lock1 = new Object(); 
            private static Object tenedores_lock2 = new Object();
            public static estados[] filosofo_estados = new estados[num_filosofos];
            private static Object num_lock = new Object();

            private static int next_filosofo_num = 0;
            public static int rand_max, rand_min;
            private static Random rand = new Random();
            private static Object print_lock = new Object();

            public static void correr()
            {
                for (int i = 0; i < tenedores.Length; i++)
                {
                    tenedores[i] = new object();
                }

                int filosofo_num = asigna_num();
                int comida = 5;
                while (comida > 0)
                {

                    pensar();
                    tomar_tenedores(filosofo_num);
                    comer();
                    poner_tenedores(filosofo_num);

                    comida--;
                    if( comida < 1)
                    {
                        Console.WriteLine("Un FILOSOFO termino de comer");
                        Console.WriteLine();
                        Console.WriteLine();
                    }
                }
            }

            private static int asigna_num()
            {
                int temp;
                lock (num_lock)
                {
                    temp = next_filosofo_num;
                    next_filosofo_num++;
                }

                return temp;
            }

            private static void poner_tenedores(int i)
            {
                lock (tenedores_lock1)
                {
                    Monitor.Exit(tenedores[i]);
                    Monitor.Exit(tenedores[(i + 1) % num_filosofos]);
                    filosofo_estados[i] = estados.pensando;
                }
                print();
            }

            private static void tomar_tenedores(int i)
            {
                filosofo_estados[i] = estados.hambriento;
                print();

                lock (tenedores_lock2)
                {
                    Monitor.Enter(tenedores[i]);
                    Monitor.Enter(tenedores[(i + 1) % num_filosofos]);

                    filosofo_estados[i] = estados.comiendo;
                }
                print();
            }
            private static void print()
            {
                lock (print_lock)
                {
                    for (int i = 0; i < num_filosofos; i++)
                    {
                        Console.Write("{0}    ", filosofo_estados[i]);
                    }
                    Console.WriteLine("   ");
                }
            }

            private static void comer()
            {
                Thread.Sleep(rand.Next(rand_min, rand_max) * 1000);
            }

            private static void pensar()
            {
                Thread.Sleep(rand.Next(rand_min, rand_max) * 1000);
            }
        }
    }
}