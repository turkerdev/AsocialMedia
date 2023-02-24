namespace AsocialMedia.Worker;

public class Logger
{
    public static void Log(string log, params object[] args)
    {
        Console.WriteLine(log, args);
    }

    public static void Error(string log, params object[] args)
    {
        Log("ERROR: "+log, args);
    }
}