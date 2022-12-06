using Xunit;

namespace Perfy.UnitTests;

public class QueueTests
{
    [Fact]
    public void EnqueueString()
    {
        var queue = new EventQueue();
        queue.Register<string>();
        queue.Enqueue<string>("Hello World");
        var result = queue.Dequeue<string>();
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void EnqueueComplextType()
    {
        var queue = new EventQueue();
        queue.Register<A>();

        queue.Enqueue(new A{
            X = 10,
            Y= 10
        });

        var result = queue.Dequeue<A>();
        Assert.NotNull(result);
    }

    private class A
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

}
