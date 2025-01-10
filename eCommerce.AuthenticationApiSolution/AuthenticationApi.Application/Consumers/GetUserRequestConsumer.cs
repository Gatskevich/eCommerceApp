using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using eCommerce.SharedLibrary.DTOs.Requests;
using eCommerce.SharedLibrary.DTOs.Responses;
using MassTransit;

namespace AuthenticationApi.Application.Consumers;

public class GetUserRequestConsumer(IUser userInterface) : IConsumer<GetUserRequest>
{
    public async Task Consume(ConsumeContext<GetUserRequest> context)
    {
        var userId = context.Message.UserId;

        if (userId <= 0)
        {
            await context.RespondAsync<AppUserDTO>(null!);

            return;
        }

        var user = await userInterface.GetUser(userId);

        await context.RespondAsync(new GetUserDTO(
            user.Id,
            user.Name,
            user.TelephoneNumber,
            user.Address,
            user.Email,
            user.Role));
    }
}
