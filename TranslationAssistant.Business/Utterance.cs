using System;

namespace TranslationAssistant.Business
{
    /// <summary>
    /// Hold an utterance of a media caption file like VTT or SRT.
    /// The content is not expected to be sentence-combined.
    /// The "portion" indicates the fraction of 1 to the full sentence indicated by the "group". 
    /// </summary>
    class Utterance
    {
        public Utterance(int order, string timecode)
        {
            Order = order;
            Timecode = timecode;
        }

        public Utterance(int order, string timecode, string content)
        {
            Order = order;
            Timecode = timecode ?? throw new ArgumentNullException(nameof(timecode));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public int Order { get; set; }
        public int Group { get; set; }
        public string Timecode { get; set; }
        public string Content { get; set; }
        public int Lines { get; set; }
        public double Portion { get; set; }
    }
}
