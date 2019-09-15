using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIDIClockCSWrapper;
using MIDIDataCSWrapper;
using MIDIIOCSWrapper;

namespace MIDIPlayNet
{
	class Program
	{
		static void Main(string[] args)
		{
			MIDIClock.TimeMode lTimeMode = 0;
			int lTimeResolution = 0;
			int lTempo = 60000000 / 120; /* [microsec/quarter note] */
			int lEndTime = 0;
			int lOldTime = 0;
			int lCurTime = 0;

			MIDIOUT mIDIOUT = new MIDIOUT("Microsoft GS Wavetable Synth");
			MIDIData mIDIData = new MIDIData("D:\\旧ドキュメント\\TSQ\\音楽(tsq)\\Let`s search for Tomorrow.mid");

			lTimeMode = (MIDIClock.TimeMode)mIDIData.TimeMode;
			lTimeResolution = mIDIData.TimeResolution;
			lEndTime = mIDIData.EndTime;

			MIDIClock mIDIClock = new MIDIClock(lTimeMode, lTimeResolution, lTempo);

			Console.WriteLine("Now playing...");
			mIDIClock.Start();

			while (lCurTime <= lEndTime && !Console.KeyAvailable)
			{
				lCurTime = mIDIClock.TickCount;
				Console.WriteLine(lCurTime);
				foreach (MIDITrack track in mIDIData)
				{
					foreach (MIDIEvent @event in track)
					{
						int lTime = @event.Time;
						if (lOldTime <= lTime && lTime < lCurTime)
						{
							if (@event.IsTempo)
							{
								lTempo = @event.Tempo;
								mIDIClock.Tempo = lTempo;
							}
							if (@event.IsMIDIEvent ||
								@event.IsSysExEvent)
							{
								byte[] byMessage = @event.Data;

								mIDIOUT.PutMIDIMessage(byMessage);
							}
						}
					}
				}
				lOldTime = lCurTime;
			}

			mIDIClock.Stop();
			mIDIOUT.Dispose();
			Console.WriteLine("Now end.");

			Console.Read();
		}
	}
}
