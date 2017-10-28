#pragma once
#include "afxcmn.h"


// CJPEGDialog dialog

class CJPEGDialog : public CDialog
{
	DECLARE_DYNAMIC(CJPEGDialog)

public:
	CJPEGDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CJPEGDialog();

// Dialog Data
	enum { IDD = IDD_JPEG_OPTIONS };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

public:
	int quality;
	CSliderCtrl slider;

public:
	virtual BOOL OnInitDialog();
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnEnChangeEdit1();
};
