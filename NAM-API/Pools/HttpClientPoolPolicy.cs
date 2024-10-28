namespace NAM_API.Pools
{
    using Microsoft.Extensions.ObjectPool;
    using System;
    using System.Net.Http;

    public class HttpClientPoolPolicy : PooledObjectPolicy<HttpClient>
    {
        private readonly Uri _baseAddress;

        public HttpClientPoolPolicy(string baseAddress)
        {
            _baseAddress = new Uri(baseAddress);
        }

        public override HttpClient Create()
        {
            var client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
            return client;
        }

        public override bool Return(HttpClient client)
        {
            // Reset the client state if necessary
            return true;
        }
    }

}
