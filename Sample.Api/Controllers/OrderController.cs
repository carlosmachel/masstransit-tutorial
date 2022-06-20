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
    private readonly IRequestClient<ICheckOrder> _checkOrderRequestClient;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public OrderController(ILogger<OrderController> logger, 
        IRequestClient<ISubmitOrder> submitOrderRequestClient,
        IRequestClient<ICheckOrder> checkOrderRequestClient,
        ISendEndpointProvider sendEndpointProvider)
    {
        _logger = logger;
        _submitOrderRequestClient = submitOrderRequestClient;
            _checkOrderRequestClient = checkOrderRequestClient;
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
       
        //Passar Accepted nesses cen�rios.
        return Accepted(response.Message);
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        var (status, notFound) = await _checkOrderRequestClient.GetResponse<IOrderStatus, IOrderNotFound>(new { OrderId = id });

        if (status.IsCompletedSuccessfully)
        {
            var response = await status;
            return Ok(response.Message);
        }
        else
        {
            var response = await notFound;
            return NotFound(response.Message);
        }
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

        //Passar Accepted nesses cen�rios.
        return Accepted();
    }
}
