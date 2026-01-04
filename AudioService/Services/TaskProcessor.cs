using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace AudioService.Services;

public class TaskProcessor : ITaskProcessor
{
    private readonly ILogger<TaskProcessor> _logger;
    private readonly HttpClient _httpClient;

    public TaskProcessor(ILogger<TaskProcessor> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken, string fileName)
    {
        _logger.LogInformation("----> Processing audio tasks started");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Add your audio processing logic here
            await PerformAudioProcessing(cancellationToken, fileName);

            stopwatch.Stop();
            _logger.LogInformation("----> Processing audio tasks completed successfully in {milliseconds}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogWarning("Audio processing was cancelled after {milliseconds}ms", stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error during audio processing after {milliseconds}ms", stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    private async Task PerformAudioProcessing(CancellationToken cancellationToken, string fileName)
    {
        _logger.LogInformation("  >> Audio processing routine starting at {time:O}", DateTime.UtcNow);

        try
        {
            const string uploadsPath = @"C:\Projects\test\WebApplication1\WebApplication1\Uploads";
            const string transcribeUrl = "http://localhost:8000/transcribe";

            if (!Directory.Exists(uploadsPath))
            {
                _logger.LogWarning("  >> Uploads directory not found: {path}", uploadsPath);
                return;
            }
            var filePath =Path.Combine(uploadsPath, fileName);
            await SendFileForTranscription(filePath, transcribeUrl, cancellationToken);

            //var audioFiles = Directory.GetFiles(uploadsPath, "*.*")
            //    .Where(f => IsAudioFile(f))
            //    .ToArray();

            //_logger.LogInformation("  >> Found {count} audio files to process", audioFiles.Length);

            //foreach (var filePath in audioFiles)
            //{
            //    if (cancellationToken.IsCancellationRequested)
            //    {
            //        _logger.LogWarning("  >> Audio processing cancelled");
            //        break;
            //    }

            //    try
            //    {
            //        _logger.LogInformation("  >> Processing file: {file}", Path.GetFileName(filePath));
            //        //await SendFileForTranscription(filePath, transcribeUrl, cancellationToken);
            //        _logger.LogInformation("  >> Successfully sent file for transcription: {file}", Path.GetFileName(filePath));
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "  >> Error processing file: {file}", Path.GetFileName(filePath));
            //    }
            //}

            _logger.LogInformation("  >> Audio processing routine completed at {time:O}", DateTime.UtcNow);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("  >> Audio processing routine was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "  >> Exception in audio processing routine");
            throw;
        }
    }

    private async Task SendFileForTranscription(string filePath, string transcribeUrl, CancellationToken cancellationToken)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var content = new MultipartFormDataContent())
            {
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/x-m4a");

                content.Add(fileContent, "file", Path.GetFileName(filePath));

                var response = await _httpClient.PostAsync(transcribeUrl, content, cancellationToken);
                response.EnsureSuccessStatusCode();


                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogDebug("  >> Transcription response status: {statusCode}", response.StatusCode);
                _logger.LogDebug("  >> Transcription responseBody: {responseBody}", responseBody);
            }
        }
    }

    private static bool IsAudioFile(string filePath)
    {
        var audioExtensions = new[] { ".m4a", ".mp3", ".wav", ".ogg", ".flac", ".aac" };
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return audioExtensions.Contains(extension);
    }
}
