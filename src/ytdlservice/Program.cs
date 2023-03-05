using ytdlservice.Queue;

var QueueManager = new QueueManager();

QueueManager.Subscribe<Basic, BasicMessage>("dl");

Thread.Sleep(Timeout.Infinite);