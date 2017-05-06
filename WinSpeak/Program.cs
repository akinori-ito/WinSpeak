using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeechLib;

namespace WinSpeak
{
    public class VSynth
    {
        SpVoice synth;
        SpFileStream outputstream;
        String lang = "en-US";
        int langnum = 409;
        String[] knownlang = { "en-US", "ja-JP" };
        int[] knownlangnum = { 409, 411 }; 
        int rate = 0;
        int volume = 100;
        //
        public VSynth()
        {
            synth = new SpVoice();
            outputstream = null;
        }
        public void setLang(String x)
        {
            for (int i = 0; i < knownlang.Length; i++)
            {
                if (x == knownlang[i])
                {
                    lang = knownlang[i];
                    langnum = knownlangnum[i];
                    return;
                }
            }
            throw new Exception("Unknown language: " + x);
        }
        public void listVoice()
        {
            ISpeechObjectTokens VoiceInfo = synth.GetVoices("Language="+langnum, "");
            for (int i = 0; i < VoiceInfo.Count; i++)
            {
                ISpeechObjectToken tok = VoiceInfo.Item(i);
                Console.WriteLine(i.ToString() + ": " + tok.GetAttribute("Name"));
            }
        }
        public void setVoice(int i)
        {
            ISpeechObjectTokens VoiceInfo = synth.GetVoices("Language=" + langnum, "");
            if (i < 0 || VoiceInfo.Count <= i)
            {
                throw new Exception("Illegal voice number");
            }
            synth.Voice = VoiceInfo.Item(i);
        }
        public void setOutputFile(String file)
        {
            if (outputstream != null)
                outputstream.Close();
            outputstream = new SpFileStream();
            outputstream.Open(file, SpeechStreamFileMode.SSFMCreateForWrite, false);
            synth.AudioOutputStream = outputstream;
        }
        public void speak()
        {
            for (;;)
            {
                String line = Console.ReadLine();
                if (line == ".") break;
                synth.Speak("<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='"+lang+"'> "+line+"</speak>");
            }
            if (outputstream != null)
                outputstream.Close();
        }
        public void setRate(int r)
        {
            rate = r;
            synth.Rate = r;
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
