namespace ChatBotApi.Model
{
    /// <summary>
    /// Represents available OpenAI models for chat completions
    /// </summary>
    public enum AvailableModel
    {
        /// <summary>
        /// GPT-3.5 Turbo model
        /// </summary>
        Gpt35Turbo,

        /// <summary>
        /// GPT-4 model
        /// </summary>
        Gpt4,

        /// <summary>
        /// GPT-4.1 model
        /// </summary>
        Gpt41
    }

    /// <summary>
    /// Extension methods for the AvailableModel enum
    /// </summary>
    public static class AvailableModelExtensions
    {
        /// <summary>
        /// Converts the model enum to the corresponding deployment name
        /// </summary>
        /// <param name="model">The model enum</param>
        /// <returns>The OpenAI deployment name</returns>
        public static string GetModelName(this AvailableModel model)
        {
            return model switch
            {
                AvailableModel.Gpt35Turbo => "gpt-3.5-turbo",
                AvailableModel.Gpt4 => "gpt-4",
                AvailableModel.Gpt41 => "gpt-4.1",
                _ => "gpt-3.5-turbo"
            };
        }
    }
}
