using AutoMapper;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Interfaces;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Request.Topic;
using CapstoneProjectRegistration.Services.Respond;
using CapstoneProjectRegistration.Services.Respond.Topics;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;

namespace CapstoneProjectRegistration.Services.Service.Topicss;

public class TopicService : ITopicService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TopicService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse> CreateTopicAsync(TopicCreateRequest request)
    {
        var duplicate = await CheckDuplicateInternalAsync(request.EnglishName, request.VietnameseName, request.Description);
        if (duplicate.IsDuplicateSuspected && duplicate.RiskLevel == "High")
        {
            return new ApiResponse().SetBadRequest(duplicate, "Topic is highly similar to an existing one.");
        }

        var creator = await _unitOfWork.Lecturers.GetByIdAsync(request.CreatorId);
        if (creator == null)
        {
            return new ApiResponse().SetBadRequest(message: "Creator lecturer not found.");
        }

        var registrationPeriod = await _unitOfWork.RegistrationPeriods.GetByIdAsync(request.RegistrationPeriodId);
        if (registrationPeriod == null || registrationPeriod.Status != "Active")
        {
            return new ApiResponse().SetBadRequest(message: "Registration period is invalid or inactive.");
        }

        var topicCodeExists = await _unitOfWork.Topics.GetAsync(x => x.TopicCode == request.TopicCode);
        if (topicCodeExists != null)
        {
            return new ApiResponse().SetBadRequest(message: "TopicCode already exists.");
        }

        var topic = _mapper.Map<Topic>(request);
        await _unitOfWork.Topics.AddAsync(topic);
        await _unitOfWork.SaveChangesAsync();
        return new ApiResponse().SetOk("Topic created successfully.");
    }

    public async Task<ApiResponse> GetAllTopicsAsync()
    {
        var topics = await _unitOfWork.Topics.GetAllAsync();
        var mapped = _mapper.Map<List<TopicRespond>>(topics);
        return new ApiResponse().SetOk(mapped);
    }

    public async Task<ApiResponse> GetTopicByIdAsync(int id)
    {
        var topic = await _unitOfWork.Topics.GetByIdAsync(id);
        if (topic == null)
        {
            return new ApiResponse().SetNotFound(message: "Topic not found.");
        }

        return new ApiResponse().SetOk(_mapper.Map<TopicRespond>(topic));
    }

    public async Task<ApiResponse> UpdateTopicAsync(int id, TopicUpdateRequest request)
    {
        var topic = await _unitOfWork.Topics.GetByIdAsync(id);
        if (topic == null)
        {
            return new ApiResponse().SetNotFound(message: "Topic not found.");
        }

        _mapper.Map(request, topic);
        topic.Status = "Pending";
        topic.ReviewStatus = "Pending";
        topic.PublicStatus = "Private";
        _unitOfWork.Topics.Update(topic);
        await _unitOfWork.SaveChangesAsync();
        return new ApiResponse().SetOk("Topic updated successfully.");
    }

    public async Task<ApiResponse> DeleteTopicAsync(int id)
    {
        var topic = await _unitOfWork.Topics.GetByIdAsync(id);
        if (topic == null)
        {
            return new ApiResponse().SetNotFound(message: "Topic not found.");
        }

        _unitOfWork.Topics.Delete(topic);
        await _unitOfWork.SaveChangesAsync();
        return new ApiResponse().SetOk("Topic deleted successfully.");
    }

    public async Task<ApiResponse> AssignReviewersAsync(int topicId, AssignReviewersRequest request)
    {
        if (request.Reviewer1Id == request.Reviewer2Id)
        {
            return new ApiResponse().SetBadRequest(message: "Two reviewers must be different.");
        }

        var topic = await _unitOfWork.Topics.GetByIdAsync(topicId);
        if (topic == null)
        {
            return new ApiResponse().SetNotFound(message: "Topic not found.");
        }

        if (topic.CreatorId == request.Reviewer1Id || topic.CreatorId == request.Reviewer2Id)
        {
            return new ApiResponse().SetBadRequest(message: "Creator cannot review their own topic.");
        }

        var existing = await _unitOfWork.TopicReviews.GetAllAsync(tr => tr.TopicId == topicId);
        if (existing.Count > 0)
        {
            return new ApiResponse().SetBadRequest(message: "Reviewers were already assigned for this topic.");
        }

        await _unitOfWork.TopicReviews.AddAsync(new TopicReview
        {
            TopicId = topicId,
            ReviewerId = request.Reviewer1Id,
            Decision = "Pending",
            Comment = string.Empty,
            ReviewDate = DateTime.UtcNow,
            IsFinalized = false
        });
        await _unitOfWork.TopicReviews.AddAsync(new TopicReview
        {
            TopicId = topicId,
            ReviewerId = request.Reviewer2Id,
            Decision = "Pending",
            Comment = string.Empty,
            ReviewDate = DateTime.UtcNow,
            IsFinalized = false
        });

        topic.Status = "InReview";
        _unitOfWork.Topics.Update(topic);
        await _unitOfWork.SaveChangesAsync();

        return new ApiResponse().SetOk("Reviewers assigned successfully.");
    }

    public async Task<ApiResponse> SubmitReviewAsync(int topicId, SubmitTopicReviewRequest request)
    {
        var normalizedDecision = request.Decision.Trim().ToLowerInvariant();
        if (normalizedDecision != "approved" && normalizedDecision != "rejected")
        {
            return new ApiResponse().SetBadRequest(message: "Decision must be either Approved or Rejected.");
        }

        var topic = await _unitOfWork.Topics.GetByIdAsync(topicId);
        if (topic == null)
        {
            return new ApiResponse().SetNotFound(message: "Topic not found.");
        }

        var review = await _unitOfWork.TopicReviews.GetAsync(tr => tr.TopicId == topicId && tr.ReviewerId == request.ReviewerId);
        if (review == null)
        {
            return new ApiResponse().SetNotFound(message: "Reviewer is not assigned to this topic.");
        }

        review.Decision = normalizedDecision == "approved" ? "Approved" : "Rejected";
        review.Comment = request.Comment;
        review.ReviewDate = DateTime.UtcNow;
        review.IsFinalized = true;
        _unitOfWork.TopicReviews.Update(review);

        await UpdateTopicReviewStatusAsync(topic);
        await _unitOfWork.SaveChangesAsync();
        return new ApiResponse().SetOk("Review submitted successfully.");
    }

    public async Task<ApiResponse> GetReviewSummaryAsync(int topicId)
    {
        var topic = await _unitOfWork.Topics.GetByIdAsync(topicId);
        if (topic == null)
        {
            return new ApiResponse().SetNotFound(message: "Topic not found.");
        }

        var reviews = await _unitOfWork.TopicReviews.GetAllAsync(tr => tr.TopicId == topicId);
        var respond = new TopicReviewSummaryRespond
        {
            TopicId = topicId,
            ApprovedCount = reviews.Count(x => x.Decision == "Approved"),
            RejectedCount = reviews.Count(x => x.Decision == "Rejected"),
            ReviewStatus = topic.ReviewStatus,
            PublicStatus = topic.PublicStatus
        };

        return new ApiResponse().SetOk(respond);
    }

    public async Task<ApiResponse> PublishTopicAsync(int topicId)
    {
        var topic = await _unitOfWork.Topics.GetByIdAsync(topicId);
        if (topic == null)
        {
            return new ApiResponse().SetNotFound(message: "Topic not found.");
        }

        if (topic.ReviewStatus != "Approved")
        {
            return new ApiResponse().SetBadRequest(message: "Topic can only be published when review status is Approved.");
        }

        topic.PublicStatus = "Public";
        topic.Status = "Published";
        _unitOfWork.Topics.Update(topic);
        await _unitOfWork.SaveChangesAsync();

        return new ApiResponse().SetOk(new { Message = "Topic published successfully.", Notification = "Email stub: sent to topic creator." });
    }

    public async Task<ApiResponse> CheckDuplicateAsync(TopicDuplicateCheckRequest request)
    {
        var result = await CheckDuplicateInternalAsync(request.EnglishName, request.VietnameseName, request.Description);
        return new ApiResponse().SetOk(result);
    }

    public async Task<ApiResponse> ExtractTopicFromFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return new ApiResponse().SetBadRequest(message: "File is required.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".docx" && extension != ".pdf")
        {
            return new ApiResponse().SetBadRequest(message: "Only .docx and .pdf are supported.");
        }

        var fullText = extension == ".docx"
            ? await ExtractDocxTextAsync(file)
            : "PDF parser placeholder: integrate PdfPig/iText in next step.";

        var response = new TopicExtractRespond
        {
            EnglishName = ExtractAfter(fullText, "English:", 300),
            VietnameseName = ExtractAfter(fullText, "Vietnamese:", 300),
            Description = ExtractAfter(fullText, "Context:", 2000),
            TopicCode = string.Empty,
            Confidence = "Medium"
        };

        return new ApiResponse().SetOk(response);
    }

    private async Task<DuplicateCheckRespond> CheckDuplicateInternalAsync(string englishName, string vietnameseName, string description)
    {
        var topics = await _unitOfWork.Topics.GetAllAsync();
        var maxScore = 0d;

        foreach (var topic in topics)
        {
            var score = (Similarity(englishName, topic.EnglishName) * 0.5)
                      + (Similarity(vietnameseName, topic.VietnameseName) * 0.3)
                      + (Similarity(description, topic.Description) * 0.2);
            if (score > maxScore)
            {
                maxScore = score;
            }
        }

        var level = maxScore >= 0.85 ? "High" : maxScore >= 0.65 ? "Medium" : "Low";
        return new DuplicateCheckRespond
        {
            SimilarityScore = Math.Round(maxScore, 2),
            RiskLevel = level,
            IsDuplicateSuspected = level != "Low",
            Message = level == "High"
                ? "High similarity detected with existing topics."
                : level == "Medium" ? "Possible duplicate topic. Manual review recommended." : "No significant duplicate found."
        };
    }

    private async Task UpdateTopicReviewStatusAsync(Topic topic)
    {
        var reviews = await _unitOfWork.TopicReviews.GetAllAsync(tr => tr.TopicId == topic.Id);
        if (reviews.Any(r => r.Decision == "Rejected"))
        {
            topic.ReviewStatus = "Rejected";
            topic.Status = "Rejected";
            topic.PublicStatus = "Private";
            _unitOfWork.Topics.Update(topic);
            return;
        }

        if (reviews.Count == 2 && reviews.All(r => r.Decision == "Approved"))
        {
            topic.ReviewStatus = "Approved";
            topic.Status = "Approved";
            _unitOfWork.Topics.Update(topic);
            return;
        }

        topic.ReviewStatus = "Pending";
        _unitOfWork.Topics.Update(topic);
    }

    private static double Similarity(string? left, string? right)
    {
        left = Normalize(left);
        right = Normalize(right);
        if (left.Length == 0 || right.Length == 0)
        {
            return 0;
        }

        var distance = Levenshtein(left, right);
        return 1d - (double)distance / Math.Max(left.Length, right.Length);
    }

    private static string Normalize(string? text)
    {
        return (text ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static int Levenshtein(string s, string t)
    {
        var n = s.Length;
        var m = t.Length;
        var d = new int[n + 1, m + 1];

        for (var i = 0; i <= n; i++) d[i, 0] = i;
        for (var j = 0; j <= m; j++) d[0, j] = j;

        for (var i = 1; i <= n; i++)
        {
            for (var j = 1; j <= m; j++)
            {
                var cost = s[i - 1] == t[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }

    private static async Task<string> ExtractDocxTextAsync(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        using var mem = new MemoryStream();
        await stream.CopyToAsync(mem);
        mem.Position = 0;
        using var doc = WordprocessingDocument.Open(mem, false);
        return doc.MainDocumentPart?.Document.Body?.InnerText ?? string.Empty;
    }

    private static string ExtractAfter(string source, string marker, int maxLen)
    {
        var idx = source.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            return string.Empty;
        }

        var value = source[(idx + marker.Length)..].Trim();
        return value.Length > maxLen ? value[..maxLen] : value;
    }
}

