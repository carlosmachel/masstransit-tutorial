using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Sample.Contracts;

namespace Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IRequestClient<ISubmitOrder> _submitOrderRequestClient;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public OrderController(ILogger<OrderController> logger, 
        IRequestClient<ISubmitOrder> submitOrderRequestClient,
        ISendEndpointProvider sendEndpointProvider)
    {
        _logger = logger;
        _submitOrderRequestClient = submitOrderRequestClient;
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Guid id, string customerNumber)
    {
        var response = await _submitOrderRequestClient.GetResponse<IOrderSubmissionAccepted, IOrderSubmissionRejected>(new
        {
            OrderId = id,
            InVar.Timestamp,
            CustomerNumber = customerNumber
        });
       
        //Passar Accepted nesses cenários.
        return Accepted(response.Message);
    }

    [HttpPut]
    public async Task<IActionResult> Put(Guid id, string customerNumber)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:submit-order"));

        await endpoint.Send<ISubmitOrder>(new
        {
            OrderId = id,
            InVar.Timestamp,
            CustomerNumber = customerNumber
        });

        //Passar Accepted nesses cenários.
        return Accepted();
    }
}
