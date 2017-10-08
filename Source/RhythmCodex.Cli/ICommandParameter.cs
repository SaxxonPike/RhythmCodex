namespace RhythmCodex.Cli
{
    /// <summary>
    /// Contains details about a parameter for a command.
    /// </summary>
    public interface ICommandParameter
    {
        /// <summary>
        /// Name which the user can use to interact with this parameter.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Describes the use of this parameter.
        /// </summary>
        string Description { get; }
    }
}