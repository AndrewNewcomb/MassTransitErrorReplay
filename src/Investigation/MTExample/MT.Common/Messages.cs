namespace Common
{
    public interface NewDataAvailable
    {
        public string Text { get; set; }
    }

    public interface InitialProcessingCompleted
    {
        public string Text { get; set; }
    }

    public interface FinalProcessingCompleted
    {
        public string Text { get; set; }
    }

    public interface SomethingNoteworthyHappened
    {
        public string Text { get; set; }
    }
}
