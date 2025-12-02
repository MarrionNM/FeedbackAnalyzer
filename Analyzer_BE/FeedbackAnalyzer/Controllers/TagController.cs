using FeedbackAnalyzer.Contracts.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FeedbackAnalyzer.Controllers;

public class TagController(ITagRepository tagRepository) : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        var tags = await tagRepository.GetTags();
        return Ok(tags);
    }
}
