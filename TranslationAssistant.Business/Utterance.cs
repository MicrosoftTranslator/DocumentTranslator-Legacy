using System;

namespace TranslationAssistant.Business
{
    class Utterance
    {
        public Utterance(int order, string timecode)
        {
            this.order = order;
            this.timecode = timecode;
        }

        public Utterance(int order, string timecode, string content)
        {
            this.order = order;
            this.timecode = timecode ?? throw new ArgumentNullException(nameof(timecode));
            this.content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public int order { get; set; }
        public int group { get; set; }
        public string timecode { get; set; }
        public string content { get; set; }
        public int lines { get; set; }
        public double portion { get; set; }
    }
}
