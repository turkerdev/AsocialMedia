using System.Text.Json;

namespace ytdlservice.Queue;

abstract class Consumer<T>
{
    public static T Parse(string RawMessage)
    {
        var message = JsonSerializer.Deserialize<T>(RawMessage, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (message == null)
        {
            throw new Exception("Invalid message");
        }

        return message;
    }

    public abstract Task Handle(T message);
}