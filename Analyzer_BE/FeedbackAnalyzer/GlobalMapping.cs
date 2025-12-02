using AutoMapper;
using FeedbackAnalyzer.Data.DTO;
using FeedbackAnalyzer.Data.Models;

namespace FeedbackAnalyzer;

public class GlobalMapping : Profile
{
    public GlobalMapping()
    {
        CreateMap<Feedback, FeedbackDTO>()
        .ForMember(dest => dest.Tags,
            opt => opt.MapFrom(src => src.FeedbackTags.Select(ft => ft.Tag.Name))).ReverseMap();

        CreateMap<FeedbackAnalysis, FeedbackAnalysisDTO>().ReverseMap();
        CreateMap<Tag, TagDTO>().ReverseMap();
    }
}
