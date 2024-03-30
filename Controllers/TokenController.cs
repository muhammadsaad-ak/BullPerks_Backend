using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourProjectNamespace.Data;
using Microsoft.EntityFrameworkCore;

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
    [HttpPut("calculate-supply")]
    public async Task<ActionResult<TokenInfo>> CalculateAndStoreSupply()
    {
        var configuration = HttpContext.RequestServices.GetService<IConfiguration>();
        var _blpTokenAddress = configuration["BNBChain:BLPTokenAddress"];
        decimal circulatingSupply = await _bnbChainService.CalculateCirculatingSupplyAsync();

        // Retrieve existing token record
        var tokenInfo = await _dbContext.Tokens.FirstOrDefaultAsync();

        if (tokenInfo == null)
        {
            // Insert new token record if it does not exist
            tokenInfo = new TokenInfo
            {
                Name = "BLP Token",
                TotalSupply = await _bnbChainService.GetTotalSupplyAsync(_blpTokenAddress),
                CirculatingSupply = circulatingSupply,
                CreatedAt = DateTime.UtcNow, // Set created timestamp
                UpdatedAt = DateTime.UtcNow  // Set updated timestamp
            };
            _dbContext.Tokens.Add(tokenInfo);
        }
        else
        {
            // Update existing token record
            tokenInfo.TotalSupply = await _bnbChainService.GetTotalSupplyAsync(_blpTokenAddress);
            tokenInfo.CirculatingSupply = circulatingSupply;
            tokenInfo.UpdatedAt = DateTime.UtcNow; // Update updated timestamp
            _dbContext.Tokens.Update(tokenInfo);
        }

        // Save changes to the database
        await _dbContext.SaveChangesAsync();

        return Ok(tokenInfo);
    }

    [HttpGet("token-data")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenInfo>> GetTokenData()
    {
        // Fetch token data from the database
        var tokenInfo = await _dbContext.Tokens.FirstOrDefaultAsync();

        if (tokenInfo == null)
        {
            return NotFound(); // Return 404 Not Found if no token data is found
        }

        return Ok(tokenInfo);
    }
}
