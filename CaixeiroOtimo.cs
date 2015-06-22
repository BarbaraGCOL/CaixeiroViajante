using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Caixeiro
{

    /* Um campo do vetor que contera a melhor rota */
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

    class CaixeiroOtimo
    {
        const int MaxCusto = 50;     // custo ou distância máxima entre duas cidades
        //------------------------------------------------------------------------------

        /* 
         * Verifica se a permutação passada como parâmetro tem custo melhor que o custo
         * já obtido. Caso positivo, então monta a rota correspondnete à permutação como
         * sendo a melhor rota (e armazena no vetor melhorRota, retornando tambem o custo 
         * total da melhor rota 
         */
        void melhorCaminho(Grafo grafo, Rota[] melhorRota, ref int melhorCusto, int[] permutacao)
        {
            int j, k;                     /* contadores: auxiliam a montagem das rotas */
            int cid1, cid2;             /* cidades da melhor rota */
            int custo;                 /* custo total da melhor rota */
            int[] proxDaRota;        /* vetor que armazena a sequencia de cidades que estao
				                           em uma rota, tal que um indice indica uma cidade e
				                           o conteudo deste indice, a proxima cidade da rota */

            proxDaRota = new int[melhorRota.Length];
            /* monta uma rota com a permutacao */
            cid1 = 0;									/* a primeira cidade é a cidade 0 */
            cid2 = permutacao[1];
            custo = grafo.M[cid1, cid2];
            proxDaRota[cid1] = cid2;

            for (j = 2; j < melhorRota.Length; j++)
            {
                cid1 = cid2;
                cid2 = permutacao[j];
                custo += grafo.M[cid1, cid2];  /* calcula o custo parcial da rota */
                proxDaRota[cid1] = cid2;      /* armazena a rota fornecida pela permutacao */
            }

            proxDaRota[cid2] = 0;			/* completa o ciclo da viagem */
            custo += grafo.M[cid2, 0];  /* custo total desta rota */

            if (custo < melhorCusto)	/* procura pelo melhor (menor) custo */
            {
                melhorCusto = custo;
                cid2 = 0;
                for (k = 0; k < melhorRota.Length; k++) /* guarda a melhor rota */
                {
                    cid1 = cid2;
                    cid2 = proxDaRota[cid1];
                    melhorRota[k].cidade1 = cid1;
                    melhorRota[k].cidade2 = cid2;
                    melhorRota[k].custo = grafo.M[cid1, cid2];
                }
            }
        } /* fim melhorCaminho */

        //------------------------------------------------------------------------------
        /* Gera os possiveis caminhos entre a cidade zero e as outras (N-1) envolvidas
             na busca, armazenando-os no vetor permutacao, um por vez, e a cada permutacao
             gerada, chama a funcao melhorCaminho que escolhe o caminho (a permutacao) de
             menor custo.
         * CÓDIGO ADAPTADO DE "Algorithms in C" (Robert Sedgewick), página 624.
        */
        void permuta(int[] permutacao, Grafo grafo, Rota[] melhorRota, ref  int melhorCusto,int controle, int k)
        {
            int i;
            permutacao[k] = ++controle;
            if (controle == (melhorRota.Length - 1)) /* se gerou um caminho então verifica se ele é melhor */
                melhorCaminho(grafo, melhorRota, ref melhorCusto, permutacao);
            else
                for (i = 1; i < melhorRota.Length; i++)
                    if (permutacao[i] == 0)
                        permuta(permutacao, grafo, melhorRota, ref melhorCusto, controle, i);
            controle--;
            permutacao[k] = 0;
        } /* fim permuta */

        //------------------------------------------------------------------------------

        /*  Gera os pesos dos arcos do grafo randomicamente e preenche
           a matriz grafo->M, que e indexada pelos nomes dos vertices (cidades)
         */
        void montaGrafo(out Grafo grafo, int numCidades)
        {
            //int i, j;      // indexadores da matriz
            int custo;             // pesos dos arcos {i, j}

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
            { // preenche a matriz grafo -> M
                Console.Write(i + " ");
                for (int j = 0; j < numCidades; j++) 
                {
                    Console.Write(" " + grafo.M[i, j]);
                }
                Console.WriteLine();
            }
        } // fim montaGrafo

        //------------------------------------------------------------------------------
        /*
                Gera os possiveis caminhos entre a cidade zero e todas as outras envolvidas
             na rota da viagem do caixeiro e escolhe a melhor rota entre todas.
        */
        void geraEscolheCaminhos(ref int[] permutacao, Grafo grafo, Rota[] melhorRota, out int melhorCusto)
        {
            int controle=-1;
            melhorCusto = int.MaxValue;
            
            for (int i = 0; i < melhorRota.Length; i++)
                melhorRota[i] = new Rota();
            
            /* Gera os caminhos possiveis e escolhe o melhor, chamando a funcao recursiva
             permuta */
            permuta(permutacao, grafo, melhorRota, ref melhorCusto, controle, 1);
        } /* fim GeraEscolheCaminhos */

        //------------------------------------------------------------------------------
        /*
            Imprime o melhor caminho para a viagem do caxeiro, bem como o custo total da
            viagem.
        */
        void imprimeMelhorCaminho(int custo, Rota[] melhorRota)
        {
            int i; /* indexa o vetor que contem a rota */
            Console.WriteLine("\n\nCUSTO MINIMO PARA A VIAGEM DO CAIXEIRO: " + custo);
            Console.WriteLine("\n\nMELHOR CAMINHO PARA A VIAGEM DO CAIXEIRO:");
            Console.WriteLine("\n\n              DE               PARA             CUSTO ");
            for (i = 0; i < melhorRota.Length; i++)
            {
                Console.Write("              " + melhorRota[i].cidade1 + "                  " + melhorRota[i].cidade2 + "                " + melhorRota[i].custo +"\n");
            }
            Console.WriteLine("\n");
        } /* fim ImprimeMelhorCaminho */

        //------------------------------------------------------------------------------

        void imprimeTempo(Stopwatch tempo)
        {
            Console.WriteLine("TEMPO DE EXECUÇÂO: ");
            Console.WriteLine(tempo.Elapsed.Hours+" horas "+tempo.Elapsed.Minutes+" minutos "+tempo.Elapsed.Seconds+" segundos "+tempo.Elapsed.Milliseconds+" milisegundos");
        } /* fim Imprime tempo */

        //------------------------------------------------------------------------------

        
        /* Coordena as partes do programa */
        static void Main(string[] args)
        {
            int[] permutacao;     /* vetor com uma possivel rota de viagem */
            Rota[] melhorRota;    /* contera' a melhor rota da viagem */
            int numCidades,      /* numero de vertices (cidades) do grafo */
                    melhorCusto;     /* custo da viagem pelo grafo (pelas cidades) */

            CaixeiroOtimo caixeiro = new CaixeiroOtimo();
            Grafo grafo; // = new Grafo();         /* matriz de adjacencia com o grafo */

            Console.Write("\n\n\t\tDigite o numero de cidades: ");
            numCidades = int.Parse(Console.ReadLine());
            caixeiro.montaGrafo(out grafo, numCidades);

            permutacao = new int[numCidades];
            melhorRota = new Rota[numCidades];

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();//Inicia a contagem do tempo
            caixeiro.geraEscolheCaminhos(ref permutacao, grafo, melhorRota, out melhorCusto);
            stopwatch.Stop();//Encerra a contagem do tempo
            caixeiro.imprimeMelhorCaminho(melhorCusto, melhorRota);
            caixeiro.imprimeTempo(stopwatch);

            Console.ReadKey(true);
        } /* fim main */
    }
}
