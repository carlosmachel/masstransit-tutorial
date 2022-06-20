﻿using MassTransit;

namespace Sample.Components.StateMachines;

public class OrderState : SagaStateMachineInstance
{
    //Unique identifier.
    public Guid CorrelationId { get; set ; }
    public int Version { get; set; }
    public string CurrentState { get; set; }
    public string CustomerNumber { get; set; }
    public DateTime Updated { get; set; }
    public DateTime SubmitDate { get;  set; }
}
