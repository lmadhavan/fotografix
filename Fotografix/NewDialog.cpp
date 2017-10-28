// NewDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "NewDialog.h"


// CNewDialog dialog

IMPLEMENT_DYNAMIC(CNewDialog, CDialog)

CNewDialog::CNewDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CNewDialog::IDD, pParent)
{
	CWinApp *app = AfxGetApp();

	const CRect &rect = globals.clipboard.GetPosition();
	if (rect.IsRectEmpty() == true) {
		w = app->GetProfileInt(TEXT("New"), TEXT("Width"), 64000) / 100.0f;
		h = app->GetProfileInt(TEXT("New"), TEXT("Height"), 48000) / 100.0f;
		unit = app->GetProfileInt(TEXT("New"), TEXT("Unit"), 0);
	} else {
		w = rect.Width();
		h = rect.Height();
		unit = 0;
	}

	res = app->GetProfileInt(TEXT("New"), TEXT("Resolution"), 72);
	fill = app->GetProfileInt(TEXT("New"), TEXT("Contents"), 0);
}

void CNewDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_W1, w);
	DDX_Text(pDX, IDC_H1, h);
	DDV_MinMaxFloat(pDX, w, 0.01f, 32000.0f);
	DDV_MinMaxFloat(pDX, h, 0.01f, 32000.0f);
	DDX_Text(pDX, IDC_RES, res);
	DDX_CBIndex(pDX, IDC_FILL, fill);
	DDX_CBIndex(pDX, IDC_UNIT, unit);

	DDX_Control(pDX, IDC_RESLABEL, wReslabel);
	DDX_Control(pDX, IDC_RES, wRes);
	DDX_Control(pDX, IDC_RESUNIT, wResunit);
}


BEGIN_MESSAGE_MAP(CNewDialog, CDialog)
	ON_CBN_SELCHANGE(IDC_UNIT, &CNewDialog::OnCbnSelchangeUnits)
	ON_BN_CLICKED(IDOK, &CNewDialog::OnBnClickedOk)
	ON_BN_CLICKED(IDC_PRESET, &CNewDialog::OnBnClickedPreset)
END_MESSAGE_MAP()


// CNewDialog message handlers

void CNewDialog::OnCbnSelchangeUnits()
{
	int oldUnit = unit;

	UpdateData(true);

	if (oldUnit == 0 && unit > 0) {			// pixels to another unit
		w /= res;
		h /= res;
	} else if (oldUnit > 0 && unit == 0) {	// another unit to pixels
		w *= res;
		h *= res;
	} else if (oldUnit == 1 && unit == 2) {	// inches to centimetres
		w *= 2.54f;
		h *= 2.54f;
		res /= 2.54f;
	} else if (oldUnit == 2 && unit == 1) { // centimetres to inches
		w /= 2.54f;
		h /= 2.54f;
		res *= 2.54f;
	}

	UpdateUnits();
	UpdateData(false);
}

void CNewDialog::OnBnClickedOk()
{
	if (UpdateData() == false)
		return;

	CWinApp *app = AfxGetApp();
	app->WriteProfileInt(TEXT("New"), TEXT("Width"), w * 100);
	app->WriteProfileInt(TEXT("New"), TEXT("Height"), h * 100);
	app->WriteProfileInt(TEXT("New"), TEXT("Unit"), unit);
	app->WriteProfileInt(TEXT("New"), TEXT("Resolution"), res);
	app->WriteProfileInt(TEXT("New"), TEXT("Contents"), fill);

	if (unit > 0) {
		w *= res;
		h *= res;
	}

	w = max(1, w);
	h = max(1, h);

	UpdateData(false);
	EndDialog(IDOK);
}

void CNewDialog::OnBnClickedPreset()
{
	CMenu menu;
	menu.LoadMenu(IDR_POPUP);

	CMenu *popup = menu.GetSubMenu(2);
	TranslateMenu(popup);
	if (globals.clipboard.GetPosition().IsRectEmpty() == true) popup->EnableMenuItem(ID_PRESET_CLIPBOARD, MF_GRAYED);
	if (::IsClipboardFormatAvailable(CF_BITMAP) == false) popup->EnableMenuItem(ID_PRESET_WINDOWS, MF_GRAYED);

	CRect rect;
	GetDlgItem(IDC_PRESET)->GetWindowRect(rect);

	switch (popup->TrackPopupMenu(TPM_LEFTALIGN | TPM_RETURNCMD, rect.left, rect.bottom, this)) {
	case ID_PRESET_A4:
		unit = 1, w = 8.3f, h = 11.7f;
		break;

	case ID_PRESET_A3:
		unit = 1, w = 11.7f, h = 16.5f;
		break;

	case ID_PRESET_A2:
		unit = 1, w = 16.5f, h = 23.4f;
		break;

	case ID_PRESET_LETTER:
		unit = 1, w = 8.5f, h = 11.0f;
		break;

	case ID_PRESET_CLIPBOARD:
		{
			const CRect &rect = globals.clipboard.GetPosition();
			if (rect.IsRectEmpty() == false) unit = 0, w = rect.Width(), h = rect.Height();
		}
		break;

	case ID_PRESET_WINDOWS:
		if (::IsClipboardFormatAvailable(CF_BITMAP) == true && ::OpenClipboard(NULL) == true) {
			BITMAP bitmap;
			if (::GetObject(::GetClipboardData(CF_BITMAP), sizeof(bitmap), &bitmap) != 0) unit = 0, w = bitmap.bmWidth, h = bitmap.bmHeight;

			::CloseClipboard();
		}
		break;
	}

	UpdateUnits();
	UpdateData(false);
}

BOOL CNewDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	TranslateDialog(this);
	UpdateData(false);
	UpdateUnits();

	return true;
}

void CNewDialog::UpdateUnits() {
	if (unit == 0) {
		wReslabel.ShowWindow(SW_HIDE);
		wRes.ShowWindow(SW_HIDE);
		wResunit.ShowWindow(SW_HIDE);
	} else {
		wReslabel.ShowWindow(SW_SHOW);
		wRes.ShowWindow(SW_SHOW);
		wResunit.ShowWindow(SW_SHOW);
		wResunit.SetWindowText(unit == 1 ? LangItem(pixels_per_inch) : LangItem(pixels_per_centimetre));
	}
}
