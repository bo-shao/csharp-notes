using Grpc.Core;
using Grpc.Example;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("https://localhost:63985/");
var client = new ChatService.ChatServiceClient(channel);


// Establish a bidirectional streaming connection
using var call = client.Chat();

// Start a background task to continuously receive messages from the server
var readTask = Task.Run(async () =>
{
    await foreach (var message in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"[{message.Timestamp}] {message.User}: {message.Content}");


        await call.RequestStream.WriteAsync(new ChatMessage
        {
            RequestId = message.RequestId,
            User = "Client",
            Content = "AAA - Client",
            Timestamp = DateTime.UtcNow.ToString("O")
        });
    }
});

Console.WriteLine("Connected to gRPC server. Type a message to send, or 'exit' to quit.");
await call.RequestStream.WriteAsync(new ChatMessage
{
    User = "Client",
    Content = "AAA",
    Timestamp = DateTime.UtcNow.ToString("O")   
});

// Call the new unary RPC to get server info
var serverInfo = await client.GetServerInfoAsync(new ServerInfoRequest());
Console.WriteLine($"Server: {serverInfo.ServerName} v{serverInfo.Version}, started at {serverInfo.StartTime}");

serverInfo = await client.GetServerInfoAsync(new ServerInfoRequest());
Console.WriteLine($"Server: {serverInfo.ServerName} v{serverInfo.Version}, started at {serverInfo.StartTime}");

//// Main thread: read user input and send messages
//while (true)
//{
//    var input = Console.ReadLine();
//    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
//        break;

//    await call.RequestStream.WriteAsync(new ChatMessage
//    {
//        User = "Client",
//        Content = input,
//        Timestamp = DateTime.UtcNow.ToString("O")
//    });
//}

// Notify the server that the client has finished sending
//await call.RequestStream.CompleteAsync();

// Wait for all remaining server messages to be received
//await readTask;

Console.WriteLine("Connection closed.");

Console.ReadKey();