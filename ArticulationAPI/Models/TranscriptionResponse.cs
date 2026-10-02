namespace ArticulationAPI.Models;

public class TranscriptionResponse
{
    public string OriginalText { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public List<Phoneme> Phonemes { get; set; } = new();
}

public class Phoneme
{
    public string Symbol { get; set; } = string.Empty;
    public string Ipa { get; set; } = string.Empty;
    public int Position { get; set; }
    public float Stress { get; set; }
}