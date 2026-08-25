namespace Flagship.Core.ViewModels {
    public class ResponseViewModel {
        public object? Data { get; set; }
        public object? AdditionalData { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool OperationStatus { get; set; }
    }
}
