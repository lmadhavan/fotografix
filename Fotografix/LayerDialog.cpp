// LayerDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "LayerDialog.h"


// CLayerDialog dialog

IMPLEMENT_DYNAMIC(CLayerDialog, CDialog)

CLayerDialog::CLayerDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CLayerDialog::IDD, pParent)
	, mode(0)
{

}

CLayerDialog::~CLayerDialog()
{
}

void CLayerDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_OPACITY, opacity);
	DDV_MinMaxInt(pDX, opacity, 0, 100);
	DDX_CBIndex(pDX, IDC_MODE, mode);
}


BEGIN_MESSAGE_MAP(CLayerDialog, CDialog)
END_MESSAGE_MAP()


// CLayerDialog message handlers

BOOL CLayerDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	TranslateDialog(this);
	UpdateData(false);

	return TRUE;
}
