namespace AlgoritmosMTPP_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Seja bem vindo ao sistema de algoritmos para o Problema do Caminho Tropical Máximo MTPP, desenvolvido por Igor de Moraes Sampaio!");

            while (true)
            {
                Console.WriteLine("Escolha o algoritmo você que executar: \n 1 - Heurística \n 2 - Programação Dinamica \n 0 - Sair \n Digite a opção: ");
                string algoritmoEscolhido = Console.ReadLine();

                if (algoritmoEscolhido == "1" || algoritmoEscolhido == "2")
                {
                    Console.WriteLine("Executar quais instâncias: \n 1 - Grafos \n 2 - Grafos Cactos \n 3 - Voltar \n 0 - Sair \n Digite a opção: ");
                    string adaptar = Console.ReadLine();

                    if (adaptar == "1" || adaptar == "2")
                    {
                        ProcessarInstancias(algoritmoEscolhido, adaptar);
                        break;
                    }
                    else if (adaptar == "3")
                    {
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Valor selecionado é inválido! Tente novamente!");
                    }
                }
                else if (algoritmoEscolhido == "0")
                {
                    Console.WriteLine("Saindo do sistema.");
                    break;
                }
                else
                {
                    Console.WriteLine("Valor selecionado é inválido! Tente novamente!");
                }
            }

        }
        static void ProcessarInstancias(string algoritmoEscolhido, string adaptar)
        {
            string arquivoInstancias = adaptar == "1" ? "../../../Instancias/MTPP/nomes.txt" : "../../../Instancias/Cacto/nomes.txt";

            using (StreamReader instancias = new StreamReader(arquivoInstancias))
            {
                int numInstancias = int.Parse(instancias.ReadLine().Trim());
                string caminhoResultado = adaptar == "1" ? "../../../Resultados/MTPP/" : "../../../Resultados/Cacto/";
                using (StreamWriter escreveResultado = new StreamWriter(caminhoResultado + "TropicalAlg " + DateTime.Now.ToString().Replace("/", "-").Replace(":", ".") + ".txt", true))
                {
                    for (int i = 0; i < numInstancias; i++)
                    {
                        string nomeGrafo = instancias.ReadLine().Trim();
                        escreveResultado.Write($"{nomeGrafo}-");
                        string arquivoInstancia = adaptar == "1" ? $"../../../Instancias/MTPP/{nomeGrafo}.txt" : $"../../../Instancias/Cacto/{nomeGrafo}.txt";
                        var grafo = Grafo.CriarGrafo(arquivoInstancia);
                        var (resposta, tempoExecucao) = ExecutarAlgoritmo(grafo, algoritmoEscolhido, adaptar);
                        escreveResultado.WriteLine($"{resposta}-{tempoExecucao:F7}");
                        Console.WriteLine($"{resposta}-{tempoExecucao:F7}");
                    }
                }
            }
        }

        static (int, double) ExecutarAlgoritmo(Grafo grafo, string algoritmo, string adaptar)
        {
            Grafo grafoAdaptado = adaptar == "1" ? Grafo.AdaptarGrafo(grafo) : grafo;

            if (algoritmo == "1")
            {
                return CalculaRotulo.Rotular(grafoAdaptado);
            }
            else if (algoritmo == "2")
            {
                return CalculaCor.CalculaCacto(grafoAdaptado);
            }
            return (0, 0);
        }
    }
}
