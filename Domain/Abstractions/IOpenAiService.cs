public interface IOpenAiService
{
    Task<string> GetBotResponse(string initialContext, string prompt, Guid chatId);
}
