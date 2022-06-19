using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Sample.Contracts;

namespace Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;

    public OrderController(ILogger<OrderController> logger, IRequestClient<SubmitOrder> submitOrderRequestClient)
    {
        _logger = logger;
        _submitOrderRequestClient = submitOrderRequestClient;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Guid id, string customerNumber)
    {
        var response = await _submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
        {
            OrderId = id,
            InVar.Timestamp,
            CustomerNumber = customerNumber
        });

        return Ok(response.Message);
    }
}
