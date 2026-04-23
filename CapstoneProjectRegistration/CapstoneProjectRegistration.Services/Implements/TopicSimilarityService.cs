using System.Text.RegularExpressions;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Interfaces;
using CapstoneProjectRegistration.Services.DTOs.Topic;
using CapstoneProjectRegistration.Services.Interface;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectRegistration.Services.Implements;

public class TopicSimilarityService : ITopicSimilarityService
{
    private readonly IUnitOfWork _unitOfWork;

    public TopicSimilarityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SimilarityResultDto>> CheckDuplicateAsync(DuplicateCheckRequestDto request)
    {
        var topics = await _unitOfWork.Topics.GetAllWithIncludeAsync(q =>
            q.Include(t => t.Semester));
        if (topics.Count == 0)
        {
            return new List<SimilarityResultDto>();
        }

        var requestDoc = CombineToDocument(request.EnglishName, request.VietnameseName, request.Description);
        var corpus = topics.Select(CombineToDocument).ToList();
        var idf = ComputeIdf(corpus);
        var queryVector = BuildTfidfVector(requestDoc, idf, corpus.Count);
        var results = new List<SimilarityResultDto>();
        for (var i = 0; i < topics.Count; i++)
        {
            var topic = topics[i];
            var doc = corpus[i];
            var docVector = BuildTfidfVector(doc, idf, corpus.Count);
            var tfIdfSim = CosineSimilarity(queryVector, docVector);
            var nameA = (request.EnglishName + " " + request.VietnameseName).Trim();
            var nameB = (topic.EnglishName + " " + topic.VietnameseName).Trim();
            var fuzzy = Fuzz.TokenSetRatio(
                nameA,
                nameB) / 100.0d;
            var total = (tfIdfSim * 0.6) + (fuzzy * 0.4);
            var risk = total > 0.8 ? "High" : total > 0.5 ? "Medium" : "Low";
            results.Add(new SimilarityResultDto
            {
                TopicId = topic.Id,
                EnglishName = topic.EnglishName,
                SemesterName = topic.Semester?.Name ?? string.Empty,
                TotalScore = Math.Round(total, 4),
                TfIdfScore = Math.Round(tfIdfSim, 4),
                FuzzyScore = Math.Round(fuzzy, 4),
                RiskLevel = risk
            });
        }

        return results
            .OrderByDescending(x => x.TotalScore)
            .ThenBy(x => x.TopicId)
            .Take(5)
            .ToList();
    }

    private static string CombineToDocument(string? english, string? vietnamese, string? description)
    {
        return (english + " " + vietnamese + " " + description).ToLowerInvariant().Trim();
    }

    private static string CombineToDocument(Topic t) => CombineToDocument(t.EnglishName, t.VietnameseName, t.Description);

    private static IReadOnlyList<string> Tokenize(string text) =>
        Regex
            .Split(string.IsNullOrEmpty(text) ? string.Empty : text, @"\W+")
            .Select(s => s)
            .Where(s => s.Length > 0)
            .ToList();

    private static Dictionary<string, double> ComputeIdf(IReadOnlyList<string> corpus)
    {
        var n = corpus.Count;
        var documentFrequency = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var document in corpus)
        {
            var terms = new HashSet<string>(Tokenize(document), StringComparer.Ordinal);
            foreach (var t in terms)
            {
                documentFrequency[t] = documentFrequency.GetValueOrDefault(t) + 1;
            }
        }

        var idf = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var kv in documentFrequency)
        {
            var df = kv.Value;
            idf[kv.Key] = Math.Log((n + 1d) / (df + 1d)) + 1d;
        }

        return idf;
    }

    private static Dictionary<string, double> BuildTfidfVector(
        string document,
        IReadOnlyDictionary<string, double> idf,
        int corpusSize)
    {
        var terms = Tokenize(document);
        var termFreq = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var t in terms)
        {
            termFreq[t] = termFreq.GetValueOrDefault(t) + 1;
        }

        var vector = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var (term, tf) in termFreq)
        {
            if (idf.TryGetValue(term, out var w))
            {
                vector[term] = (1d + Math.Log(tf)) * w;
            }
            else
            {
                var imputedIdf = Math.Log((corpusSize + 1d) / 1d) + 1d;
                vector[term] = (1d + Math.Log(tf)) * imputedIdf;
            }
        }

        return vector;
    }

    private static double CosineSimilarity(
        IReadOnlyDictionary<string, double> a,
        IReadOnlyDictionary<string, double> b)
    {
        if (a.Count == 0 && b.Count == 0)
        {
            return 0;
        }

        if (a.Count == 0 || b.Count == 0)
        {
            return 0;
        }

        var keys = new HashSet<string>(a.Keys, StringComparer.Ordinal);
        keys.IntersectWith(b.Keys);
        double dot = 0, na = 0, nb = 0;
        foreach (var (k, va) in a)
        {
            na += va * va;
        }

        foreach (var (k, vb) in b)
        {
            nb += vb * vb;
        }

        foreach (var k in keys)
        {
            dot += a[k] * b[k];
        }

        if (na <= 0 || nb <= 0)
        {
            return 0;
        }

        return dot / (Math.Sqrt(na) * Math.Sqrt(nb));
    }
}
