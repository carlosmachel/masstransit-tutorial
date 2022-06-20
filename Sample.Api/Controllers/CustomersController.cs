using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Sample.Contracts;

namespace Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{

    public IPublishEndpoint _publishEndpoint;

    public CustomersController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }


    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id, string customerNumber)
    {
        await _publishEndpoint.Publish<ICustomerAccountClosed>(new
        {
            CustomerId = id,
            CustomerNumber = customerNumber
        });

        return Ok();
    }
}