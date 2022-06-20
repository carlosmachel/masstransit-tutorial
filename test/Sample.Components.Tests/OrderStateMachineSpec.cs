﻿using MassTransit;
using MassTransit.Testing;
using Sample.Components.StateMachines;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.Tests;

public class OrderStateMachineSpec
{

    [Test]
    public async Task Should_create_a_state_instance()
    {
        var orderStateMachine = new OrderStateMachine();
        var harness = new InMemoryTestHarness();
        var saga = harness.StateMachineSaga<OrderState,OrderStateMachine>(orderStateMachine);

        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();

            await harness.Bus.Publish<IOrderSubmited>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "12345"
            });

            Assert.That(saga.Created.Select(x => x.CorrelationId == orderId).Any(), Is.True);

            var instanceId = await saga.Exists(orderId, x => x.Submitted);
            Assert.That(instanceId, Is.Not.Null);

            var instance = saga.Sagas.Contains(instanceId.Value);

            Assert.That(instance.CustomerNumber, Is.EqualTo("12345"));
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task Should_responde_to_status_checks()
    {
        var orderStateMachine = new OrderStateMachine();
        var harness = new InMemoryTestHarness();
        var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);

        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();

            await harness.Bus.Publish<IOrderSubmited>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "12345"
            });

            Assert.That(saga.Created.Select(x => x.CorrelationId == orderId).Any(), Is.True);

            var instanceId = await saga.Exists(orderId, x => x.Submitted);
            Assert.That(instanceId, Is.Not.Null);

            var requestClient = await harness.ConnectRequestClient<ICheckOrder>();

            var response = await requestClient.GetResponse<IOrderStatus>(new { OrderId = orderId });

            Assert.That(response.Message.State, Is.EqualTo(orderStateMachine.Submitted.Name));
        }
        finally
        {
            await harness.Stop();
        }
    }
}
