#include "stdafx.h"
#include "Language.h"
#include <cstdio>

CMapStringToString langMap;
static bool langInit = false;

bool LoadLanguage(LPCTSTR langFile) {
	FILE *file;

	if ((file = _tfopen(langFile, TEXT("r,ccs=UTF-8"))) != NULL) {
		CString line;
		TCHAR buffer[256];
		while (_fgetts(buffer, 256, file) != NULL) {
			line = buffer;
			line.Trim('\n');

			if (line.IsEmpty() == true || line[0] == TEXT('#')) continue;

			int pos = line.Find('=');
			if (pos > -1) langMap[line.Left(pos).Trim()] = line.Mid(pos + 1).Trim();
		}

		fclose(file);
		langInit = true;
		return true;
	}

	return false;
}

CString GetLangMessage(LPCTSTR key, LPCTSTR param) {
	CString result;

	if (LangTranslate(key, result) == true) {
		result.Replace(TEXT("\\t"), TEXT("\t"));
		result.Replace(TEXT("\\n"), TEXT("\n"));
		if (param != NULL) result.Replace(TEXT("%s"), param);
		return result;
	}

	result.Format(TEXT("Localized string \"%s\" not found."), key);
	return result;
}

CString GetLangItemEarly(LPCTSTR key) {
	return langInit == true ? GetLangItem(key) : key;
}

CString GetLangItem(LPCTSTR key) {
	CString result;

	if (LangTranslate(key, result) == true)
		return result;

	result.Format(TEXT("%s (missing string)"), key);
	return result;
}

CString GetLangItem(LPCTSTR key, LPCTSTR param) {
	CString result;

	if (LangTranslate(key, result) == true) {
		result.Replace(TEXT("%s"), param);
		return result;
	}

	result.Format(TEXT("%s (missing string)"), key);
	return result;
}

CString GetMenuItem(LPCTSTR key) {
	CString result;

	if (LangTranslate(key, result) == true)
		return result;

	return key;
}

static BOOL CALLBACK TranslateDialogEnum(HWND hWnd, LPARAM lParam) {
	static TCHAR buf[256];

	GetClassName(hWnd, buf, 256);

	if (_tcsicmp(buf, TEXT("Static")) == 0 ||
		_tcsicmp(buf, TEXT("Button")) == 0) {
			int len = GetWindowText(hWnd, buf, 256);
			bool prompt;

			if (buf[len - 1] == ':') {
				prompt = true;
				buf[len - 1] = 0;
			} else
				prompt = false;

			CString text;
			if (LangTranslate(buf, text) == true) {
				text.Replace(TEXT("\\t"), TEXT("\t"));
				text.Replace(TEXT("\\n"), TEXT("\n"));
				if (prompt == true) text.AppendChar(':');
				SetWindowText(hWnd, text);
			}
	} else if (_tcsicmp(buf, TEXT("ComboBox")) == 0) {
		if (SendMessage(hWnd, CB_GETLBTEXT, 0, (LPARAM)buf) != CB_ERR) {
			CString list;
			if (LangTranslate(buf, list) == true) {
				SendMessage(hWnd, CB_RESETCONTENT, 0, 0);

				int pos = 0;
				CString str;
				str = list.Tokenize(TEXT(";"), pos);
				while (pos > -1) {
					SendMessage(hWnd, CB_ADDSTRING, 0, (LPARAM)(LPCTSTR)str);
					str = list.Tokenize(TEXT(";"), pos);
				}
			}
			SendMessage(hWnd, CB_SETCURSEL, 0, 0);
		}
	}

	return TRUE;
}

void TranslateDialog(CWnd *dlg) {
	CString oldTitle, newTitle;
	dlg->GetWindowText(oldTitle);
	if (LangTranslate(oldTitle, newTitle) == true)
		dlg->SetWindowText(newTitle);

	EnumChildWindows(dlg->m_hWnd, TranslateDialogEnum, 0);
}

void TranslateMenu(CMenu *menu) {
	int n = menu->GetMenuItemCount();
	for (int i = 0; i < n; i++) {
		int id = menu->GetMenuItemID(i);
		if (id == 0) continue;

		CString str, accel;
		TCHAR accelKey = 0;
		bool ellip = false;
		int pos;
		
		menu->GetMenuString(i, str, MF_BYPOSITION);

		if ((pos = str.Find('\t')) > -1) {
			accel = str.Mid(pos + 1);
			str = str.Left(pos);
		}
		if (str.Right(3) == TEXT("...")) {
			ellip = true;
			str = str.Left(str.GetLength() - 3);
		}
		if ((pos = str.Find('&')) > -1) {
			if (pos + 1 < str.GetLength())
				accelKey = _totupper(str[pos + 1]);
			str.Remove('&');
		}

		str = GetMenuItem(str);
		str.Replace(TEXT("&"), TEXT("&&"));
		if (accelKey > 0) {
			if ((pos = str.Find(accelKey)) > -1 || (pos = str.Find(_totlower(accelKey))) > -1)
				str.Insert(pos, '&');
			else
				str.AppendFormat(TEXT(" (&%c)"), accelKey);
		}
		if (ellip == true)
			str.Append(TEXT("..."));
		if (accel.IsEmpty() == false)
			str.AppendFormat(TEXT("\t%s"), accel);

		MENUITEMINFO mii = { sizeof(MENUITEMINFO), MIIM_STRING };
		mii.cch = str.GetLength();
		mii.dwTypeData = str.GetBuffer();
		menu->SetMenuItemInfo(i, &mii, TRUE);
		str.ReleaseBuffer();

		if (id == -1) TranslateMenu(menu->GetSubMenu(i));
	}
}
