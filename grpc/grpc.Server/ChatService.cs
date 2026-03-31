using Grpc.Core;
using Grpc.Example;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace grpc.Server;

public class ChatService : Grpc.Example.ChatService.ChatServiceBase
{
    private static readonly string StartTime = DateTime.UtcNow.ToString("O");

    private readonly ILogger<ChatService> _logger;


    private static readonly ConcurrentDictionary<string, TaskCompletionSource<ChatMessage>> _pendingRequests = new();
    private static readonly ConcurrentDictionary<string, ChannelWriter<ChatMessage>> _activeStreams = new();

    public ChatService(ILogger<ChatService> logger)
    {
        _logger = logger;
    }

    public override async Task<ServerInfoResponse> GetServerInfo(
        ServerInfoRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("GetServerInfo called by: {Peer}", context.Peer);

        var response = new ServerInfoResponse
        {
            ServerName = "gRPC Chat Server",
            Version = "1.0.0",
            StartTime = StartTime
        };
        var userId = context.Peer;
        //if (_activeStreams.TryGetValue(userId, out var writer))
        //{
        //    var reply = new ChatMessage
        //    {
        //        User = "Server",
        //        Content = $"Echo from server: GetServerInfo",
        //        Timestamp = DateTime.UtcNow.ToString("O")
        //    };

        //    await writer.WriteAsync(reply);
        //}

        var msg = await GetMessageAsync(userId, new ChatMessage
        {
            RequestId = Guid.NewGuid().ToString(),
            User = "Server",
            Content = "Hello from server!: GetServerInfo",
            Timestamp = DateTime.UtcNow.ToString("O")
        });
        var msg2 = await GetMessageAsync(userId, new ChatMessage
        {
            RequestId = Guid.NewGuid().ToString(),
            User = "Server2",
            Content = "Hello from server2!: GetServerInfo",
            Timestamp = DateTime.UtcNow.ToString("O")
        });

        return await Task.FromResult(response);
    }

    public override async Task Chat(
        IAsyncStreamReader<ChatMessage> requestStream,
        IServerStreamWriter<ChatMessage> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("Client connected: {Peer}", context.Peer);

        var userId = context.Peer;
        var channel = Channel.CreateUnbounded<ChatMessage>();
        _activeStreams[userId] = channel.Writer;

        var sendTask = Task.Run(async () =>
        {
            await foreach (var msg in channel.Reader.ReadAllAsync())
            {
                await responseStream.WriteAsync(msg);
            }
        });

        await foreach (var message in requestStream.ReadAllAsync(context.CancellationToken))
        {
            _logger.LogInformation("[{User}]: {Content}", message.User, message.Content);
            if (_pendingRequests.TryRemove(message.RequestId, out var tcs))
            {
                tcs.SetResult(message);
            }
            //var reply = new ChatMessage
            //{
            //    User = "Server",
            //    Content = $"Echo from server: [{message.User}] said \"{message.Content}\"",
            //    Timestamp = DateTime.UtcNow.ToString("O")
            //};

            //await responseStream.WriteAsync(reply);
        }
        //await sendTask;

        _logger.LogInformation("Client disconnected: {Peer}", context.Peer);
    }


    public async Task<ChatMessage> GetMessageAsync(string clientId, ChatMessage msg)
    {
        if (_activeStreams.TryGetValue(clientId, out var writer))
        {
            var tcs = new TaskCompletionSource<ChatMessage>();
            _pendingRequests[msg.RequestId] = tcs;
            await writer.WriteAsync(msg);
            return await tcs.Task;
        }

        return null;
    }
}
