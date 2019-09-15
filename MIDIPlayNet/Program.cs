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
			MIDIData mIDIData = new MIDIData("D:\\旧ドキュメント\\TSQ\\音楽(tsq)\\ブラック★ロックシューター.mid");

			lTimeMode = (MIDIClock.TimeMode)mIDIData.TimeMode;
			lTimeResolution = mIDIData.TimeResolution;
			//オープンMIDIプロジェクトのサイトにあったサンプルの方法だと、正常に再生できなかったので、フォーマット０に変換し、１トラックのイベントを最初から順番に再生することにした。
			mIDIData.Format = MIDIData.Formats.Format0;

			MIDIClock mIDIClock = new MIDIClock(lTimeMode, lTimeResolution, lTempo);

			Console.WriteLine("Now playing...");
			mIDIClock.Start();

			foreach (MIDITrack track in mIDIData)
			{
				foreach (MIDIEvent @event in track)
				{
					if (Console.KeyAvailable)
					{
						break;
					}
					int lTime = @event.Time;
					//イベントの再生すべき時間になるまで待機
					while (mIDIClock.TickCount <= lTime){ }
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
				break;
			}

			mIDIClock.Stop();
			mIDIOUT.Dispose();
			Console.WriteLine("Now end.");

			Console.Read();

		}
	}
}
