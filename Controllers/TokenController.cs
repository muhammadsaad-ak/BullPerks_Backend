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

    [HttpPut("calculate-supply")]
    public async Task<ActionResult<TokenInfo>> CalculateAndStoreSupply()
    {
        var configuration = HttpContext.RequestServices.GetService<IConfiguration>();
        var _blpTokenAddress = configuration["BNBChain:BLPTokenAddress"];
        decimal circulatingSupply = await _bnbChainService.CalculateCirculatingSupplyAsync();

        var tokenInfo = await _dbContext.Tokens.FirstOrDefaultAsync();

        if (tokenInfo == null)
        {
            tokenInfo = new TokenInfo
            {
                Name = "BLP Token",
                TotalSupply = await _bnbChainService.GetTotalSupplyAsync(_blpTokenAddress),
                CirculatingSupply = circulatingSupply,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.Tokens.Add(tokenInfo);
        }
        else
        {
            tokenInfo.TotalSupply = await _bnbChainService.GetTotalSupplyAsync(_blpTokenAddress);
            tokenInfo.CirculatingSupply = circulatingSupply;
            tokenInfo.UpdatedAt = DateTime.UtcNow;
            _dbContext.Tokens.Update(tokenInfo);
        }

        await _dbContext.SaveChangesAsync();

        return Ok(tokenInfo);
    }

    [HttpGet("token-data")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenInfo>> GetTokenData()
    {
        var tokenInfo = await _dbContext.Tokens.FirstOrDefaultAsync();

        if (tokenInfo == null)
        {
            return NotFound();
        }

        return Ok(tokenInfo);
    }
}
