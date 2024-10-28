using Newtonsoft.Json;


namespace NAM_API.Models
{
    public struct Holding
    {
        [JsonProperty("stock_id")] 
        public string StockId { get; set; }
        public decimal Value { get; set; }
    }
}
