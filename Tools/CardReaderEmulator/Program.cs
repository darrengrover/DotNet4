using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

class Program
{
    static bool _verbose = false;

    static async Task Main(string[] args)
    {
        int port = 5500;
        if (args.Length > 0 && int.TryParse(args[0], out var p))
        {
            port = p;
        }

        var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();
        Console.WriteLine($"Card reader emulator listening on {port}");
        Console.WriteLine("Commands: [hex card ID] to queue card, 'verbose' to toggle debug output");
        Console.WriteLine();

        var cards = new ConcurrentQueue<string>();
        _ = Task.Run(() =>
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                line = line.Trim();

                if (line.Equals("verbose", StringComparison.OrdinalIgnoreCase))
                {
                    _verbose = !_verbose;
                    Console.WriteLine($">>> Verbose logging: {(_verbose ? "ON" : "OFF")}");
                    continue;
                }

                if (line.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    line = line.Substring(2);

                cards.Enqueue(line.ToUpperInvariant());
                Console.WriteLine($">>> Queued card: {line}");
            }
        });

        while (true)
        {
            using var client = await listener.AcceptTcpClientAsync();
            Console.WriteLine(">>> Client CONNECTED");
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.ASCII);
            using var writer = new StreamWriter(stream, Encoding.ASCII) { NewLine = "\r\n", AutoFlush = true };

            while (client.Connected)
            {
                var cmd = await reader.ReadLineAsync();
                if (cmd == null)
                    break;

                // ONLY log if verbose mode is ON
                if (_verbose)
                    Console.WriteLine($"RX: {cmd}");

                if (cmd.StartsWith("rfid:qid.id", StringComparison.OrdinalIgnoreCase))
                {
                    if (!cards.TryDequeue(out var card))
                    {
                        await writer.WriteLineAsync("{0x0000,0,0x00,0;0x00}");
                    }
                    else
                    {
                        await writer.WriteLineAsync("{0x0000,0,0x00,0;0x" + card + "}");
                        Console.WriteLine($">>> SENT card: {card}");
                    }
                }
                else if (cmd.StartsWith("rfid:dev.luid?", StringComparison.OrdinalIgnoreCase))
                {
                    await writer.WriteLineAsync("rfid:dev.luid=1");
                    if (_verbose)
                        Console.WriteLine("TX: rfid:dev.luid=1");
                }
                else
                {
                    await writer.WriteLineAsync("OK");
                    if (_verbose)
                        Console.WriteLine("TX: OK");
                }
            }
            Console.WriteLine(">>> Client DISCONNECTED");
        }
    }
}