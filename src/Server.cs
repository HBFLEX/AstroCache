using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;


public class Server
{
    private const int Port = 6379;
    private TcpListener? _server;
    private Socket? _client;
    private byte[]? _buffer;
    private readonly Dictionary<string, (string, long?)> _store = new Dictionary<string, (string, long?)>();
    private string? _hostDir;
    private string? _hostFileName;
    private CommandParser? _fileParser;
    
    public static async Task Main(string[] args)
    {
        
        var app = new Server();
        // initiate command line arguments parsing
        app._fileParser = new CommandParser(args);
        app._hostDir = app._fileParser.GetHostDir(); // get db directory 
        app._hostFileName = app._fileParser.GetHostFileName(); // get db file name
        // read db on startup
        app.ReadDatabase(app._hostDir, app._hostFileName);
        // start server
        await app.Start();
    }

    private void ReadDatabase(string directory, string fileName)
    {
        int byteOffset = 0;
        int headerFirstBytes = 9;
        
        string db= Path.Combine(directory, fileName);

        if (!File.Exists(db))
        {
            Console.Error.WriteLine("Database file not found: {0}", db);
            return;
        }
        
        byte[] dbContent = File.ReadAllBytes(db);
        string headerInfo = Encoding.UTF8.GetString(dbContent, byteOffset, headerFirstBytes);
        byteOffset += 9;
        
        if (headerInfo != "REDIS0011")
        {
            throw new Exception($"Invalid header version: {headerInfo}");
        }
        
        Console.WriteLine("HEADER LOOKS GOOD");
    }

    private async Task Start()
    {
        // create a new tcp server
        _server = CreateServer();
        _server.Start();
        Console.WriteLine($"Server started on port: {Port}");
        
        // handle multiple client connections
        while (true)
        {
            _client = await _server!.AcceptSocketAsync();
            HandleClientRequest(_client);
        }
    }

    private TcpListener CreateServer()
    {
        return new TcpListener(IPAddress.Any, Port);
    }

    private async void HandleClientRequest(Socket client)
    {
        byte[] buffer = new byte[1024];
        int bytesRead;
        // persist the connection
        while (client.Connected)
        {
            bytesRead = await client.ReceiveAsync(buffer);

            if (bytesRead > 0)
            {
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                AstroParser parser = new AstroParser(_store, _hostDir!, _hostFileName!); // pass in the store for every request to persist data
                string response = parser.ParseCommand(request);
                SendMessage(client, response);
            }
            else
            {
                Console.WriteLine("No data was received from client");
            }
        }
        // close the connection after client disconnects
        client.Close();
    }

    private async void SendMessage(Socket client, string message)
    {
        await client.SendAsync(Encoding.UTF8.GetBytes(message), SocketFlags.None);
    }
    
}









