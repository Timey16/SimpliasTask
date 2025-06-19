namespace backend.ViewModels
{
    public class AuthResponseViewModel
    {
        public string Token { get; set; } = string.Empty;
        public bool Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
