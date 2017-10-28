// AdjustDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "AdjustDialog.h"
#include "FotografixDoc.h"

// CAdjustDialog dialog

IMPLEMENT_DYNAMIC(CAdjustDialog, CDialog)

CAdjustDialog::CAdjustDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CAdjustDialog::IDD, pParent)
	, preview(true)
	, change(true)
	, adjust(false)
	, adjustLayer(false)
	, pDoc(NULL)
{
	min[0] = min[1] = min[2] = 0;
	max[0] = max[1] = max[2] = 0;
	def[0] = def[1] = def[2] = 0;
	value[0] = value[1] = value[2] = 0;
}

CAdjustDialog::~CAdjustDialog()
{
}

void CAdjustDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Check(pDX, IDC_PREVIEW, preview);

	DDX_Text(pDX, IDC_LABEL1, label[0]);
	DDX_Text(pDX, IDC_EDIT1, value[0]);
	DDX_Control(pDX, IDC_SLIDER1, slider[0]);
	DDV_MinMaxInt(pDX, value[0], min[0], max[0]);

	DDX_Text(pDX, IDC_LABEL2, label[1]);
	DDX_Text(pDX, IDC_EDIT2, value[1]);
	DDX_Control(pDX, IDC_SLIDER2, slider[1]);
	DDV_MinMaxInt(pDX, value[1], min[1], max[1]);

	DDX_Text(pDX, IDC_LABEL3, label[2]);
	DDX_Text(pDX, IDC_EDIT3, value[2]);
	DDX_Control(pDX, IDC_SLIDER3, slider[2]);
	DDV_MinMaxInt(pDX, value[2], min[2], max[2]);
}


BEGIN_MESSAGE_MAP(CAdjustDialog, CDialog)
	ON_WM_HSCROLL()
	ON_EN_CHANGE(IDC_EDIT1, &CAdjustDialog::OnEnChangeEdit1)
	ON_EN_CHANGE(IDC_EDIT2, &CAdjustDialog::OnEnChangeEdit2)
	ON_EN_CHANGE(IDC_EDIT3, &CAdjustDialog::OnEnChangeEdit3)
	ON_WM_TIMER()
	ON_BN_CLICKED(IDOK, &CAdjustDialog::OnBnClickedOk)
	ON_BN_CLICKED(IDCANCEL, &CAdjustDialog::OnBnClickedCancel)
	ON_BN_CLICKED(IDC_PREVIEW, &CAdjustDialog::OnBnClickedPreview)
	ON_BN_CLICKED(IDC_RESET1, &CAdjustDialog::OnBnClickedReset1)
	ON_BN_CLICKED(IDC_RESET2, &CAdjustDialog::OnBnClickedReset2)
	ON_BN_CLICKED(IDC_RESET3, &CAdjustDialog::OnBnClickedReset3)
END_MESSAGE_MAP()


// CAdjustDialog message handlers

BOOL CAdjustDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	GetDlgItem(IDOK)->SetWindowText(LangItem(OK));
	GetDlgItem(IDCANCEL)->SetWindowText(LangItem(Cancel));
	GetDlgItem(IDC_PREVIEW)->SetWindowText(LangItem(Preview));

	SetWindowText(title);

	for (int i = 0; i < num; i++) {
		slider[i].SetPos(1);
		slider[i].SetRange(min[i], max[i]);
		slider[i].SetPos(value[i] = def[i]);
		slider[i].SetTicFreq(50);
	}

	for (int i = num; i < 3; i++) {
		GetDlgItem(IDC_LABEL1 + i)->ShowWindow(SW_HIDE);
		GetDlgItem(IDC_RESET1 + i)->ShowWindow(SW_HIDE);
		GetDlgItem(IDC_EDIT1 + i)->ShowWindow(SW_HIDE);
		GetDlgItem(IDC_RESET1 + i)->ShowWindow(SW_HIDE);
		slider[i].ShowWindow(SW_HIDE);
	}

	if (adjustLayer == true) GetDlgItem(IDC_PREVIEW)->EnableWindow(false);

	if (num < 3) {
		const int height[2] = { 130, 180 };

		CRect rect;
		GetWindowRect(rect);

		SetWindowPos(NULL, 0, 0, rect.right, height[num - 1], SWP_NOZORDER | SWP_NOMOVE);
	}

	UpdateData(false);
	SetTimer(1, 1000, NULL);

	return true;
}

void CAdjustDialog::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	if ((CWnd *)pScrollBar == (CWnd *)&slider[0])
		value[0] = slider[0].GetPos();
	else if ((CWnd *)pScrollBar == (CWnd *)&slider[1])
		value[1] = slider[1].GetPos();
	else if ((CWnd *)pScrollBar == (CWnd *)&slider[2])
		value[2] = slider[2].GetPos();

	change = true;
	UpdateData(false);
}

void CAdjustDialog::OnEnChangeEdit1()
{
	if (UpdateData()) {
		slider[0].SetPos(value[0]);
		change = true;
	}
}

void CAdjustDialog::OnEnChangeEdit2()
{
	if (UpdateData()) {
		slider[1].SetPos(value[1]);
		change = true;
	}
}

void CAdjustDialog::OnEnChangeEdit3()
{
	if (UpdateData()) {
		slider[2].SetPos(value[2]);
		change = true;
	}
}

void CAdjustDialog::OnTimer(UINT_PTR nIDEvent)
{
	if (nIDEvent == 1) {
		if (change == true) {
			change = false;

			if (preview == true && pDoc != NULL && UpdateData()) {
				if (adjust == true) {
					adjust = false;
					pDoc->Undo();
				}

				adjust = true;

				if (adjustLayer == true) {
					pDoc->SaveUndoModifyLayer(layer);

					FGXLayer *l = pDoc->image.GetLayer(layer);
					l->a1 = value[0];
					l->a2 = value[1];
					l->a3 = value[2];

					pDoc->RedrawLayer(layer);
				} else if (pDoc->selection.GetPosition().IsRectEmpty()) {
					pDoc->image.GetLayer(layer)->Adjust(pDoc->GetUndoLayer(layer, title), adjustType, value[0], value[1], value[2], channelMask);
					pDoc->RedrawLayer(layer);
				} else {
					pDoc->image.GetLayer(layer)->Adjust(pDoc->GetUndoLayer(layer, title), pDoc->selection, adjustType, value[0], value[1], value[2], channelMask);
					pDoc->RedrawLayerSelection(layer);
				}
			}
		}
	} else
		CDialog::OnTimer(nIDEvent);
}

void CAdjustDialog::OnBnClickedOk()
{
	if (UpdateData()) {
		KillTimer(1);

		if (change == true || adjust == false) {
			change = true;
			preview = true;
			OnTimer(1);
		}

		if (adjustLayer == true)
			pDoc->Undo();

		pDoc->SetModifiedFlag(true);
		EndDialog(IDOK);
	}
}

void CAdjustDialog::OnBnClickedCancel()
{
	KillTimer(1);

	if (adjust == true) {
		pDoc->Undo();
		pDoc->ClearRedo();
	}

	EndDialog(IDCANCEL);
}

void CAdjustDialog::OnBnClickedPreview()
{
	UpdateData();

	if (preview == true) {
		if (change == false)
			change = true;
	} else {
		if (adjust == true) {
			adjust = false;
			pDoc->Undo();
		}
	}
}

void CAdjustDialog::OnBnClickedReset1()
{
	if (value[0] != def[0]) {
		slider[0].SetPos(value[0] = def[0]);
		UpdateData(false);
		change = true;
	}
}

void CAdjustDialog::OnBnClickedReset2()
{
	if (value[1] != def[1]) {
		slider[1].SetPos(value[1] = def[1]);
		UpdateData(false);
		change = true;
	}
}

void CAdjustDialog::OnBnClickedReset3()
{
	if (value[2] != def[2]) {
		slider[2].SetPos(value[2] = def[2]);
		UpdateData(false);
		change = true;
	}
}
