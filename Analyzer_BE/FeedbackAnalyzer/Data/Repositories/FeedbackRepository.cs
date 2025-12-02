using AutoMapper;
using FeedbackAnalyzer.Contracts.IRepository;
using FeedbackAnalyzer.Contracts.IServices;
using FeedbackAnalyzer.Data.DTO;
using FeedbackAnalyzer.Data.Models;
using FeedbackAnalyzer.Data.Payloads;
using FeedbackAnalyzer.Data.Response;
using Microsoft.EntityFrameworkCore;

namespace FeedbackAnalyzer.Data.Repositories;

public class FeedbackRepository(ApplicationDbContext context,
   IAnalysisService analysisService,
 IMapper mapper) : IFeedbackRepository
{
    public async Task<FeedbackDTO> CreateFeedback(FeedbackDTO feedbackPayload, float[] vectorEmbedding)
    {
        var feedback = new Feedback
        {
            Id = Guid.NewGuid().ToString(),
            Text = feedbackPayload.Text,
            Email = feedbackPayload.Email,
            CreatedAt = DateTime.UtcNow,
            VectorEmbedding = vectorEmbedding,
            Analysis = mapper.Map<FeedbackAnalysis>(feedbackPayload.Analysis)
        };

        context.Feedback.Add(feedback);
        await context.SaveChangesAsync();

        return mapper.Map<FeedbackDTO>(feedback);
    }

    public async Task<FeedbackDTO?> GetFeedbackById(string id)
    {
        var entity = await context.Feedback
            .AsNoTracking()
            .Include(f => f.Analysis)
            .Include(f => f.FeedbackTags)
                .ThenInclude(ft => ft.Tag)
            .FirstOrDefaultAsync(f => f.Id == id);

        return entity == null ? null : mapper.Map<FeedbackDTO>(entity);
    }

    // Gets all active Hub
    public async Task<PagedResponse<FeedbackDTO>> GetFeedbacks(Filter filter)
    {
        var query = context.Feedback
            .Include(f => f.Analysis)
            .Include(f => f.FeedbackTags)
                .ThenInclude(ft => ft.Tag)
            .AsNoTracking();

        // SEMANTIC SEARCH
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchText = filter.Search.Trim();

            // Generate vector embedding from the search text
            var queryVector = await analysisService.GenerateEmbeddingAsync(searchText);

            // Load all feedback embeddings into memory
            var allEmbeddings = await context.Feedback
                .Select(f => new { f.Id, f.VectorEmbedding })
                .ToListAsync();

            // Compute similarity scores
            var scoredIds = allEmbeddings
                .Select(f => new
                {
                    f.Id,
                    Score = CosineSimilarity(queryVector, f.VectorEmbedding)
                })
                .OrderByDescending(x => x.Score)
                .Take(2)          // round off to 2 closests feedbacks
                .Select(x => x.Id)
                .ToList();

            // Apply semantic order
            query = query.Where(f => scoredIds.Contains(f.Id))
                         .OrderBy(f => scoredIds.IndexOf(f.Id));
        }
        else if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            // ---- FALLBACK: KEYWORD SEARCH ----
            var search = filter.Search.Trim().ToLower();
            query = query.Where(f =>
                f.Text.ToLower().Contains(search) ||
                f.Analysis!.Summary.ToLower().Contains(search));
        }

        // ---- FILTER BY SENTIMENT ----
        if (!string.IsNullOrWhiteSpace(filter.Sentiment))
        {
            query = query.Where(f =>
                f.Analysis!.Sentiment.ToLower() == filter.Sentiment.ToLower());
        }

        // ---- FILTER BY TAG ----
        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            var tag = filter.Tag.Trim();
            query = query.Where(f =>
                f.FeedbackTags.Any(t => t.Tag.Id == tag));
        }

        // Order and paginate
        query = query.OrderBy(e => e.CreatedDate);

        var totalRecords = await query.CountAsync();

        // ---- PAGINATION ----
        var entities = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var data = mapper.Map<List<FeedbackDTO>>(entities);

        return new PagedResponse<FeedbackDTO>
        {
            Data = data,
            CurrentPage = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    private static float CosineSimilarity(float[] a, float[] b)
    {
        float dot = 0, normA = 0, normB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        return dot / (float)(Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}
