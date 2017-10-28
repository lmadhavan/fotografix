#pragma once

class CFotografixView;

class FGXScript
{
private:
	FGXScript() { }
	static int GetValue(CString str, CFotografixView &target);

public:
	static void Initialize();
	static bool Execute(LPCTSTR scriptPath, CFotografixView &target);

private:
	static CMapStringToPtr map;
};
