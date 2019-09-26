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
			int lTempo = 60000000 / 160; /* [microsec/quarter note] */

			MIDIOUT mIDIOUT = new MIDIOUT("Microsoft GS Wavetable Synth");
			MIDIData mIDIData = new MIDIData("D:\\旧ドキュメント\\TSQ\\音楽(tsq)\\君とみた海(完全版)\\君とみた海(伴奏).mid");

			lTimeMode = (MIDIClock.TimeMode)mIDIData.TimeMode;
			lTimeResolution = mIDIData.TimeResolution;

			//すべてのイベントを列挙してから再生
			List<List<MIDIEvent>> mIDIEvents = new List<List<MIDIEvent>>();
			foreach (var track in mIDIData)
			{
				List<MIDIEvent> events = new List<MIDIEvent>();
				foreach (var @event in track)
				{
					events.Add(@event);
				}
				mIDIEvents.Add(events);
			}

			MIDIClock mIDIClock = new MIDIClock(lTimeMode, lTimeResolution, lTempo);

			Console.WriteLine("Now playing...");
			mIDIClock.Start();

			int lCurTime = 0;
			int lEndTime = mIDIData.EndTime;
			int lOldTime = 0;
			while (lCurTime <= lEndTime && !Console.KeyAvailable)
			{
				lCurTime = mIDIClock.TickCount;
				foreach (var track in mIDIEvents)
				{
					foreach(var @event in track)
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
