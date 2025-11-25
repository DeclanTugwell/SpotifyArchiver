namespace SpotifyArchiver.Presentation
{
    public class Operation
    {
        public string Name { get; }
        public string Description { get; }
        private readonly Func<Task> _action;

        public Operation(string name, string description, Func<Task> action)
        {
            Name = name;
            Description = description;
            _action = action;
        }

        public async Task Execute() => await _action.Invoke();
    }
}
