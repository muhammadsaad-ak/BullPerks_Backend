using Newtonsoft.Json.Linq;

public interface IBNBChainService
{
    Task<decimal> GetTotalSupplyAsync(string tokenAddress);
    Task<decimal> GetBalanceAsync(string tokenAddress, string walletAddresses);
    Task<decimal> CalculateCirculatingSupplyAsync();
}

public class BNBChainService : IBNBChainService
{
    private readonly string _blpTokenAddress;
    private readonly List<string> _nonCirculatingAddresses;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public BNBChainService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _blpTokenAddress = configuration["BNBChain:BLPTokenAddress"];
        _nonCirculatingAddresses = configuration.GetSection("BNBChain:NonCirculatingAddresses").Get<List<string>>();
        _apiKey = configuration["BNBChain:APIKey"];
    }

    public async Task<decimal> GetTotalSupplyAsync(string tokenAddress)
    {
        var requestUri = $"https://api.bscscan.com/api?module=stats&action=tokensupply&contractaddress={tokenAddress}&apikey={_apiKey}";
        var response = await _httpClient.GetStringAsync(requestUri);
        var responseObject = JObject.Parse(response);
        var result = responseObject["result"].ToString();
        return decimal.Parse(result);
    }

    public async Task<decimal> GetBalanceAsync(string tokenAddress, string walletAddresses)
    {
        var balanceRequestUri = $"https://api.bscscan.com/api?module=account&action=tokenbalance&contractaddress={tokenAddress}&address={walletAddresses}&tag=latest&apikey={_apiKey}";
        var balanceResponse = await _httpClient.GetStringAsync(balanceRequestUri);
        var responseObject = JObject.Parse(balanceResponse);

        if (responseObject["status"].ToString() == "0")
        {
            var errorMessage = responseObject["message"].ToString();
            throw new Exception($"Error retrieving balance: {errorMessage}");
        }

        var result = responseObject["result"].ToString();
        return decimal.Parse(result);
    }

    public async Task<decimal> CalculateCirculatingSupplyAsync()
    {
        decimal totalSupply = await GetTotalSupplyAsync(_blpTokenAddress);
        decimal nonCirculatingTotal = 0;

        foreach (var address in _nonCirculatingAddresses)
        {
            decimal balance = await GetBalanceAsync(_blpTokenAddress, address);
            nonCirculatingTotal += balance;
        }

        return totalSupply - nonCirculatingTotal;
    }
}
