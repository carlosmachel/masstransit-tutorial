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
    private readonly IPublishEndpoint publishEndpoint;

    public OrderController(ILogger<OrderController> logger, 
        IRequestClient<ISubmitOrder> submitOrderRequestClient,
        IRequestClient<ICheckOrder> checkOrderRequestClient,
        ISendEndpointProvider sendEndpointProvider,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _submitOrderRequestClient = submitOrderRequestClient;
            _checkOrderRequestClient = checkOrderRequestClient;
        _sendEndpointProvider = sendEndpointProvider;
        this.publishEndpoint = publishEndpoint;
    }

    /*
    [HttpPost]
    public async Task<IActionResult> Post(Guid id, string customerNumber)
    {
        var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<IOrderSubmissionAccepted, IOrderSubmissionRejected>(new
        {
            OrderId = id,
            InVar.Timestamp,
            CustomerNumber = customerNumber
        });
       
        if(accepted.IsCompletedSuccessfully)
        {
            var response = await accepted;

            return Accepted(response.Message);
        }
        else
        {
            var response = await rejected;
            return NotFound(response.Message.Reason);
        }
    }
    */

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

    [HttpPost(), Route("accept-order")]
    public async Task<IActionResult> AcceptOrderAsync(Guid id)
    {
        await publishEndpoint.Publish<IOrderAccepted>(new
        {
            OrderId = id,
            InVar.Timestamp
        });

        return Accepted();
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOrderAsync(Guid id, string customerNumber)
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
