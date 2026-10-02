using ArticulationAPI.Models;
using ArticulationAPI.TranscriptionService.Grpc;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using GrpcStatusCode = Grpc.Core.StatusCode;

namespace ArticulationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranscriptionController : ControllerBase
{
    private readonly TranscriptionGrpc.TranscriptionGrpcClient _client;

    public TranscriptionController(TranscriptionGrpc.TranscriptionGrpcClient client)
    {
        _client = client;
    }

    [HttpPost("transcribe")]
    public async Task<ActionResult<TranscriptionResponse>> Transcribe([FromBody] TranscriptionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Text is required");
        }

        try
        {
            var reply = await _client.TranscribeAsync(
                new TranscribeRequest
                {
                    Text = request.Text,
                    Language = request.Language
                },
                deadline: DateTime.UtcNow.AddSeconds(3));

            var result = new TranscriptionResponse
            {
                OriginalText = reply.OriginalText,
                Language = reply.Language,
                Phonemes = reply.Phonemes.Select(p => new Phoneme
                {
                    Symbol = p.Symbol,
                    Ipa = p.Ipa,
                    Position = p.Position,
                    Stress = p.Stress
                }).ToList()
            };

            return Ok(result);
        }
        catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.InvalidArgument)
        {
            return BadRequest(ex.Status.Detail);
        }
        catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.DeadlineExceeded)
        {
            return StatusCode(StatusCodes.Status504GatewayTimeout, "Transcription service timed out");
        }
        catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.Unavailable)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Transcription service is unavailable");
        }
        catch (RpcException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Status.Detail);
        }
    }

    [HttpGet("languages")]
    public async Task<ActionResult<IEnumerable<string>>> GetSupportedLanguages()
    {
        try
        {
            var reply = await _client.GetSupportedLanguagesAsync(
                new GetSupportedLanguagesRequest(),
                deadline: DateTime.UtcNow.AddSeconds(3));

            return Ok(reply.Languages);
        }
        catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.Unavailable)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Transcription service is unavailable");
        }
    }
}