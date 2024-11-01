namespace SharedLibrary.DTOs
{
    public class ErrorDto
    {
        public List<string> Erorrs { get; private set; } = new List<string>();
        public bool IsShow { get; private set; }

        public ErrorDto(string error, bool isShow)
        {
            Erorrs.Add(error);
            IsShow = isShow;
        }

        public ErrorDto(List<string> errors, bool isShow)
        {
            Erorrs = errors;
            IsShow = isShow;
        }
    }
}
