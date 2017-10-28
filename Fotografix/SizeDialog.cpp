// SizeDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "SizeDialog.h"


// CSizeDialog dialog

IMPLEMENT_DYNAMIC(CSizeDialog, CDialog)

CSizeDialog::CSizeDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CSizeDialog::IDD, pParent)
	, resample(true)
	, constrain(true)
{

}

CSizeDialog::~CSizeDialog()
{
}

void CSizeDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_W1, w);
	DDX_Text(pDX, IDC_H1, h);
	DDV_MinMaxFloat(pDX, w, 0.01f, 32000.0f);
	DDV_MinMaxFloat(pDX, h, 0.01f, 32000.0f);
	DDX_Text(pDX, IDC_RES, res);
	DDX_CBIndex(pDX, IDC_UNIT, unit);
	DDX_Check(pDX, IDC_RESAMPLE, resample);
	DDX_Check(pDX, IDC_CONSTRAIN, constrain);

	DDX_Control(pDX, IDC_W1, wW);
	DDX_Control(pDX, IDC_H1, wH);
	DDX_Control(pDX, IDC_RESLABEL, wReslabel);
	DDX_Control(pDX, IDC_RES, wRes);
	DDX_Control(pDX, IDC_RESUNIT, wResunit);
	DDX_Control(pDX, IDC_CONSTRAIN, wConstrain);
}


BEGIN_MESSAGE_MAP(CSizeDialog, CDialog)
	ON_BN_CLICKED(IDOK, &CSizeDialog::OnBnClickedOk)
	ON_BN_CLICKED(IDC_RESAMPLE, &CSizeDialog::OnBnClickedResample)
	ON_BN_CLICKED(IDC_CONSTRAIN, &CSizeDialog::OnBnClickedConstrain)
	ON_EN_KILLFOCUS(IDC_W1, &CSizeDialog::OnEnKillfocusW)
	ON_EN_KILLFOCUS(IDC_H1, &CSizeDialog::OnEnKillfocusH)
	ON_EN_KILLFOCUS(IDC_RES, &CSizeDialog::OnEnKillfocusRes)
	ON_CBN_SELCHANGE(IDC_UNIT, &CSizeDialog::OnCbnSelchangeUnits)
END_MESSAGE_MAP()


// CSizeDialog message handlers

void CSizeDialog::OnCbnSelchangeUnits()
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

void CSizeDialog::OnBnClickedOk()
{
	if (UpdateData() == false)
		return;

	if (unit > 0) {
		w *= res;
		h *= res;
	}

	w = max(1, w);
	h = max(1, h);

	UpdateData(false);
	EndDialog(IDOK);
}

void CSizeDialog::OnBnClickedResample()
{
	if (UpdateData()) {
		if (resample == false) {
			constrain = true;
			UpdateData(false);
			wConstrain.EnableWindow(false);
		} else
			wConstrain.EnableWindow(true);

		UpdateUnits();
	}
}

void CSizeDialog::OnBnClickedConstrain()
{
	if (UpdateData() && constrain == true) {
		h = w * oh / ow;
		UpdateData(false);
	}
}

void CSizeDialog::OnEnKillfocusW()
{
	int oldW = w;

	if (UpdateData()) {
		if (constrain == true)
			h = w * oh / ow;

		if (resample == false && unit > 0)
			res = oldW * res / w;

		UpdateData(false);
	}
}

void CSizeDialog::OnEnKillfocusH()
{
	int oldW = w;

	if (UpdateData()) {
		if (constrain == true)
			w = h * ow / oh;

		if (resample == false && unit > 0)
			res = oldW * res / w;

		UpdateData(false);
	}
}

void CSizeDialog::OnEnKillfocusRes()
{
	int oldRes = res;

	if (UpdateData() && unit > 0) {
		if (resample == false) {
			w = w * oldRes / res;
			h = h * oldRes / res;
			UpdateData(false);
		}
	}
}

BOOL CSizeDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	TranslateDialog(this);

	if (unit > 0) {
		w = float(ow) / res;
		h = float(oh) / res;
	} else {
		w = ow;
		h = oh;
	}

	UpdateUnits();
	UpdateData(false);

	return true;
}

void CSizeDialog::UpdateUnits() {
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

	if (resample == false && unit == 0) {
		wW.EnableWindow(false);
		wH.EnableWindow(false);
	} else {
		wW.EnableWindow(true);
		wH.EnableWindow(true);
	}
}
