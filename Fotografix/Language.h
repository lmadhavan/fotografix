#pragma once

extern CMapStringToString langMap;

inline bool LangTranslate(LPCTSTR key, CString &result) {
	return langMap.Lookup(key, result) == TRUE;
}

bool LoadLanguage(LPCTSTR langFile);
CString GetLangMessage(LPCTSTR key, LPCTSTR param = NULL);
CString GetLangItemEarly(LPCTSTR key);
CString GetLangItem(LPCTSTR key);
CString GetLangItem(LPCTSTR key, LPCTSTR param);
CString GetMenuItem(LPCTSTR key);
void TranslateDialog(CWnd *dlg);
void TranslateMenu(CMenu *menu);

#define LangMessage(key) GetLangMessage(TEXT(#key))
#define LangMessageParam(key, param) GetLangMessage(TEXT(#key), param)
#define LangItem(key) GetLangItem(TEXT(#key))
#define LangItemParam(key, param) GetLangItem(TEXT(#key), param)
