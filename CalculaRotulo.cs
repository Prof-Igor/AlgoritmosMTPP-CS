using System.Diagnostics;

namespace AlgoritmosMTPP_CS
{
    internal class CalculaRotulo
    {
        public static bool EFolha(Grafo G, int u)
        {
            foreach (var item in G.idArestas[u])
            {
                if (!G.idHistorico.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        public static List<int> VerificaCiclo(Grafo G, int u, List<int> ciclo, int ant, List<int> historico)
        {
            if (ciclo.Contains(u))
                return ciclo;

            foreach (int filho in G.idArestas[ciclo.Last()])
            {
                if (ciclo.Contains(u))
                    return ciclo;
                else if (filho == ant)
                    continue;
                else if (historico.Contains(filho))
                    break;

                ciclo.Add(filho);
                historico.Add(filho);
                VerificaCiclo(G, u, ciclo, ciclo[ciclo.Count - 2], historico);
            }

            if (ciclo.Contains(u))
                return ciclo;
            else
            {
                ciclo.RemoveAt(ciclo.Count - 1);
                return ciclo;
            }
        }

        public static Tuple<List<int>, List<List<int>>> Filhos(Grafo G, int u)
        {
            List<int> filhosArvore = new List<int>();
            List<List<int>> filhosCiclo = new List<List<int>>();

            foreach (int filho in G.idArestas[u])
            {
                if (!G.idHistorico.Contains(filho))
                {
                    List<int> ciclo = new List<int>();
                    ciclo.Add(filho);
                    List<int> historico = new List<int>();
                    historico.Add(filho);
                    VerificaCiclo(G, u, ciclo, u, historico);
                    if (ciclo.Count == 0)
                    {
                        filhosArvore.Add(filho);
                        G.idHistorico.Add(filho);
                    }
                    else
                    {
                        ciclo.RemoveAt(ciclo.Count - 1);
                        List<int> listaCiclo = new List<int>();
                        foreach (int nos in ciclo)
                        {
                            listaCiclo.Add(nos);
                            G.idHistorico.Add(nos);
                        }
                        filhosCiclo.Add(listaCiclo);
                    }
                }
            }

            for (int i = 0; i < filhosCiclo.Count; i++)
            {
                for (int j = 0; j < filhosCiclo.Count; j++)
                {
                    if (filhosCiclo[i][0] == filhosCiclo[j][filhosCiclo[j].Count - 1] && filhosCiclo[i][filhosCiclo[i].Count - 1] == filhosCiclo[j][0])
                    {
                        filhosCiclo.RemoveAt(j);
                    }
                }
            }

            return new Tuple<List<int>, List<List<int>>>(filhosArvore, filhosCiclo);
        }

        public static List<int> Sequencia(Grafo G, List<int> ciclo, List<int> sequencia, int melhorVertice)
        {
            foreach (var item in G.idArestas[sequencia.Last()])
            {
                if (ciclo.Contains(item) && !sequencia.Contains(item) && item != melhorVertice)
                {
                    sequencia.Add(item);
                    Sequencia(G, ciclo, sequencia, melhorVertice);
                }
            }
            return sequencia;
        }

        public static List<int> Aux(Grafo G, List<int> ciclo, int Pai)
        {
            List<List<int>> ConjuntosCores = new();

            foreach (var item in ciclo)
            {
                List<int> vizinhos = new();
                foreach (var vert in G.idArestas[item])
                {
                    if (ciclo.Contains(vert))
                        vizinhos.Add(vert);
                }

                if (vizinhos.Count == 1)
                {
                    List<int> CoresCiclo = new();
                    CoresCiclo.AddRange(G.vertices[item].idCores);

                    foreach (var vert in ciclo)
                    {
                        if (!CoresCiclo.Contains(G.vertices[vert].idCor))
                            CoresCiclo.Add(G.vertices[vert].idCor);
                    }

                    if (!CoresCiclo.Contains(G.vertices[Pai].idCor))
                        CoresCiclo.Add(G.vertices[Pai].idCor);

                    ConjuntosCores.Add(CoresCiclo);
                }
                else
                {
                    List<int> SequenciaA = new List<int>();
                    SequenciaA.Add(vizinhos[0]);
                    Sequencia(G, ciclo, SequenciaA, item);

                    List<int> CoresLadoA = new List<int>();
                    CoresLadoA.AddRange(G.vertices[item].idCores);

                    foreach (var vert in SequenciaA)
                    {
                        if (!CoresLadoA.Contains(G.vertices[vert].idCor))
                            CoresLadoA.Add(G.vertices[vert].idCor);
                    }

                    if (!CoresLadoA.Contains(G.vertices[Pai].idCor))
                        CoresLadoA.Add(G.vertices[Pai].idCor);

                    ConjuntosCores.Add(CoresLadoA);

                    List<int> SequenciaB = new List<int>();
                    SequenciaB.Add(vizinhos[1]);
                    Sequencia(G, ciclo, SequenciaB, item);

                    List<int> CoresLadoB = new List<int>();
                    CoresLadoB.AddRange(G.vertices[item].idCores);

                    foreach (var vert in SequenciaB)
                    {
                        if (!CoresLadoB.Contains(G.vertices[vert].idCor))
                            CoresLadoB.Add(G.vertices[vert].idCor);
                    }

                    if (!CoresLadoB.Contains(G.vertices[Pai].idCor))
                        CoresLadoB.Add(G.vertices[Pai].idCor);

                    ConjuntosCores.Add(CoresLadoB);
                }
            }

            return ConjuntosCores.OrderBy(x => x.Count()).Last();
        }

        public static int MelhoresFilhos(List<List<int>> CoresFilhos)
        {
            List<int> Melhores = new List<int>();
            foreach (var item in CoresFilhos[0])
            {
                Melhores.Add(item);
            }

            for (int i = 0; i < CoresFilhos.Count; i++)
            {
                for (int j = i + 1; j < CoresFilhos.Count; j++)
                {
                    List<int> Atual = new List<int>();
                    Atual.AddRange(CoresFilhos[i]);
                    foreach (var item in CoresFilhos[j])
                    {
                        if (!Atual.Contains(item))
                            Atual.Add(item);
                    }

                    if (Atual.Count > Melhores.Count)
                        Melhores = Atual;
                }
            }

            return Melhores.Count;
        }

        public static void ComputaCores(Grafo G, int u)
        {
            if (EFolha(G, u))
            {
                G.rotulos[u] = new Tuple<int, int>(1, 1);
                if (!G.vertices[u].idCores.Contains(G.vertices[u].idCor))
                    G.vertices[u].idCores.Add(G.vertices[u].idCor);
            }
            else
            {
                Tuple<List<int>, List<List<int>>> filhos = Filhos(G, u);

                List<List<int>> CoresFilhos = new List<List<int>>();

                foreach (int raiz in filhos.Item1)
                {
                    ComputaCores(G, raiz);
                    CoresFilhos.Add(G.vertices[raiz].idCores.Distinct().ToList());

                    if (!CoresFilhos.Last().Contains(G.vertices[u].idCor))
                        CoresFilhos.Last().Add(G.vertices[u].idCor);
                }

                foreach (List<int> ciclos in filhos.Item2)
                {
                    foreach (int raiz in ciclos)
                    {
                        ComputaCores(G, raiz);
                    }

                    CoresFilhos.Add(Aux(G, ciclos, u).Distinct().ToList());
                }

                List<int> MaiorCores = CoresFilhos.OrderBy(x => x.Count()).Last();
                G.rotulos[u] = new Tuple<int, int>(MaiorCores.Count, MelhoresFilhos(CoresFilhos));
                foreach (var item in MaiorCores.Distinct().ToList())
                {
                    if (!G.vertices[u].idCores.Contains(item))
                    {
                        G.vertices[u].idCores.Add(item);
                    }
                }
            }
        }

        public static (int, double) Rotular(Grafo grafo)
        {

            int Max = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int j = 0; j < grafo.vertices.Count(); j++)
            {
                grafo.idHistorico.Clear();
                for (int k = 0; k < grafo.vertices.Count(); k++)
                {
                    grafo.rotulos[k] = new Tuple<int, int>(0, 0);
                    grafo.vertices[k].idCores.Clear();
                }
                grafo.idHistorico.Add(j);
                ComputaCores(grafo, j);
                foreach (var item in grafo.rotulos)
                {
                    if (item.Item2 > Max)
                        Max = item.Item2;
                }
                if (Max == grafo.nCores)
                    break;
            }

            stopwatch.Stop();
            double tempoExecucao = stopwatch.Elapsed.TotalSeconds;
            return (Max, tempoExecucao);
        }
    }
}
