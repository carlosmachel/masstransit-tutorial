using MassTransit;
using MassTransit.Testing;
using Sample.Components.Consumers;
using Sample.Contracts;

namespace Sample.Components.Tests;

public class When_an_order_request_is_consumed
{

    [Test]
    public async Task Should_responde_with_acceptance_if_ok()
    {
        var harness = new InMemoryTestHarness();
        var consumer = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();

            var requestClient = await harness.ConnectRequestClient<ISubmitOrder>();

            var response = await requestClient.GetResponse<IOrderSubmissionAccepted>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "12345"
            });

            Assert.That(response.Message.OrderId, Is.EqualTo(orderId));
            Assert.That(consumer.Consumed.Select<ISubmitOrder>().Any(), Is.True);
            Assert.That(harness.Sent.Select<IOrderSubmissionAccepted>().Any(), Is.True);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task Should_responde_with_rejection_if_test()
    {
        var harness = new InMemoryTestHarness();
        var consumer = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();

            var requestClient = await harness.ConnectRequestClient<ISubmitOrder>();

            var response = await requestClient.GetResponse<IOrderSubmissionRejected>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "TEST12345"
            });

            Assert.That(response.Message.OrderId, Is.EqualTo(orderId));
            Assert.That(consumer.Consumed.Select<ISubmitOrder>().Any(), Is.True);
            Assert.That(harness.Sent.Select<IOrderSubmissionRejected>().Any(), Is.True);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task Should_consume_submit_order_command()
    {
        var harness = new InMemoryTestHarness();
        var consumer = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();

            await harness.InputQueueSendEndpoint.Send<ISubmitOrder>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "12345"
            });

            Assert.That(consumer.Consumed.Select<ISubmitOrder>().Any(), Is.True);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task Should_publish_order_submitted_event()
    {
        var harness = new InMemoryTestHarness();
        var consumer = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();

            await harness.InputQueueSendEndpoint.Send<ISubmitOrder>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "12345"
            });

            Assert.That(harness.Published.Select<IOrderSubmited>().Any(), Is.True);
        }
        finally
        {
            await harness.Stop();
        }
    }


    [Test]
    public async Task Should_not_publish_order_submitted_event_when_rejected()
    {
        var harness = new InMemoryTestHarness();
        var consumer = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();

            await harness.InputQueueSendEndpoint.Send<ISubmitOrder>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "TEST12345"
            });

            Assert.That(harness.Published.Select<IOrderSubmited>().Any(), Is.False);
        }
        finally
        {
            await harness.Stop();
        }
    }
}
