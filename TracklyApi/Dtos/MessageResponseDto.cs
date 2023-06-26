namespace TracklyApi.Dtos;
public class MessageResponseDto
{
    public string Message { get; set; } = null!;
    public MessageResponseDto(string message)
    {
        Message = message;
    }
}
