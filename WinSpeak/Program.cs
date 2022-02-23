using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using System.Globalization;

namespace WinSpeak
{
    public class VSynth
    {
        SpeechSynthesizer synth;
        List<InstalledVoice> voices;
        CultureInfo lang;
        int rate = 0;
        int volume = 100;
        //
        public VSynth()
        {
            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            voices = new List<InstalledVoice>();
            foreach (var voice in synth.GetInstalledVoices())
            {
                voices.Add(voice);
            }
            setVoice(0);
        }
        public void setLang(String x)
        {
//            for (int i = 0; i < knownlang.Length; i++)
//            {
//                if (x == knownlang[i])
//                {
//                    lang = knownlang[i];
//                    langnum = knownlangnum[i];
//                    return;
//                }
//            }
 //           throw new Exception("Unknown language: " + x);
        }
        public void listVoice()
        {
            for (var i = 0; i < voices.Count; i++)
            {
                Console.WriteLine(i.ToString() + ": " +
                    voices[i].VoiceInfo.Name+
                    " ("+voices[i].VoiceInfo.Culture.ToString()+")");
            }
        }
        public void setVoice(int i)
        {
            if (i < 0 || voices.Count <= i)
            {
                throw new Exception("Illegal voice number");
            }
            synth.SelectVoice(voices[i].VoiceInfo.Name);
            lang = voices[i].VoiceInfo.Culture;
        }
        public void setOutputFile(String file)
        {
            synth.SetOutputToWaveFile(file);
        }
        public void speak()
        {
            for (; ; )
            {
                String line = Console.ReadLine();
                if (line == ".") break;
                var speechcontent = "<?xml version=\"1.0\"?>";
                speechcontent += "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\""+
                    lang.ToString()+"\">";
                string volumestr = "";
                if (volume != 100)
                {
                    volumestr = volume.ToString();
                }
                speechcontent += "<s>";
                string prosody = "";
                if (rate != 0)
                {
                    prosody += "rate=\"" + (100 + rate).ToString() + "%\" ";
                }
                if (volumestr != "")
                {
                    prosody += "volume=\""+volumestr+"\" ";
                }
                if (prosody != "")
                {
                    speechcontent += "<prosody "+prosody+">";
                    speechcontent += line;
                    speechcontent += "</prosody>";
                }
                else
                {
                    speechcontent += line;

                }
                speechcontent += "</s>";
                speechcontent += "</speak>";

                Console.WriteLine(speechcontent);
                synth.SpeakSsml(speechcontent);
            }
        }
        public void setRate(int r)
        {
            rate = r;
        }
        public void setVolume(int v)
        {
            volume = v;
            synth.Volume = v;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            bool listupmode = false;
            int voiceNo = 0;
            int rate = 0;
            int volume = 100;
            String wavfile = "";
            String lang = "en-US";

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-listvoice")
                {
                    listupmode = true;
                }
                else if (args[i] == "-voice")
                {
                    voiceNo = int.Parse(args[++i]);
                }
                else if (args[i] == "-volume")
                {
                    volume = int.Parse(args[++i]);
                }
                else if (args[i] == "-rate")
                {
                    rate = int.Parse(args[++i]);
                }
                else if (args[i] == "-wav")
                {
                    wavfile = args[++i];
                }
                else if (args[i] == "-lang")
                {
                    lang = args[++i];
                }
            }
            VSynth vs = new VSynth();
            vs.setLang(lang);
            if (listupmode)
                vs.listVoice();
            else
            {
                vs.setVoice(voiceNo);
                vs.setRate(rate);
                vs.setVolume(volume);
                if (wavfile.Length > 0)
                {
                    vs.setOutputFile(wavfile);
                }
                vs.speak();
            }
        }
    }
}
