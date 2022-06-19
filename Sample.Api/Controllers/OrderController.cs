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

    public OrderController(ILogger<OrderController> logger, IRequestClient<ISubmitOrder> submitOrderRequestClient)
    {
        _logger = logger;
        _submitOrderRequestClient = submitOrderRequestClient;
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
}
