using System.Collections.Generic;

public class RequestSender : TcpSender
{
    public enum Request { HumanSegmentation, HandProcessor, HandGesture }
    HashSet<Request> requests = new();

    protected override int PORT => 8002;

    private void Start()
    {
        Add(Request.HumanSegmentation);
    }

    public void Add(Request request)
    {
        requests.Add(request);

        SendData($"({request}, {true})");
    }

    public void Remove(Request request)
    {
        requests.Remove(request);
    }
}