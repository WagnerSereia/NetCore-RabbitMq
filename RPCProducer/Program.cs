using System;
using System.Threading;
using System.Threading.Tasks;

namespace RPCProducer
{
    class Program
    {
        private static readonly AutoResetEvent _waitHandle =
            new AutoResetEvent(false);

        static void Main(string[] args)
        {
            #region Cabecalho
            Console.WriteLine("####################################################");
            Console.WriteLine("#.....................RPC Producer.................#");
            Console.WriteLine("####################################################");
            #endregion

            #region Evento para sair
            Console.CancelKeyPress += (o, e) =>
            {
                Console.WriteLine("Saindo...");

                // Libera a continuação da thread principal
                _waitHandle.Set();
                e.Cancel = true;
            };
            #endregion

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Digite o valor para obter o Fibonacci");
                Console.WriteLine("Sugestão não ultrapassar 30");

                string n = Console.ReadLine();
                Task t = InvokeAsync(n);
                t.Wait();
            }
            _waitHandle.WaitOne();
        }

        private static async Task InvokeAsync(string n)
        {
            var rpcClient = new RpcClient();

            Console.WriteLine(" [x] Requesting fib({0})", n);
            var response = await rpcClient.CallAsync(n.ToString());
            Console.WriteLine(" [.] Got '{0}'", response);

            rpcClient.Close();
            Console.WriteLine("Pressione qualquer tecla para realizar novo calculo");
            Console.ReadKey();
        }
    }
}
