using System.Diagnostics;
using System.Linq;

namespace AlgoritmosMTPP_CS
{
    internal class CalculaCor2
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
            (List<int>, List<List<int>>) filhos = BuscaFilhos(grafo, idVertice);

            foreach (int filhoArvore in filhos.Item1)
            {
                if (grafo.vertices[idVertice].listaCoresBase.Find(tupla => tupla.Item1 == filhoArvore) == null)
                    grafo.vertices[idVertice].listaCoresBase.Add(new Tuple<int, List<int>> (filhoArvore, new List<int> {grafo.vertices[idVertice].idCor, grafo.vertices[filhoArvore].idCor}));

                //grafo.vertices[idVertice].listaCoresAtual.Add(new Tuple<int, List<int>>(filhoArvore, new List<int>(new HashSet<int>((ant.HasValue ? grafo.vertices[ant.Value].listaCoresBase.Find(tupla => tupla.Item1 == idVertice).Item2 : new List<int>()).Concat(grafo.vertices[idVertice].listaCoresBase.Find(tupla => tupla.Item1 == filhoArvore).Item2)))));

                var listaCoresBaseAnt = ant.HasValue
                    ? grafo.vertices[ant.Value].listaCoresBase.Find(tupla => tupla.Item1 == idVertice)?.Item2 ?? new List<int>()
                    : new List<int>();

                var listaCoresBaseAtual = grafo.vertices[idVertice].listaCoresBase.Find(tupla => tupla.Item1 == filhoArvore)?.Item2 ?? new List<int>();

                grafo.vertices[idVertice].listaCoresAtual.Add(
                    new Tuple<int, List<int>>(filhoArvore, new List<int>(new HashSet<int>(listaCoresBaseAnt.Concat(listaCoresBaseAtual))))
                );


                CalculaCaminho(filhoArvore, idVertice, grafo);
            }

            foreach (var filhosCiclo in filhos.Item2)
            {
                filhosCiclo.Add(idVertice);
                foreach (var v in filhosCiclo)
                {
                    if (v != idVertice)
                    {
                        if (grafo.vertices[idVertice].listaCoresBase.Find(tupla => tupla.Item1 == v) == null)
                            grafo.vertices[idVertice].listaCoresBase.Add(new Tuple<int, List<int>>(v, new HashSet<int> { grafo.vertices[idVertice].idCor }.Concat(ConjuntoCores(grafo, filhosCiclo, idVertice, v)).ToList()));

                        //grafo.vertices[idVertice].listaCoresAtual.Add(new Tuple<int, List<int>>(v, new List<int>(new HashSet<int>((ant.HasValue ? grafo.vertices[ant.Value].listaCoresAtual.Find(tupla => tupla.Item1 == idVertice).Item2 : new List<int>()).Concat(grafo.vertices[idVertice].listaCoresAtual.Find(tupla => tupla.Item1 == v).Item2)))));
                        var listaCoresAnt = ant.HasValue
                            ? grafo.vertices[ant.Value].listaCoresAtual.Find(tupla => tupla.Item1 == idVertice)?.Item2 ?? new List<int>()
                            : new List<int>();

                        var listaCoresAtual = grafo.vertices[idVertice].listaCoresAtual.Find(tupla => tupla.Item1 == v)?.Item2 ?? new List<int>();

                        grafo.vertices[idVertice].listaCoresAtual.Add(
                            new Tuple<int, List<int>>(v, new List<int>(new HashSet<int>(listaCoresAnt.Concat(listaCoresAtual))))
                        );

                        CalculaCaminho(v, idVertice, grafo);
                    }
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
            List<int> maiorListaCores = new List<int>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //foreach (int folha in verticesFolha)
            for (int folha = 0; folha < grafo.vertices.Count(); folha++)
            {
                grafo.idHistorico.Add(folha);
                CalculaCaminho(folha, null, grafo);
                grafo.idHistorico.Clear();

                foreach (var i in grafo.vertices)
                {
                    foreach (var j in i.listaCoresAtual)
                    {
                        if (j.Item2.Count > maiorListaCores.Count)
                        {
                            maiorListaCores = j.Item2;
                        }
                    }
                }

                if (maiorListaCores.Count == grafo.nCores)
                    break;

                grafo.limpaListaAtual();
            }

            stopwatch.Stop();
            double tempoExecucao = stopwatch.Elapsed.TotalSeconds;
            return (maiorListaCores.Count, tempoExecucao);
        }
    }
}
