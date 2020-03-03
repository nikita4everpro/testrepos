#include "stdafx.h"
#include <stdio.h>
#include <iostream>
using namespace std;
#include <string>
#include <Windows.h>

BOOL PrintStrings(HANDLE hOut, ...);
BOOL PrintMsg(HANDLE hOut, LPCTSTR pMsg);
VOID ReportError(LPCTSTR UserMessage, DWORD ExitCode, BOOL PrintErrorMsg)
{
	DWORD eMsgLen, LastErr = GetLastError();
	LPTSTR lpvSysMsg;
	TCHAR Buf[MAX_PATH];

	HANDLE hStdErr = GetStdHandle(STD_ERROR_HANDLE);
	PrintMsg(hStdErr, UserMessage);
	if (PrintErrorMsg) {
		eMsgLen = FormatMessage
		(FORMAT_MESSAGE_ALLOCATE_BUFFER |
			FORMAT_MESSAGE_FROM_SYSTEM, NULL, LastErr,
			MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
			(LPTSTR)&lpvSysMsg, 0, NULL);
		CharToOem(lpvSysMsg, (char *)&Buf[0]);
		PrintStrings(hStdErr, _T("\n"), Buf,
			_T("\n"), NULL);

		HeapFree(GetProcessHeap(), 0, lpvSysMsg);
	}
	if (ExitCode>0)
		ExitProcess(ExitCode);
	else
		return;
}

BOOL PrintStrings(HANDLE hOut, ...)
{
	DWORD MsgLen, Count;
	LPCTSTR pMsg;
	va_list pMsgList;
	va_start(pMsgList, hOut); 
	while ((pMsg = va_arg(pMsgList, LPCTSTR)) != NULL) {
		MsgLen = _tcslen(pMsg);
		if (!WriteConsole(hOut, pMsg, MsgLen, &Count, NULL)
			&& !WriteFile(hOut, pMsg, MsgLen * sizeof(TCHAR),
				&Count, NULL)) return FALSE;
	}
	va_end(pMsgList);
	return TRUE;
}

BOOL PrintMsg(HANDLE hOut, LPCTSTR pMsg)
{
	TCHAR Buf[MAX_PATH + 1];
	CharToOem(pMsg, Buf);
	return PrintStrings(hOut, Buf, NULL);
}



int main(int argc, char* argv[])
{
	setlocale(LC_ALL, "rus");
	HANDLE hRead, hRead2;
	FILE* f;
	FILE* f2;

	int n;
	cin >> n;

	switch (n)
	{
	case 1:
		hRead = CreateFile("file", GENERIC_READ, 0, NULL, OPEN_EXISTING, NULL, NULL);
		fopen("file", "r");

		ReportError("Результат ReportError:", NULL, true);
		perror("Результат perror");

		CloseHandle(hRead);
		DeleteFileA("file");
		break;
	case 2:
		hRead = CreateFile("file", GENERIC_READ, 0, NULL, CREATE_ALWAYS, NULL, NULL);
		hRead = CreateFile("file", GENERIC_WRITE, 0, NULL, OPEN_EXISTING, NULL, NULL);
		fopen("file", "w");

		ReportError("Результат ReportError:", NULL, true);
		perror("Результат perror");

		CloseHandle(hRead);
		DeleteFileA("file");
		break;
	case 3:
		hRead = CreateFile("file", GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, NULL, NULL);
		hRead = CreateFile("file", GENERIC_WRITE, 0, NULL, OPEN_EXISTING, NULL, NULL);
		fopen("file", "w");

		ReportError("Результат ReportError:", NULL, true);
		perror("Результат perror");

		CloseHandle(hRead);
		DeleteFileA("file");
		break;
	case 4:
		hRead = CreateFile("file", GENERIC_READ, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_READONLY, NULL);
		WriteFile(hRead, "string", 1, NULL, NULL);

		ReportError("Результат ReportError:", NULL, true);
		CloseHandle(hRead);

		f = fopen("file", "r");
		fwrite("string", 1, 1, f);

		perror("Результат perror");

		fclose(f);
		DeleteFileA("file");
		break;
	case 5:
		hRead = CreateFile("file", GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, NULL, NULL);
		hRead2 = CreateFile("file", GENERIC_WRITE, 0, NULL, OPEN_EXISTING, NULL, NULL);

		WriteFile(hRead, "string", 1, NULL, NULL);

		ReportError("Результат ReportError:", NULL, true);
		CloseHandle(hRead);
		CloseHandle(hRead2);
		/*
		f = fopen("file", "wb");

		fwrite("string", 1, 1, f);

		perror("Результат perror");

		fclose(f);
		*/
		
		DeleteFileA("file");
		break;
	case 6:
		hRead = CreateFile("file", GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, NULL, NULL);
		hRead = CreateFile("file", GENERIC_READ, 0, NULL, CREATE_ALWAYS, NULL, NULL);

		ReadFile(hRead, NULL, 1, NULL, NULL);

		ReportError("Результат ReportError:", NULL, true);
		CloseHandle(hRead);

		f = fopen("file", "rb");

		fread("типа string", 1, 1, f);

		perror("Результат perror");

		fclose(f);

		DeleteFileA("file");
		break;
	case 7:
		
		hRead2 = CreateFile("file", GENERIC_ALL, 0, NULL, CREATE_ALWAYS, NULL, NULL);
		hRead = CreateFile("file", GENERIC_READ, 0, NULL, CREATE_ALWAYS, NULL, NULL);

		CloseHandle(hRead2);
		ReportError("Результат ReportError:", NULL, true);
		CloseHandle(hRead);
		
		f = fopen("file", "rb");

		hRead = CreateFile("file", GENERIC_READ, 0, NULL, CREATE_ALWAYS, NULL, NULL);
		fclose(f);

		perror("Результат perror");

		CloseHandle(hRead);

		DeleteFileA("file");
		break;


	default:
		/*hRead = CreateFile("file", GENERIC_ALL, 0, NULL, CREATE_ALWAYS, NULL, NULL);
		system("pause");
		CloseHandle(hRead);
		DeleteFileA("file");*/


		ReportError("Результат ReportError:", NULL, true);
		perror("Результат perror");
		break;
	}

    return 0;
}

