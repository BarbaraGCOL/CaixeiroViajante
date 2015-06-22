using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caixeiro
{
    class Rota
    {
        public int cidade1, cidade2, custo;
    }

    //-----------------------------------------------------------------------------

    /* matriz que contera' o grafo completo */
    class Grafo
    {
        public int[,] M; /** o grafo esperado*/
    }

    //-----------------------------------------------------------------------------

    class CaixeiroGuloso
    {
        const int MaxCusto = 50;// custo ou distância máxima entre duas cidades

        /**
         * Monta a rota obtida através do algortimo guloso
        */
        void encontraCaminho(Grafo grafo, List<Rota> caminho, ref int custo)
        {
            int i,cid1=0,cid2=0,menorCusto,tamanho=caminho.Capacity;
            int[] pos = new int[grafo.M.Length];
            List<int>adicionados=new List<int>(grafo.M.Length);//Lista contendo os vértices que já foram adicionados à rota
            adicionados.Add(0);//adiciona o primeiro vértice (0)
            Rota rota = new Rota();
            custo = 0;

            for (i = 0; i < tamanho;i++)
            {
                menorCusto = MaxCusto + 1;
                rota.cidade1 = cid1;//Adiciona a cidade 1 à rota

                //No final retorna ao vértice inicial (0)
                if(i==caminho.Capacity-1)
                {
                    menorCusto = grafo.M[cid1,0];
                    cid2 = 0;
                }
                else
                {
                    for (int j = 0; j < tamanho; j++)
                    {
                        if (j != cid1 && !adicionados.Contains(j))
                        {
                            //Encontra o arco adjacente com menor peso, e o vértice adjacente a ele
                            if (grafo.M[cid1,j] != 0 && grafo.M[cid1,j] < menorCusto )
                            {
                                menorCusto = grafo.M[cid1,j];
                                cid2 = j;
                            }
                        }
                    }
                }
                rota.cidade2 = cid2;//adiciona a segunda cidade à rota()
                rota.custo = menorCusto;//adiciona o custo da rota
                adicionados.Add(cid2);
                custo += menorCusto;//adiciona o custo da rota a soma do custo total
                caminho.Add(rota);//Adiciona a rota oa caminho
                rota = new Rota();
                cid1 = cid2;
            }
        }

        void montaGrafo(out Grafo grafo, int numCidades)
        {
            int custo;          // pesos dos arcos {i, j}
            
            Random randomizer = new Random();
            grafo = new Grafo();
            grafo.M = new int[numCidades, numCidades];

            for (int i = 0; i < numCidades; i++)
            {
                for (int j = 0; j < numCidades; j++)
                {
                    custo = randomizer.Next(MaxCusto) + 1;
                    if (i < j)
                        grafo.M[i, j] = custo;
                    else
                        if (i == j)
                            grafo.M[i, j] = 0;
                        else
                            grafo.M[i, j] = grafo.M[j, i];
                }
            }

            /* Para conferência: no formatting at all!!! */
            Console.Write("\nCidades e custos:\n   ");
            for (int i = 0; i < numCidades; i++)   // preenche a matriz grafo -> M
                Console.Write(i + " ");
            Console.WriteLine();
            for (int i = 0; i < numCidades; i++)
            { 
                // preenche a matriz grafo -> M
                Console.Write(i + " ");
                for (int j = 0; j < numCidades; j++)
                {
                    Console.Write(" " + grafo.M[i, j]);
                }
                Console.WriteLine();
            }
        } // fim montaGrafo

        /**
         * Imprime caminho encontrato pelo algoritmo guloso
         */
        void imprimeCaminho(int custo, List<Rota> melhorRota)
        {
            Console.WriteLine("\n\nCUSTO MINIMO PARA A VIAGEM DO CAIXEIRO: " + custo);
            Console.WriteLine("\n\nMELHOR CAMINHO PARA A VIAGEM DO CAIXEIRO:");
            Console.WriteLine("\n\n              DE               PARA             CUSTO ");
            foreach(Rota rota in melhorRota)
            {
                Console.Write("              " + rota.cidade1 + "                  " + rota.cidade2 + "                " + rota.custo + "\n");
            }
            Console.WriteLine("\n");
        } /* fim ImprimeCaminho */

        /**
         * Imprime o tempo gasto na execução do algoritmo
         */
        void imprimeTempo(Stopwatch tempo)
        {
            Console.WriteLine("TEMPO DE EXECUÇÂO: ");
            Console.WriteLine(tempo.Elapsed.Hours + " horas " + tempo.Elapsed.Minutes + " minutos " + tempo.Elapsed.Seconds + " segundos " + tempo.Elapsed.Milliseconds + " milisegundos");
        } /* fim Imprime tempo */

        static void Main(string[] args)
        {
            List<Rota> caminho;    /* conterá a rota da viagem */
            int numCidades,      /* numero de vértices (cidades) do grafo */
                    custo = 0;     /* custo da viagem pelo grafo (pelas cidades) */
            CaixeiroGuloso caixeiro = new CaixeiroGuloso();
            Grafo grafo; // = new Grafo();         /* matriz de adjacencia com o grafo */

            Console.Write("Digite o número de cidades e tecle ENTER: ");
            numCidades = int.Parse(Console.ReadLine());
            caixeiro.montaGrafo(out grafo, numCidades);

            caminho = new List<Rota>(numCidades);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();//Inicia a contagem do tempo
            caixeiro.encontraCaminho(grafo, caminho,ref custo);
            caixeiro.imprimeCaminho(custo, caminho);
            stopwatch.Stop();//Encerra a contagem do tempo
            caixeiro.imprimeTempo(stopwatch);

            Console.ReadKey();
        }


    }
}
