using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourProjectNamespace.Data;

public class TokenController : ControllerBase
{
    private readonly IBNBChainService _bnbChainService;
    private readonly ApplicationDbContext _dbContext;

    public TokenController(IBNBChainService bnbChainService, ApplicationDbContext dbContext)
    {
        _bnbChainService = bnbChainService ?? throw new ArgumentNullException(nameof(bnbChainService));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    // This endpoint requires authentication and calculates the supply
    // [Authorize]
    [HttpPost("calculate-supply")]
    public async Task<ActionResult<TokenInfo>> CalculateAndStoreSupply()
    {
        var configuration = HttpContext.RequestServices.GetService<IConfiguration>();
        var _blpTokenAddress = configuration["BNBChain:BLPTokenAddress"];
        decimal circulatingSupply = await _bnbChainService.CalculateCirculatingSupplyAsync();

        // Save token information to the database
        var tokenInfo = new TokenInfo
        {
            Name = "BLP Token",
            TotalSupply = await _bnbChainService.GetTotalSupplyAsync(_blpTokenAddress),
            CirculatingSupply = circulatingSupply
        };

        _dbContext.Tokens.Add(tokenInfo);
        await _dbContext.SaveChangesAsync();

        return Ok(tokenInfo);
    }

    [HttpGet("token-data")]
    [AllowAnonymous]
    public ActionResult<TokenInfo> GetTokenData()
    {
        // Fetch token data from the database
        var tokenInfo = _dbContext.Tokens.FirstOrDefault();

        if (tokenInfo == null)
        {
            return NotFound(); // Return 404 Not Found if no token data is found
        }

        return Ok(tokenInfo);
    }
}
