#include "pch.h"
/* Don't forget to add MIDIIO(d).lib to your project */
/* Don't forget to add MIDIData(d).lib to your project */
/* Don't forget to add MIDIClock(d).lib to your project */

int _tmain() {
	long lTimeMode = 0;
	long lTimeResolution = 0;
	long lTempo = 60000000 / 120; /* [microsec/quarter note] */
	long lEndTime = 0;
	long lOldTime = 0;
	long lCurTime = 0;
	MIDIOut* pMIDIOut = NULL;
	MIDIData* pMIDIData = NULL;
	MIDITrack* pMIDITrack = NULL;
	MIDIEvent* pMIDIEvent = NULL;
	MIDIClock* pMIDIClock = NULL;

	pMIDIOut = MIDIOut_Open(_T("Microsoft GS Wavetable Synth"));
	if (pMIDIOut == NULL) {
		_tprintf(_T("MIDIOut Open failed.\n"));
		return 0;
	}

	pMIDIData = MIDIData_LoadFromSMF(_T("D:\\旧ドキュメント\\TSQ\\音楽(tsq)\\Let`s search for Tomorrow.mid"));
	if (pMIDIData == NULL) {
		_tprintf(_T("MIDIData Load failed.\n"));
		return 0;
	}

	lTimeMode = MIDIData_GetTimeMode(pMIDIData);
	lTimeResolution = MIDIData_GetTimeResolution(pMIDIData);
	lEndTime = MIDIData_GetEndTime(pMIDIData);

	pMIDIClock = MIDIClock_Create(lTimeMode, lTimeResolution, lTempo);
	if (pMIDIClock == NULL) {
		_tprintf(_T("MIDIClock_Create failed.\n"));
		return 0;
	}

	_tprintf(_T("Now playing...\n"));
	MIDIClock_Start(pMIDIClock);

	while (lCurTime <= lEndTime && !_kbhit()) {
		lCurTime = MIDIClock_GetTickCount(pMIDIClock);
		forEachTrack(pMIDIData, pMIDITrack) {
			forEachEvent(pMIDITrack, pMIDIEvent) {
				long lTime = MIDIEvent_GetTime(pMIDIEvent);
				if (lOldTime <= lTime && lTime < lCurTime) {
					if (MIDIEvent_IsTempo(pMIDIEvent)) {
						long lTempo = MIDIEvent_GetTempo(pMIDIEvent);
						MIDIClock_SetTempo(pMIDIClock, lTempo);
					}
					if (MIDIEvent_IsMIDIEvent(pMIDIEvent) ||
						MIDIEvent_IsSysExEvent(pMIDIEvent)) {
						unsigned char byMessage[256];
						long lLen = MIDIEvent_GetLen(pMIDIEvent);
						MIDIEvent_GetData(pMIDIEvent, byMessage, 256);
						MIDIOut_PutMIDIMessage(pMIDIOut, byMessage, lLen);
					}
				}
			}
		}
		lOldTime = lCurTime;
		//Sleep(5);
	}
	MIDIClock_Stop(pMIDIClock);
	_tprintf(_T("Now end.\n"));

	MIDIClock_Delete(pMIDIClock);
	MIDIData_Delete(pMIDIData);
	MIDIOut_Close(pMIDIOut);
	return 1;
}