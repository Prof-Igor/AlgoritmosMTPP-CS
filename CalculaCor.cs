using System.Diagnostics;

namespace AlgoritmosMTPP_CS
{
    internal class CalculaCor
    {
        public static List<int> VerificaCiclo(Grafo grafo, int u, List<int> ciclo, int ant, List<int> historico)
        {
            if (ciclo.Contains(u))
                return ciclo;

            foreach (int filho in grafo.idArestas[ciclo[ciclo.Count - 1]])
            {
                if (ciclo.Contains(u))
                    return ciclo;
                else if (filho == ant)
                    continue;
                else if (historico.Contains(filho))
                    break;

                ciclo.Add(filho);
                historico.Add(filho);
                VerificaCiclo(grafo, u, ciclo, ciclo[ciclo.Count - 2], historico);
            }

            if (ciclo.Contains(u))
                return ciclo;
            else
            {
                ciclo.RemoveAt(ciclo.Count - 1);
                return ciclo;
            }
        }

        public static (List<int>, List<List<int>>) BuscaFilhos(Grafo grafo, int raiz)
        {
            List<int> filhosArvore = new List<int>();
            List<List<int>> filhosCiclo = new List<List<int>>();

            foreach (int filho in grafo.idArestas[raiz])
            {
                if (!grafo.idHistorico.Contains(filho))
                {
                    List<int> ciclo = new List<int> { filho };
                    List<int> historico = new List<int> { filho };
                    VerificaCiclo(grafo, raiz, ciclo, raiz, historico);

                    if (ciclo.Count == 0)
                    {
                        filhosArvore.Add(filho);
                        grafo.idHistorico.Add(filho);
                    }
                    else
                    {
                        ciclo.RemoveAt(ciclo.Count - 1);
                        List<int> listaCiclo = new List<int>(ciclo);
                        filhosCiclo.Add(listaCiclo);
                        grafo.idHistorico.AddRange(ciclo);
                    }
                }
            }

            // Remove ciclos repetidos (em sentido inverso)
            for (int i = 0; i < filhosCiclo.Count; i++)
            {
                for (int j = 0; j < filhosCiclo.Count; j++)
                {
                    if (filhosCiclo[i][0] == filhosCiclo[j][filhosCiclo[j].Count - 1] &&
                        filhosCiclo[i][filhosCiclo[i].Count - 1] == filhosCiclo[j][0])
                    {
                        filhosCiclo.RemoveAt(j);
                    }
                }
            }

            return (filhosArvore, filhosCiclo);
        }

        public static List<int> ConjuntoCores(Grafo grafo, List<int> ciclo, int inicio, int fim)
        {
            int idxInicio = ciclo.IndexOf(inicio);

            List<int> caminhoDireita = new List<int>();
            int i = idxInicio;
            do
            {
                caminhoDireita.Add(grafo.vertices[ciclo[i]].idCor);
                if (ciclo[i] == fim) break;
                i = (i + 1) % ciclo.Count;
            } while (true);

            List<int> caminhoEsquerda = new List<int>();
            i = idxInicio;
            do
            {
                caminhoEsquerda.Add(grafo.vertices[ciclo[i]].idCor);
                if (ciclo[i] == fim) break;
                i = (i - 1 + ciclo.Count) % ciclo.Count;
            } while (true);

            return (new HashSet<int>(caminhoDireita).Count > new HashSet<int>(caminhoEsquerda).Count)
                ? new List<int>(new HashSet<int>(caminhoDireita))
                : new List<int>(new HashSet<int>(caminhoEsquerda));
        }

        public static void CalculaCaminho(int idVertice, int? ant, Grafo grafo)
        {
            var filhos = BuscaFilhos(grafo, idVertice);

            foreach (int filhoArvore in filhos.Item1)
            {
                if (grafo.calcBase[idVertice][filhoArvore].Count == 0)
                    grafo.calcBase[idVertice][filhoArvore] = new List<int> { grafo.vertices[idVertice].idCor, grafo.vertices[filhoArvore].idCor };

                grafo.calcAtual[idVertice][filhoArvore] = new List<int>(new HashSet<int>((ant.HasValue ? grafo.calcAtual[ant.Value][idVertice] : new List<int>())
                    .Concat(grafo.calcBase[idVertice][filhoArvore])));

                CalculaCaminho(filhoArvore, idVertice, grafo);
            }

            foreach (var filhosCiclo in filhos.Item2)
            {
                filhosCiclo.Add(idVertice);
                foreach (var v in filhosCiclo)
                {
                    if (v != idVertice)
                    {
                        if (grafo.calcBase[idVertice][v].Count == 0)
                            grafo.calcBase[idVertice][v] = new List<int> { grafo.vertices[idVertice].idCor }
                                .Concat(ConjuntoCores(grafo, filhosCiclo, idVertice, v)).ToList();

                        grafo.calcAtual[idVertice][v] = new List<int>(new HashSet<int>((ant.HasValue ? grafo.calcAtual[ant.Value][idVertice] : new List<int>())
                            .Concat(grafo.calcBase[idVertice][v])));
                    }
                }

                foreach (var filhoCiclo in filhosCiclo)
                {
                    CalculaCaminho(filhoCiclo, idVertice, grafo);
                }
            }
        }

        public static (int, double) CalculaCacto(Grafo grafo)
        {
            /*
            List<int> verticesFolha = new List<int>();
            for (int i = 0; i < grafo.idArestas.Count; i++)
            {
                if (grafo.idArestas[i].Count == 1)
                    verticesFolha.Add(i);
            }
            */
            List<string> maiorListaCores = new List<string>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //foreach (int folha in verticesFolha)
            for (int folha = 0; folha < grafo.vertices.Count(); folha++)
            {
                grafo.idHistorico.Add(folha);
                CalculaCaminho(folha, null, grafo);
                grafo.idHistorico.Clear();

                foreach (var i in grafo.calcAtual)
                {
                    foreach (var j in i)
                    {
                        List<string> listaAtual = new List<string>();
                        foreach (var k in j)
                        {
                            listaAtual.Add(grafo.cores[k]);
                        }

                        if (listaAtual.Count > maiorListaCores.Count)
                        {
                            maiorListaCores = listaAtual;
                        }
                    }
                }

                if (maiorListaCores.Count == grafo.nCores)
                    break;

                grafo.limpaCalcAtual();
            }

            stopwatch.Stop();
            double tempoExecucao = stopwatch.Elapsed.TotalSeconds;
            return (maiorListaCores.Count, tempoExecucao);
        }
    }
}
