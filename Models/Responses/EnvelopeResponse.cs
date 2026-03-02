namespace NoWasteOfMoney.Models.Responses
{
    public class EnvelopeResponse<T>
    {
        public T? Data { get; set; }
        public EnvelopeMeta? Meta { get; set; }
    }
}
